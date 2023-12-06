using System.Net.Mail;

namespace Fortress.Domain.Rules
{
    public class UserRules
    {
        public static bool IsValidName(string name)
        {
            if (name == null)
                return false;

            return name.Length >= 3;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
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
            if (password == null || password.Length < 12)
                return false;

            var hasAnyUpper = false;
            var hasAnyLower = false;
            var hasAnyNumber = false;
            var hasAnySymbol = false;

            foreach (var character in password)
            {
                if (char.IsUpper(character))
                    hasAnyUpper = true;

                if (char.IsLower(character))
                    hasAnyLower = true;

                if (char.IsNumber(character))
                    hasAnyNumber = true;

                if (char.IsSymbol(character) || char.IsPunctuation(character))
                    hasAnySymbol = true;
            }

            return hasAnyUpper
                && hasAnyLower
                && hasAnyNumber
                && hasAnyNumber
                && hasAnySymbol;
        }
    }
}
