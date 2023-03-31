using LibraryAPI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Infrastructure.Interface
{
    public interface IBorrowedBookRepository : IGenericRepository<BorrowedBook>
    {
    }
}
