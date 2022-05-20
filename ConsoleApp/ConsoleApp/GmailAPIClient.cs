using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class GmailAPIClient
    {
        public string[] Scopes = { GmailService.Scope.GmailReadonly };
        public string ApplicationName = "Gmail API .NET Quickstart";
        public string credentials_file = "credentials.json";
        public string token_file = "C:/token.json";

        GmailService service;

        [Obsolete]
        public GmailAPIClient(string[] Scopes, string ApplicationName, string credentials_file, string token_file)
        {
            UserCredential credential;
            using (var stream =
                new FileStream(credentials_file, FileMode.Open, FileAccess.Read))
            {
                string credPath = token_file;
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void GetMessages()
        {
            var request = service.Users.Messages.List("me");
            var messages = request.Execute().Messages;
            Console.WriteLine("Messages:");
            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    try
                    {
                        var msg_id = message.Id;
                        Console.WriteLine("================={0}", msg_id);
                        var msg_request = service.Users.Messages.Get("me", msg_id);
                        var result = msg_request.Execute();

                        var msg_subject = result.Payload.Headers.First(em => em.Name == "Subject").Value;
                        var msg_body = result.Payload.Body.Data;
                        var msg_from = result.Payload.Headers.First(em => em.Name == "From").Value;
                        var msg_to = result.Payload.Headers.First(em => em.Name == "To").Value;

                        Console.WriteLine("Subject: {0}", msg_subject);
                        Console.WriteLine("Body: {0}", msg_body);
                        Console.WriteLine("From: {0}", msg_from);
                        Console.WriteLine("To: {0}", msg_to);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("No Messages found.");
            }
        }
    }
}
