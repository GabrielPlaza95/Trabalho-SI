﻿using Fortress.Domain.Entities;
using Fortress.Domain.Enums;
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

        public void Add(UserAuth userAuth)
        {
            _context.UserAuths.Add(userAuth);
            _context.SaveChanges();
        }

        public void Update(UserAuth userAuth)
        {
            _context.UserAuths.Update(userAuth);
            _context.SaveChanges();
        }

        public UserAuth GetById(long id)
            => _context.UserAuths.FirstOrDefault(x => x.Id == id);

        public UserAuth GetByUserIdAndAuthFactor(Guid userId, AuthFactorEnum authFactor)
            => _context.UserAuths.FirstOrDefault(x => x.UserId == userId && x.AuthFactorId == authFactor);
    }

    public interface IUserAuthRepository
    {
        void Add(UserAuth userAuth);
        void Update(UserAuth userAuth);
        UserAuth GetById(long id);
        UserAuth GetByUserIdAndAuthFactor(Guid userId, AuthFactorEnum authFactor);
    }
}
