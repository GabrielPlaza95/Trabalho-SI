using System.Net.Mail;

namespace Fortress.Domain.Rules
{
    public class UserRules
    {
        public static bool IsValidName(string name)
        {
            if (name == null)
                return false;

            return name.Length is >= 3 and <= 256;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > 256)
            {
                return false;
            }

            try
            {
                var addr = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (password == null || password.Length is < 12 or > 256)
                return false;

            var hasUpper = false;
            var hasLower = false;
            var hasNumber = false;
            var hasSymbol = false;

            foreach (var character in password)
            {
                if (char.IsUpper(character))
                    hasUpper = true;

                if (char.IsLower(character))
                    hasLower = true;

                if (char.IsNumber(character))
                    hasNumber = true;

                if (char.IsSymbol(character) || char.IsPunctuation(character))
                    hasSymbol = true;
            }

            return hasUpper
                && hasLower
                && hasNumber
                && hasNumber
                && hasSymbol;
        }
    }
}
