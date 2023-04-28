using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Data.Models
{
    public  class StudentModel
    {
        public long userId { get; set; }
        public string StudentId { get; set; }
        public int TotalLoan { get; set; }
        public int TotalOverDue { get; set; }
    }

    public class StudentLoanModel
    {
        public string Student { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Borrowed { get; set; }
    }
}

