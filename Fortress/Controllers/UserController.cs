using static Microsoft.AspNetCore.Http.StatusCodes;
using Fortress.Domain.Entities;
using Fortress.Domain.Rules;
using Fortress.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;

namespace Fortress.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly IUserRepository _userRespository;
        public UserController(IUserRepository userRespository)
        {
            _userRespository = userRespository;
        }

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public ActionResult<CreateUserResponse> Post([FromBody] CreateUserRequest request)
        {
            if (!UserRules.IsValidName(request.Name))
                return BadRequest("Invalid name.");

            if (!UserRules.IsValidEmail(request.Email))
                return BadRequest("Invalid email.");

            if (!UserRules.IsValidPassword(request.Password))
                return BadRequest("Invalid password.");

            var userDb = _userRespository.GetByEmail(request.Email);

            if (userDb != null)
                return BadRequest("Email already in use.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _userRespository.Add(user);

            return Created(string.Empty, new CreateUserResponse { Id = user.Id });
        }
    }
}
