namespace JewelleryStore.Common
{
    public static class ErrorMessageConstants
    {
        public const string InvalidUser = "Invalid username or password";

        public const string EmptyUsername = "Username cannot be null or empty";

        public const string EmptyPassword = "Password cannot be null or empty";

        public const string InvalidGoldPrice = "Gold price should not be null or less than or equal to 0";

        public const string InvalidWeight = "Weight should not be null or less than or equal to 0";

        public const string InvalidConfigurationKey = "Value for key - {0}, has not been provided in the appSettings.Json file";
    }
}
