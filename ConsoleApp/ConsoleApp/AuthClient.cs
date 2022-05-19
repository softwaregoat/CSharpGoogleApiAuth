using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class AuthClient
    {
        public string domain { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string audience { get; set; }
        public string client_credentials { get; set; }
        private DateTime accessTokenExpiry { get; set; }

        public AuthClient()
        {
            _ = GetTokenAsync();
        }

        public async Task<string> GetTokenAsync()
        {
            var expiryDate = new DateTime();
            if (accessTokenExpiry > expiryDate)
            {
                return null;
            }

            var url = $"https://{domain}.auth0.com/oauth/token";

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            dynamic json_body = new ExpandoObject();
            json_body.client_id = client_id;
            json_body.client_secret = client_secret;
            json_body.audience = audience;
            json_body.grant_type = client_credentials;

            string json = JsonConvert.SerializeObject(json_body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");


            var response = await client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();

            dynamic response_json = JsonConvert.DeserializeObject(result);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                accessTokenExpiry = expiryDate.AddSeconds(response_json.expires_in);
                return response_json.access_token;
            }
            return null;
        }

        public async Task<dynamic> GetPeopleByCustomerAsync(string customerId)
        {
            var url = "https://api.icims.com/customers/" + customerId +"/search/people";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            client.DefaultRequestHeaders.Add("Autorization", $"Bearer {GetTokenAsync()}");

            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            dynamic response_json = JsonConvert.DeserializeObject(result);
            return response_json; // {"searchResults":[{"self":"https://api.icims.com/customers/1221/people/1", "id":1}]}
        }

        public async Task<dynamic> GetPeopleAsync(string customerId, string profileId)
        {
            var url = "https://api.icims.com/customers/" + customerId + "/people/" + profileId;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            client.DefaultRequestHeaders.Add("Autorization", $"Bearer {GetTokenAsync()}");

            var response = await client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            dynamic response_json = JsonConvert.DeserializeObject(result);
            return response_json; 
        }

    }
}
