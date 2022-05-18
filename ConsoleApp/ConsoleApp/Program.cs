using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
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
    class Program
    {// If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/gmail-dotnet-quickstart.json
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API .NET Quickstart";

        [Obsolete]
        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            var request = service.Users.Messages.List("me");

            // List labels.
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


                        string connetionString = @"Data Source=DESKTOP-3RMGPM9\SQLSERVER;Initial Catalog=testdb;User ID=sa;Password=xxx";

                        using (SqlConnection connection = new SqlConnection(connetionString))
                        {
                            SqlCommand command = new SqlCommand("INSERT INTO [dbo].[msg] ([msg_id],[msg_subject],[msg_body],[msg_from],[msg_to]) VALUES"
                                + $"('{msg_id}'"
                                + $",'{msg_subject}'"
                                + $",'{msg_body}'"
                                + $",'{msg_from}'"
                                + $",'{msg_to}')"
           , connection);
                            command.Connection.Open();
                            command.ExecuteNonQuery();
                        }

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
            Console.Read();
        }
    }
}
