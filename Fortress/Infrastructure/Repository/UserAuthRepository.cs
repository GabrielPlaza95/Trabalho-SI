using Fortress.Domain.Entities;
using Fortress.Infrastructure.Context;

namespace Fortress.Infrastructure.Repository
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly FortressContext _context;

        public UserAuthRepository(FortressContext context)
        {
            _context = context;
        }

        public UserAuth GetById(long id)
            => _context.UserAuths.FirstOrDefault(x => x.Id == id);
        
    }

    public interface IUserAuthRepository
    {
        UserAuth GetById(long id);
    }
}
