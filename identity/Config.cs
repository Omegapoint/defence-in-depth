// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(
                    name: "email",
                    userClaims: new[] {"email"},
                    displayName: "Your email data")
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("products.read"),
                new ApiScope("products.write"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("products.api") { Scopes = { "products.read", "products.write" } }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedScopes = { "products.read", "products.write" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", "offline_access", "products.read", "products.write" }
                },
            };
    }
}