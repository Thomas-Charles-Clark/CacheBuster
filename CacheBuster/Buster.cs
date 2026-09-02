using CacheBuster.Models;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace CacheBuster
{
	public static class Credentials
	{
		public static readonly string SiteUsername = "CAdmin";
		public static readonly string SitePassword = "TapestryFlower22";
		public static readonly string CacheBustingKey = "9079C890-8ADB-47C2-8D26-458C773EFEDA";
	}

	public static partial class Buster
	{
		private static readonly ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 4
		};

		// This is used by Flock to bust individual pieces of content
		public static int BustWebCache(PassBust objBust)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();

				client.DefaultRequestHeaders.Add("cacheBustingKey", Credentials.CacheBustingKey);

				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
					Convert.ToBase64String(
						System.Text.ASCIIEncoding.ASCII.GetBytes($"{Credentials.SiteUsername}:{Credentials.SitePassword}")
					)
				);

				client.DefaultRequestHeaders.Add("User-Agent", "DesignersGuild_Cache_Bust");

				Parallel.ForEach(objBust.items, parallelOptions, (item) =>
				{
					// Why does this even need to go through Cloudflare? Can't I just DNS lookup the domain names and route directly there?
					// Or is there a barrier between VM-SQL and VM-WEB?
					// No; VM-WEB uses IIS which does some trickery to permit multiple different sites with different bindings existing on the same server IP

					// That is achieved by using the "Host Header" header. Perform the DNS lookup, include the Host Header header, Robert will be your father's brother.

					var request = new HttpRequestMessage()
					{
						RequestUri = new Uri($"{(objBust.host.StartsWith("https://") ? "" : Framework.HttpsPrefix)}{objBust.host}{Framework.CacheBustingApiSingularSuffix}"),
						Method = HttpMethod.Post
					};

					request.Headers.Add("Host", objBust.host);

					request.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

					var response = client.SendAsync(request).Result;

#if DEBUG
					var obj = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

					Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));// JsonConvert.SerializeObject(obj, Formatting.Indented));
#endif
				});
			}

			return 0;
		}
	}

	public static partial class Buster
	{
		// This is used by SQL to bust large batches of content
		[Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		public static int Bust(string JSON)
		{
			try
			{
				// Trigger the synchronous execution
				string jsonResult = MicroserviceClient.PostSync("https://cachebuster.designersguild.com/cacheBusting", JSON);

#if DEBUG
				Console.WriteLine(jsonResult);
#else
				// Send information back to the SQL Server Messages tab for visibility
				// SqlContext.Pipe.Send("Microservice call completed successfully.");
				// SqlContext.Pipe.Send(jsonResult);
#endif

				return 0;
			}
			catch (Exception ex)
			{
#if DEBUG
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex);
#else
				// Standard SQLCLR exception bubbling
				// SqlContext.Pipe.Send("Error executing microservice call: " + ex.GetBaseException().Message);
#endif
				throw ex;
			}
		}
	}
}
