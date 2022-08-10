using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            /*  {
                    ""contentDate"": ""2020/06/22"",
                    ""tradeSwitch"": null,
                    ""contentType"": ""productPage"",
                    ""contentId"": ""24479"",
                    ""countryId"": ""81"",
                    ""languageId"": ""1""
                }, { 
                    ""contentDate"": ""2020/06/22"",
                    ""tradeSwitch"": false,
                    ""contentType"": ""productPage"",
                    ""contentId"": ""24479"",
                    ""countryId"": ""81"",
                    ""languageId"": ""1""
                }, 
            */

            CacheBuster.Buster.Bust($@"{{
                ""target"": 3,
                ""items"": [{{ 
                    ""templateId"": null, 
                    ""explicitExpiry"": null,
                    ""contentDate"": ""{DateTime.Now.ToString("yyyy/MM/dd")}"",
                    ""tradeSwitch"": null,
                    ""contentType"": ""landingPage"",
                    ""contentId"": 1092,
                    ""countryId"": 81,
                    ""languageId"": 1
                }}]
            }}");

            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }
    }
}
