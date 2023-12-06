namespace Fortress.Dtos
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserResponse
    {
        public Guid Id { get; set; }
    }

    public class AuthenticateUserEmailRequest
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticateUserOTPRequest
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }

    public class CheckUserAuthenticationRequest
    {
        public Guid UserId { get; set; }
    }

    public class GetUserOTPSetupRequest
    {
        public Guid UserId { get; set; }
    }

    public class GetUserOTPSetupResponse
    {
        public string AccountName { get; set; }
        public string Key { get; set; }
        public string QrCode { get; set; }
    }
}
