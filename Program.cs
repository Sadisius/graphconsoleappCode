using System;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using Helpers;

namespace graphconsoleapp
{
    class Program
    {
        // Add the following to the Main method to the Program class
        static void Main(string[] args)
        {
            var config = LoadAppSettings();
            if (config == null)
            {
                Console.WriteLine("Invalid appsettings.json file.");
                return;
            }
            // Add the authenticated instance of the GraphServicesClient and submit a request for the first user to the main method
            var client = GetAuthenticatedGraphClient(config);

            var graphRequest = client.Users.Request();

            var results = graphRequest.GetAsync().Result;
            foreach(var user in results)
            {
                Console.WriteLine(user.Id +": " + user.DisplayName + " <" + user.Mail + "> ");
            }
            Console.WriteLine("\nGraph Request:");
            Console.WriteLine(graphRequest.GetHttpRequestMessage().RequestUri);

        }

        // Add the following static member to the Program class
        private static GraphServiceClient _graphClient;

        // Add the following method LoadAppSettings to the Program class.
        private static IConfigurationRoot LoadAppSettings()
          {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .Build();

                if (string.IsNullOrEmpty(config["applicationId"]) ||
                    string.IsNullOrEmpty(config["applicationSecret"]) ||
                    string.IsNullOrEmpty(config["redirecttUri"]) ||
                    string.IsNullOrEmpty(config["tenantId"]))
                    {
                        return null;
                    }

                     return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                
                return null;
            }
        }

        // Add the following method CreateAuthorizationProvider to the Program class.
        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var clientId = config["applicationId"];
            var clientSecret = config["applicationSecret"];
            var redirecttUri = config["redirecttUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                                        .WithAuthority(authority)
                                        .WithRedirectUri(redirecttUri)
                                        .WithClientSecret(clientSecret)
                                        .Build();
            return new MsalAuthenticationProvider(cca, scopes.ToArray());
        }

        // Add the following method GetAuthenticatedGraphClient to the Program Class.
        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _graphClient = new GraphServiceClient(authenticationProvider);
            return _graphClient;
        }

        
    }
}
