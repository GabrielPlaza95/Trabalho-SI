using Fortress.Domain.Entities;
using Fortress.Infrastructure.Context;

namespace Fortress.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly FortressContext _context;

        public UserRepository(FortressContext context)
        {
            _context = context;        
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetById(Guid id)
            => _context.Users.FirstOrDefault(x => x.Id == id);

        public User GetByEmail(string email)
        {
            email = email.ToLower();
            return _context.Users.FirstOrDefault(x => x.Email.ToLower() == email);
        }
    }

    public interface IUserRepository
    {
        void Add(User user);
        User GetById(Guid id);
        User GetByEmail(string email);
    }
}
