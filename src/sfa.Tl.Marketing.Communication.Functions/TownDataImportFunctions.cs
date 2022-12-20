using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions;

public class TownDataImportFunctions
{
    private readonly ITownDataService _townDataService; 
    private readonly ITableStorageService _tableStorageService;

    public TownDataImportFunctions
    (
        ITownDataService townDataService,
        ITableStorageService tableStorageService)
    {
        _townDataService = townDataService ?? throw new ArgumentNullException(nameof(townDataService));
        _tableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));
    }

    [Function("ImportTowns")]
    public async Task<HttpResponseData> ImportTowns(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            var resultsCount = await _townDataService.ImportTowns();

            logger.LogInformation("Town data import saved {resultsCount} towns.", resultsCount);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/text");

            await response.WriteStringAsync(
                $"Saved {resultsCount} towns.");

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error importing town data. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("UploadIndexOfPlaceNames")]
    public async Task<HttpResponseData> UploadData(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            var parsedFormBody = MultipartFormDataParser.ParseAsync(request.Body, Encoding.UTF8);
            var file = parsedFormBody.Result.Files[0];

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (extension != ".csv")
            {
                throw new ArgumentException($"Invalid file extension '{extension}'. Only .csv files are allowed.");
            }

            using var ms = new MemoryStream();
            await file.Data.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var count = await _townDataService.ImportTownsFromCsvStream(ms);

            logger.LogInformation("Town data upload saved {resultsCount} towns.", count);

            var response = request.CreateResponse(HttpStatusCode.Accepted);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync($"{{ \"saved\": {count} }}");

            return response;
        }
        catch (ArgumentException aex)
        {
            var response = request.CreateResponse(HttpStatusCode.BadRequest);
            response.Headers.Add("Content-Type", "application/text");
            await response.WriteStringAsync(aex.Message);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error reading or processing uploaded data. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetTowns")]
    public async Task<HttpResponseData> GetTowns(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            logger.LogInformation("GetTowns function was called.");

            var towns =
                (await _tableStorageService.GetAllTowns())
                .OrderBy(t => t.Name)
                .ToList();

            logger.LogInformation("GetTowns returned {townsCount} records.", towns.Count);

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var json = JsonSerializer.Serialize(towns,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await response.WriteStringAsync(json);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError("Error in GetTowns. Internal Error Message {e}", e);

            return request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}