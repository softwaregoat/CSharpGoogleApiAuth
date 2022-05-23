﻿using Google.Apis.Auth.OAuth2;
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
    {
        [Obsolete]
        static void Main(string[] args)
        {

            AuthClient authClient = new AuthClient();

            authClient.domain = "";
            authClient.client_id = "";
            authClient.client_secret = "";
            authClient.audience = "";
            authClient.client_credentials = "";

            var customerId = "12211";

            var result = authClient.GetPeoplesByCustomerAsync(customerId).Result;

            if (result == null)
            {
                Console.WriteLine("GetPeoplesByCustomerAsync issue. Please try later");
                Console.Read();
                Environment.Exit(0);
            }

            string connetionString = @"Data Source=DESKTOP-3RMGPM9\SQLSERVER;Initial Catalog=testdb;User ID=sa;Password=xxx";

            var peoples = result.searchResults;

            foreach (var people in peoples)
            {
                string profileId = GetProperty(people, "id").ToString();

                var profile = authClient.GetPeopleAsync(customerId, profileId);

                if (profile == null)
                {
                    Console.WriteLine($"GetPeopleAsync Issue in ProfileID: {profileId}. Please try later");
                    continue;
                }

                string query = "INSERT INTO [dbo].[profile] ([profileID],[firstname],[lastname],[email],[folder]) VALUES"
                                + $"('{profileId}'"
                                + $",'{GetProperty(profile, "firstname").ToString()}'"
                                + $",'{GetProperty(profile, "lastname").ToString()}'"
                                + $",'{GetProperty(profile, "email").ToString()}'"
                                + $",'{GetProperty(profile, "folder").ToString()}')";

                Console.WriteLine(query);

                Insert2SQL(connetionString, query);
            }

            Console.Read();
        }

        static void Insert2SQL(string connetionString, string query)
        {
            using (SqlConnection connection = new SqlConnection(connetionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static object GetProperty(object target, string name)
        {
            var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }
    }
}
