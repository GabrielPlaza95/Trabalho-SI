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

            var userAuth = _userAuthRespository.GetByUserIdAndAuthFactor(user.Id, AuthFactorEnum.Email);

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
            else
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
            var user = _userRespository.GetById(request.UserId);

            if (user == null)
                return BadRequest("User does not exist.");

            var userAuthEmail = _userAuthRespository.GetByUserIdAndAuthFactor(user.Id, AuthFactorEnum.Email);

            if (userAuthEmail == null || userAuthEmail.HasExpired(_userAuthExpirationTime))
                return Unauthorized();

            var isOTPAuthenticated = _otpAuthenticator.ValidateTwoFactorPIN(
                GetOTPAuthSecretKey(user),
                request.Code);

            if (!isOTPAuthenticated)
                return Unauthorized();

            var userAuthOTP = _userAuthRespository.GetByUserIdAndAuthFactor(user.Id, AuthFactorEnum.OTP);

            if (userAuthOTP == null)
            {
                var newUserAuthOTP = new UserAuth()
                {
                    UserId = user.Id,
                    AuthFactorId = AuthFactorEnum.OTP,
                    LastAuthTimeUtc = DateTime.UtcNow
                };

                _userAuthRespository.Add(newUserAuthOTP);
            }
            else
            {
                userAuthOTP.LastAuthTimeUtc = DateTime.UtcNow;
                _userAuthRespository.Update(userAuthOTP);
            }

            userAuthEmail.LastAuthTimeUtc = DateTime.UtcNow;
            _userAuthRespository.Update(userAuthEmail);

            return Ok();
        }

        [HttpGet("otp/setup")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        public ActionResult<GetUserOTPSetupResponse> GetOTPSetup([FromQuery] GetUserOTPSetupRequest request)
        {
            var user = _userRespository.GetById(request.UserId);

            if (user == null)
                return BadRequest("User does not exist.");

            var userAuth = _userAuthRespository.GetByUserIdAndAuthFactor(user.Id, AuthFactorEnum.Email);

            if (userAuth == null || userAuth.HasExpired(_userAuthExpirationTime))
                return Unauthorized();

            var accountName = user.Email;

            var setup = _otpAuthenticator.GenerateSetupCode(
                _otpAuthIssuer,
                accountName,
                GetOTPAuthSecretKey(user),
                false);

            var response = new GetUserOTPSetupResponse()
            {
                AccountName = accountName,
                Key = setup.ManualEntryKey,
                QrCode = setup.QrCodeSetupImageUrl
            };

            return Ok(response);
        }

        [HttpPatch("check")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        public ActionResult PatchCheck([FromBody] CheckUserAuthenticationRequest request)
        {
            var userAuthEmail = _userAuthRespository.GetByUserIdAndAuthFactor(request.UserId, AuthFactorEnum.Email);

            if (userAuthEmail == null || userAuthEmail.HasExpired(_userAuthExpirationTime))
                return Unauthorized();

            var userAuthOTP = _userAuthRespository.GetByUserIdAndAuthFactor(request.UserId, AuthFactorEnum.OTP);

            if (userAuthOTP == null || userAuthOTP.HasExpired(_userAuthExpirationTime))
                return Unauthorized();

            return Ok();
        }

        private string GetOTPAuthSecretKey(User user)
            => user.Id.ToString().Replace("-", "").Substring(0, 10);

    }
}
