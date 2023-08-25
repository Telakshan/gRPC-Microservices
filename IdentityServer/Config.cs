using IdentityServer4.Models;

namespace IdentityServer;

public class Config
{
    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "ShoppingCartClient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("rv605bzGolgN8W3Y0YysgQyoZc73xZv9".Sha256())
                },
                AllowedScopes = { "ShoppingCartAPI" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("ShoppingCartAPI", "Shopping Cart API")
            };


}
