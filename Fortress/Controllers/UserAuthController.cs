using static Microsoft.AspNetCore.Http.StatusCodes;
using Google.Authenticator;
using Fortress.Infrastructure.Repository;
using Fortress.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Fortress.Domain.Enums;

namespace Fortress.Controllers
{
    [ApiController]
    [Route("userAuth")]
    public class UserAuthController : ControllerBase
    {
        private readonly TimeSpan _userAuthExpirationTime;
        private readonly string _otpAuthIssuer;
        private readonly TwoFactorAuthenticator _otpAuthenticator = new();
        private readonly PasswordHasher<User> _passwordHasher = new();

        private readonly IUserRepository _userRespository;
        private readonly IUserAuthRepository _userAuthRespository;

        public UserAuthController(
            IConfiguration configuration,
            IUserRepository userRespository,
            IUserAuthRepository userAuthRespository)
        {
            _userAuthExpirationTime = TimeSpan.FromSeconds(int.Parse(configuration["UserAuthExpirationInSeconds"]));
            _otpAuthIssuer = configuration["OTPAuthIssuer"];
            _userRespository = userRespository;
            _userAuthRespository = userAuthRespository;
        }

        [HttpPatch("email")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public ActionResult PatchEmail([FromBody] AuthenticateUserEmailRequest request)
        {
            var user = _userRespository.GetByEmail(request.Email);

            if (user == null)
                return BadRequest("User does not exist.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result != PasswordVerificationResult.Success)
                return BadRequest("Wrong password.");

            var userAuth = _userAuthRespository.GetByUserId(user.Id);

            if (userAuth == null)
            {
                var newUserAuth = new UserAuth()
                {
                    UserId = user.Id,
                    AuthFactorId = AuthFactorEnum.Email,
                    LastAuthTimeUtc = DateTime.UtcNow
                };

                _userAuthRespository.Add(newUserAuth);
            }
            else if (userAuth.HasExpired(_userAuthExpirationTime))
            {
                userAuth.LastAuthTimeUtc = DateTime.UtcNow;

                _userAuthRespository.Update(userAuth);
            }

            return Ok();
        }

        [HttpPatch("otp")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        public ActionResult PatchOTP([FromBody] AuthenticateUserOTPRequest request)
        {
            //todo: verificar se está autenticado com email

            var isValid = _otpAuthenticator.ValidateTwoFactorPIN(
                SampleData.OTPAuthSecretKey,
                request.Code);

            return isValid ? Ok() : BadRequest();
        }

        [HttpGet("otp/setup")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        public ActionResult<GetUserOTPSetupResponse> GetOTPSetup([FromQuery] GetUserOTPSetupRequest request)
        {
            //todo: verificar se está autenticado com email

            //for frontend testing
            var setup = _otpAuthenticator.GenerateSetupCode(
                _otpAuthIssuer,
                SampleData.Email,
                SampleData.OTPAuthSecretKey,
                false);

            var response = new GetUserOTPSetupResponse()
            {
                AccountName = SampleData.Email,
                Key = setup.ManualEntryKey,
                QrCode = setup.QrCodeSetupImageUrl
            };

            return Ok(response);
        }

        [HttpPatch("check")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        public ActionResult PatchCheck()
        {
            //todo: enquanto não tiver bearer token, verificar se está autenticado com email e otp
            return Ok();
        }
    }

    public static class SampleData
    {
        public static readonly Guid UserId = new Guid("0a1a80a4-a801-486e-891a-94bbf0516442");
        public static readonly string Email = "tucanemo@gmail.com";

        public static string OTPAuthSecretKey
            => UserId.ToString().Replace("-", "").Substring(0, 10);
    }
}
