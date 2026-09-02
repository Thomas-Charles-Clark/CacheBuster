using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CacheBuster.Models
{
	public static class MicroserviceClient
	{
		// Single static instance to prevent socket exhaustion inside SQL Server
		private static readonly HttpClient Client;

		static MicroserviceClient()
		{
			// Set up the default handler with connection pooling for .NET 4.6.1
			var handler = new HttpClientHandler
			{
				UseProxy = false, // Speeds up SQLCLR outbound calls by skipping proxy detection
				AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
			};

			Client = new HttpClient(handler);

			// SQL Server tasks can run long, but give it a safety cap
			Client.Timeout = TimeSpan.FromMinutes(6);
		}

		/// <summary>
		/// Safely executes a synchronous POST to the microservice from SQL Server
		/// </summary>
		public static string PostSync(string url, string jsonPayload)
		{
			// CRITICAL FOR SQLCLR: Wrap the async task in a clean background thread execution.
			// This prevents the asynchronous state machine from capturing SQL Server's 
			// underlying synchronization context, avoiding thread locks.
			return Task.Run(async () =>
			{
				using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
				{
					using (var response = await Client.PostAsync(url, content).ConfigureAwait(false))
					{
						// Ensure we read the content fully before releasing the thread
						return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					}
				}
			}).GetAwaiter().GetResult(); // Cleanest way to pull exceptions out of a task synchronously
		}
	}
}
