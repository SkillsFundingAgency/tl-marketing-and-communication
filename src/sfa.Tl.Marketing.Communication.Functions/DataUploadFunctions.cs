using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Extensions;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Functions;

public class DataUploadFunctions
{
    private readonly IBlobStorageService _blobStorageService;

    public DataUploadFunctions(
            IBlobStorageService blobStorageService = null)
    {
        _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
    }

    [Function("UploadProviderDataFile")]
    public async Task<HttpResponseData> UploadProviderData(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequestData request,
        FunctionContext functionContext)
    {
        var logger = functionContext.GetLogger("HttpFunction");

        try
        {
            //https://stackoverflow.com/questions/67813786/using-azure-function-net5-and-httprequestdata-how-to-handle-file-upload-form
            //https://github.com/Http-Multipart-Data-Parser/Http-Multipart-Data-Parser
            var parsedFormBody = MultipartFormDataParser.ParseAsync(request.Body, Encoding.UTF8);
            var file = parsedFormBody.Result.Files[0];

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (extension != ".json")
            {
                throw new ArgumentException($"Invalid file extension '{extension}'. Only .json files are allowed.");
            }

            using (var ms = new MemoryStream())
            {
                await file.Data.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                await _blobStorageService.Upload(ms,
                    TempProviderDataExtensions.TempDataBlobContainerName,
                    TempProviderDataExtensions.TempDataFileName,
                    "application/json");
            }

            var response = request.CreateResponse(HttpStatusCode.Accepted);

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
}