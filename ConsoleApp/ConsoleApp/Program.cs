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
            string connetionString = @"Data Source=DESKTOP-3RMGPM9\SQLSERVER;Initial Catalog=testdb;User ID=sa;Password=xxx";

            AuthClient authClient = new AuthClient();

            var customerId = "1221";
            var peoples = authClient.GetPeoplesByCustomerAsync(customerId).Result;
            foreach (var people in peoples)
            {
                var profileID = people.id;
                var profile = authClient.GetPeopleAsync(customerId, profileID.ToString());

                string query = "INSERT INTO [dbo].[profile] ([profileID],[PersonID],[JobID],[CompanyID],[Note]) VALUES"
                                + $"('{profileID}'"
                                + $",'{profile.PersonID}'"
                                + $",'{profile.JobID}'"
                                + $",'{profile.CompanyID}'"
                                + $",'{profile.Note}')";

                Insert2SQL(connetionString, query);
            }

        }

        static void Insert2SQL(string connetionString, string query)
        {

            using (SqlConnection connection = new SqlConnection(connetionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
            Console.Read();
        }
    }
}