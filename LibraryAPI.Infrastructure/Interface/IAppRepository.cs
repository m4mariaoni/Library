using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Infrastructure.Interface
{
    public interface IAppRepository : IDisposable
    {
        IUserRepository Users { get; }

        int Save();

        IBookRepository Books { get; }
        IBorrowedBookRepository BorrowedBooks { get; }   

    }
}