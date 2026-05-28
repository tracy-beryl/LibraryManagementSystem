using LibraryManagementSystem.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public interface IRecommendationService
    {
        Task<List<RecommendedBookItemViewModel>> GetRecommendedBooksForUserAsync(string userId, int count = 6);
    }
}