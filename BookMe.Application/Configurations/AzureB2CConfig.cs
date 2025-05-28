namespace BookMe.Application.Configurations;

public class AzureB2CConfig
{
    public const string AZUREADB2C = "AzureAdB2C";
    public string Instance { get; set; }
    public string Domain { get; set; }
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string CallbackPath { get; set; }
    public string SignedOutCallbackPath { get; set; }
    public string SignUpSignInPolicyId { get; set; }
}
