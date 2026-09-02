using CacheBuster;
using CacheBuster.Models;

using Dapper;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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

			var productPageId = 46698;

			var bustResult = Buster.BustWebCache(new PassBust() { 
				host = "localhost:44338",
				items = new List<BustItem>()
				{
					new BustItem(
						templateId: null,
						explicitExpiry: null,
						contentDate: DateTime.Now,
						tradeSwitch: null,
						contentType: "productPage",
						contentId: productPageId,
						countryId: 81,
						languageId: 1
					)
				}
			});

			Console.WriteLine($"Bust Result: {bustResult}");

			//var fillStr = $@"{{
			//  ""urLs"": [
			//    ""https://flock-dev.designersguild.com/en-gb/p{productPageId}""
			//  ]
			//}}";

			//var result = Filler.Fill(fillStr);

//			var fillerJson = "";

//			using (var conn = new SqlConnection("Data Source=vm-sql;Initial Catalog=FlockDev;Integrated Security=True"))
//			{
//				try
//				{
//					var fillerJsonArray = conn.Query<string>(@"
//DECLARE @BaseURL NVARCHAR(200) = N'https://flock-dev.designersguild.com';

//	DECLARE @I INT = 1,
//			@Now DATETIME2(2),
//			@CountryID SMALLINT = 81,
//			@LanguageID TINYINT = 1,
//			@Culture NVARCHAR(50),
//			@IsTrade BIT,
//			@Date DATETIME2(3) = SYSDATETIME(),
//			@Target TINYINT = 1;

//DECLARE @FormattedDate NVARCHAR(30) = CONVERT(NVARCHAR(30), CONVERT(DATETIMEOFFSET(3), @Date), 127); -- yyyy/MM/ddTHH:mm:ss.sssZ
//DECLARE @ExplicitExpiry NVARCHAR(20) = @FormattedDate + N' 23:59:59';
//DECLARE @ForwardSlash NVARCHAR(1) = N'/';
//DECLARE @ContentType NVARCHAR(1) = N'p';

//IF OBJECT_ID('tempdb..#TomsProductPages') IS NOT NULL DROP TABLE #TomsProductPages;
//	CREATE TABLE #TomsProductPages (
//		TemplateID TINYINT, 
//		ProductPageID INT NOT NULL PRIMARY KEY, -- Clustered index speeds up FOR JSON PATH sorting
//		URLKeywords NVARCHAR(500) NULL,
//		Culture NVARCHAR(10)
//	);

//TRUNCATE TABLE #TomsProductPages;
//SET @Now = SYSDATETIME();

//-- SARGable query utilizing native indexes efficiently
//INSERT INTO #TomsProductPages (ProductPageID, URLKeywords, Culture)
//SELECT	
//	PP.ProductPageID,
//	PP.[URLKeywords],
//	CL.[DotNetCulture] AS [Culture]
//FROM dbo.ProductPage AS PP
//LEFT OUTER JOIN dbo.ProductPage_Country AS PPC ON PPC.ProductPageID = PP.ProductPageID
//LEFT OUTER JOIN dbo.[Country_Language] AS CL ON CL.[CountryID] = @CountryID AND CL.LanguageID = @LanguageID
//WHERE PP.Deleted = 0 
//	AND (PP.TradeSwitch IS NULL OR PP.TradeSwitch = @IsTrade) 
//	AND (PPC.CountryID IS NULL OR PPC.CountryID = @CountryID) 
//	-- Inlined SARGable replacement for pub.fnc_DateControl UDF
//	AND @Date >= PP.OnDate AND (PP.OffDate IS NULL OR @Date <= PP.OffDate)

//DECLARE @JSON NVARCHAR(MAX) = NULL;

//-- Singular structured multi-item JSON string generation
//SELECT @JSON = (
//	SELECT @Target AS [target],
//	JSON_QUERY((
//		SELECT	'productPage' AS [contentType],
//				ProductPageID AS [contentId],
//				@CountryID AS [countryId],
//				@LanguageID AS [languageId],
//				@FormattedDate AS [contentDate]
//		FROM #TomsProductPages
//		FOR JSON PATH
//	)) AS [items]
//	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
//);

//SELECT @JSON = N'[' + STRING_AGG(CAST(N'""' AS NVARCHAR(MAX)) + @BaseURL + @ForwardSlash + LOWER([Culture]) + @ForwardSlash + [URLKeywords] + @ForwardSlash + N'p' + CAST([ProductPageID] AS NVARCHAR(10)) + N'""', N',') + N']'
//			FROM #TomsProductPages;

//SELECT @JSON = JSON_MODIFY(N'{}', N'$.urls', JSON_QUERY(@JSON));

//SELECT @JSON
//", new { }, null, true, 30, CommandType.Text);

//					fillerJson = String.Join("", fillerJsonArray);
//				}
//				finally
//				{
//					conn.Close();
//				}
//			}

//			System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(JObject.Parse(fillerJson), Formatting.Indented));

//			var result = Filler.Fill(fillerJson);

			//Console.WriteLine($"Fill Result: {result}");

            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }
    }
}