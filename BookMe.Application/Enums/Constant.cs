namespace BookMe.Application.Enums;

public static class Constant
{
    public const string HTTP_CONTEXT_USER_ITEM_KEY = "AuthenticatedUser";
}

public static class RoleName
{
    public const string ADMIN = "Admin";
    public const string CUSTOMER = "Customer";
}

public static class DefaultRoles
{
    public static readonly Guid AdminId = Guid.Parse("6B29FC40-CA47-1067-B31D-00DD010662FD");
    public static readonly Guid CustomerId = Guid.Parse("6B29FC40-CA47-1067-B31D-00DD010662FC");
}

public static class CacheKeyConstants
{
    public const string TWILIO_VERIFICATION_RESULT = "TwilioVerificationResult";
}
