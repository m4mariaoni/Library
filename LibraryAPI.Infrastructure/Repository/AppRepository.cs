using LibraryAPI.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Infrastructure.Repository
{
    public class AppRepository : IAppRepository
    {
        private readonly AppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IBookRepository Books { get; }
        public IBorrowedBookRepository BorrowedBooks { get; }

        public AppRepository(AppDbContext dbContext, IUserRepository userRepository,
            IBookRepository bookRepository, IBorrowedBookRepository borrowedBookRepository)
        {
            _dbContext = dbContext;
            Users = userRepository;
            Books = bookRepository;
            BorrowedBooks = borrowedBookRepository;
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }


    }
}