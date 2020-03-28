namespace Shared
{
    public static class PhoneNumber
    {
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
