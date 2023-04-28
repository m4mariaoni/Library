using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Data.Entity
{
    public class BorrowedBook
    {
        public long Id { get; set; }
        //public string UserName { get; set; }
        //public string ISBN { get; set; }
        public long userId { get; set; }
        public virtual User User { get; set; }
        public long bookId { get; set; }
        public virtual Book Book { get; set; }
        public DateTime DateBorrowed { get; set; }
        public DateTime? DateReturned { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public int? OverDue { get; set; }
    }


}
