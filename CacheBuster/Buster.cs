using CacheBuster.Models;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CacheBuster
{
    public static class Buster
    {
        private static string siteUsername = "CAdmin";
        private static string sitePassword = "D3s1gner5";
        private static string cacheBustingKey = "9079C890-8ADB-47C2-8D26-458C773EFEDA";

        private static readonly string[] siteEndpoints = new string[]
        {
            "https://localhost:44302/",
            "https://flock-dev.designersguild.com/",
            "https://flock-preview.designersguild.com/",
            "https://www.designersguild.com/api/cache/purge"
        };

        private static readonly string[] cacheEndpoints = new string[]
        {
            "https://localhost:44302/api/cache/purge",
            "https://flock-dev.designersguild.com/api/cache/purge",
            "https://flock-preview.designersguild.com/api/cache/purge",
            "https://www.designersguild.com/api/cache/purge"
        };

        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int Bust(string JSON)
        {
            var bustParams = JsonConvert.DeserializeObject<PassBust>(JSON);

            Console.WriteLine($"{JSON}");

            BustWebCache(bustParams);

            return 0;
        }

        public static int BustWebCache(PassBust objBust)
        {
            using (var client = new HttpClient())
            {
                Parallel.ForEach(objBust.items, (item) => 
                {
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(cacheEndpoints[objBust.target]),
                        Method = HttpMethod.Post
                    };

                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes($"{siteUsername}:{sitePassword}")
                        )
                    );

                    request.Headers.Add("cacheBustingKey", cacheBustingKey);

                    request.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

                    var response = client.SendAsync(request).Result;

#if DEBUG
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
#endif
                });
            }

            return 0;
        }
    }
}
