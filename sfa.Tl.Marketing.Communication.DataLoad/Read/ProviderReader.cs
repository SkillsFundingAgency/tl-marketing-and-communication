using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace sfa.Tl.Marketing.Communication.DataLoad.Read
{
    internal class ProviderReader
    {
        private const string FailedToImportMessage = "Failed to load CSV file. Please check the format.";

        internal ProviderReadResult ReadData(string csvFilePath)
        {
            var providerLoadResult = new ProviderReadResult();

            using (var fileStream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                try
                {
                    csv.Configuration.RegisterClassMap<ProviderReadDataMap>();
                    var records = csv.GetRecords<ProviderReadData>().ToList();
                    providerLoadResult.Providers = records;
                }
                catch (ReaderException re)
                {
                    providerLoadResult.Error = $"{FailedToImportMessage} {re.Message} {re.InnerException?.Message}";
                }
                catch (ValidationException ve)
                {
                    providerLoadResult.Error = $"{FailedToImportMessage} {ve.Message} {ve.InnerException?.Message}";
                }
                catch (BadDataException bde)
                {
                    providerLoadResult.Error = $"{FailedToImportMessage} {bde.Message} {bde.InnerException?.Message}";
                }
            }

            return providerLoadResult;
        }
    }
}