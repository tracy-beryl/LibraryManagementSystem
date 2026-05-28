using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string AdmissionNumber { get; set; }
        public string Program { get; set; }
        public string Level { get; set; }

        public ICollection<Loan> Loans { get; set; }

        public string GetCourse()
        {
            if (string.IsNullOrEmpty(AdmissionNumber) || AdmissionNumber.Length < 4)
                return null;

            return AdmissionNumber.Substring(0, 3);
        }

        public string GetLevel()
        {
            if (string.IsNullOrEmpty(AdmissionNumber) || AdmissionNumber.Length < 4)
                return null;

            return AdmissionNumber.Substring(3, 1);
        }
    }
}
