using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Data.Models
{
    public class InvoiceModel
    {
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int Type { get; set; }
        public string StudentId { get; set; }  

    }


    public class InvoiceViewModel
    {
        public long Id { get; set; }
        public string Reference { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string StudentId { get; set; }
    }

}