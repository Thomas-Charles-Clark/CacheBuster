using CacheBuster.Models;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CacheBuster
{
	public static class Filler
	{
		[Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		public static int Fill(string JSON)
		{
			try
			{
				// Trigger the synchronous execution
#if DEBUG
				string jsonResult = MicroserviceClient.PostSync("https://localhost:7254/cacheFilling", JSON);
#else
				string jsonResult = MicroserviceClient.PostSync("https://cachebuster.designersguild.com/cacheFilling", JSON);
#endif
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
