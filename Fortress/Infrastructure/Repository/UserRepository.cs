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

        public User GetById(Guid id)
            => _context.Users.FirstOrDefault(x => x.Id == id);
        
    }

    public interface IUserRepository
    {
        User GetById(Guid id);
    }
}
