using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace AuthServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("socialNetworkApi", "Social Network API")
            };
        
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                
                //simple version
                //TODO rewrite it in expanded version for more control
                new ApiResource("socialNetworkApi")
                {
                    Scopes = { "socialNetworkApi" }
                }
            };
        
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // React client
                new Client
                {
                    ClientId = "socialNetworkReactClient",
                    ClientName = "React Client for Social Network",
                    ClientUri = "http://localhost:3000",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    
                    RequireClientSecret = false,

                    RedirectUris =
                    {                        
                        "http://localhost:3000/callback-oidc",                        
                    },

                    PostLogoutRedirectUris = { "http://localhost:3000/signout-oidc" },
                    AllowedCorsOrigins = { "http://localhost:3000" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialNetworkApi"
                    },

                    AllowAccessTokensViaBrowser = true
                }
            };
    }
}