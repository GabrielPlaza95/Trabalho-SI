using static Microsoft.AspNetCore.Http.StatusCodes;
using Google.Authenticator;

namespace Fortress.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public ActionResult<CreateUserResponse> Post([FromBody] CreateUserRequest request)
        {
            //for frontend testing
            var response = new CreateUserResponse()
            {
                Id = SampleData.UserId
            };

            return Created(string.Empty, response);
        }
    }

    [ApiController]
    [Route("userAuth")]
    public class UserAuthController : ControllerBase
    {
        private readonly string _otpAuthIssuer;
        private readonly TwoFactorAuthenticator _otpAuthenticator = new();

        public UserAuthController(IConfiguration configuration)
        {
            _otpAuthIssuer = configuration["OTPAuthIssuer"];
        }

        [HttpPatch("email")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public ActionResult PatchEmail([FromBody] AuthenticateUserEmailRequest request)
        {
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
