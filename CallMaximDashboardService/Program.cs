using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CallMaximDashboardService
{
    class Program
    {
        class Token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string userName { get; set; }
            [JsonProperty(".issued")]
            public string issued { get; set; }
            [JsonProperty(".expires")]
            public string expires { get; set; }
        }

        static void Main(string[] args)
        {
            string token = GetToken("http://localhost:50341/", "kevin@andersonplanet.com", "!Test123");
            Console.WriteLine("Token {0}", token);
            var result = CallApi("http://localhost:50341/api/image/", token);
            Console.WriteLine("Result {0}", result);
            Console.ReadLine();
        }

        static string GetToken(string url, string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "grant_type", "password" ),
                new KeyValuePair<string, string>( "username", userName ),
                new KeyValuePair<string, string> ( "Password", password )
            };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url + "Token", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }



        static string CallApi(string url, string token)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var t = JsonConvert.DeserializeObject<Token>(token);

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + t.access_token);
                }
                var response = client.GetAsync(url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
