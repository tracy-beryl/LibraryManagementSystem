using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.Services
{
    public class DarajaService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public DarajaService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _http = httpClient;
        }

        private async Task<string> GetAccessToken()
        {
            var consumerKey = _config["Daraja:ConsumerKey"];
            var consumerSecret = _config["Daraja:ConsumerSecret"];

            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
                throw new Exception("Daraja consumer key or secret is missing.");

            var auth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}")
            );

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", auth);

            var response = await _http.GetAsync(
                "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials");

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get Daraja access token. Response: {json}");

            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("access_token", out var tokenElement))
                throw new Exception($"Daraja token response did not contain access_token. Response: {json}");

            return tokenElement.GetString();
        }

        public async Task<string> InitiateStkPush(string phone, decimal amount, string reference)
        {
            var token = await GetAccessToken();

            var normalizedPhone = NormalizePhoneNumber(phone);
            var callbackUrl = _config["Daraja:CallbackUrl"];
            var shortcode = _config["Daraja:ShortCode"];
            var passkey = _config["Daraja:PassKey"];

            if (string.IsNullOrWhiteSpace(callbackUrl))
                throw new Exception("Daraja callback URL is missing.");

            if (string.IsNullOrWhiteSpace(shortcode) || string.IsNullOrWhiteSpace(passkey))
                throw new Exception("Daraja shortcode or passkey is missing.");

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var password = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{shortcode}{passkey}{timestamp}")
            );

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                BusinessShortCode = shortcode,
                Password = password,
                Timestamp = timestamp,
                TransactionType = "CustomerPayBillOnline",
                Amount = (int)Math.Ceiling(amount),
                PartyA = normalizedPhone,
                PartyB = shortcode,
                PhoneNumber = normalizedPhone,
                CallBackURL = callbackUrl,
                AccountReference = reference,
                TransactionDesc = "Library Payment"
            };

            var payloadJson = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                payloadJson,
                Encoding.UTF8,
                "application/json");

            var response = await _http.PostAsync(
                "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest",
                content);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Daraja STK Push failed. Response: {json}");

            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("errorMessage", out var errorMessage))
                throw new Exception($"Daraja returned an error: {errorMessage.GetString()}");

            if (!doc.RootElement.TryGetProperty("CheckoutRequestID", out var checkoutElement))
                throw new Exception($"Daraja response missing CheckoutRequestID. Response: {json}");

            return checkoutElement.GetString();
        }

        private string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new Exception("Phone number is missing.");

            phone = phone.Trim().Replace(" ", "");

            if (phone.StartsWith("+"))
                phone = phone.Substring(1);

            if (phone.StartsWith("0"))
                phone = "254" + phone.Substring(1);

            if (!phone.StartsWith("254"))
                throw new Exception("Phone number must be in Kenyan format, e.g. 07XXXXXXXX or 2547XXXXXXXX.");

            return phone;
        }
    }
}