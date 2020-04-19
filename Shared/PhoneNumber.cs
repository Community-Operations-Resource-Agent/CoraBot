namespace Shared
{
    public static class PhoneNumber
    {
        /// <summary>
        /// Simple validation of a phone number.
        /// </summary>
        public static bool IsValid(string phoneNumber)
        {
            return phoneNumber.Length == 12 && phoneNumber.StartsWith("+1");
        }

        /// <summary>
        /// Converts a phone number to +1XXXYYYZZZZ format.
        /// </summary>
        public static string Standardize(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber;
            }

            phoneNumber = StripSeparators(phoneNumber);

            if (phoneNumber.Length == 10)
            {
                phoneNumber = "1" + phoneNumber;
            }

            if (phoneNumber.Length == 11)
            {
                phoneNumber = "+" + phoneNumber;
            }

            return phoneNumber;
        }

        private static string StripSeparators(string phoneNumber)
        {
            return phoneNumber
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty)
                .Replace(".", string.Empty)
                .Replace(" ", string.Empty);
        }
    }
}
