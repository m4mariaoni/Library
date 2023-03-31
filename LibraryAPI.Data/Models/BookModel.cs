using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Data.Models
{
    public class BookModel
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public string Year { get; set; }
        public int Copies { get; set; }
    }
    public class BorrowedBookModel
    {
        public string ISBN { get; set; }

    }

    public class BorrowedBookViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string ISBN { get; set; }
        public DateTime DateBorrowed { get; set; }
        public DateTime? DateReturned { get; set; }
        public int? OverDue { get; set; }
    }

    public class BorrowedBookResponse
    {
        public string Message { get; set; }
        public List<BorrowedBookViewModel> Borrowed { get; set; }
    }


}
