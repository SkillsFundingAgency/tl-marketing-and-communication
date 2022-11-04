using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Repositories;

public class GenericCloudTableRepository<T> : ICloudTableRepository<T>
    where T : class, ITableEntity, new()
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly ILogger<GenericCloudTableRepository<T>> _logger;

    private readonly string _environment;
    private readonly string _tableName;

    private const int TableBatchSize = 100;

    public GenericCloudTableRepository(
        TableServiceClient tableServiceClient,
        ConfigurationOptions siteConfiguration,
        ILogger<GenericCloudTableRepository<T>> logger)
    {
        _tableServiceClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (siteConfiguration is null) throw new ArgumentNullException(nameof(siteConfiguration));
        _environment = siteConfiguration.Environment;

        _tableName = typeof(T).Name;
        const string suffix = "Entity";
        if (_tableName.EndsWith(suffix))
        {
            _tableName = _tableName.Remove(_tableName.Length - suffix.Length);
        }
    }

    public async Task<int> Delete(IList<T> entities)
    {
        try
        {
            var tableClient = _tableServiceClient.GetTableClient(_tableName);

            var responses = await BatchManipulateEntities(tableClient, entities, TableTransactionActionType.Delete).ConfigureAwait(false);

            return responses.Sum(response => response.Value.Count);
        }
        catch (RequestFailedException fex)
        {
            _logger.LogError(fex,
                "GenericCloudTableRepository DeleteAll: error for table '{_tableName}'. Returning 0 results.", _tableName);
            return 0;
        }
    }

    public async Task<int> DeleteAll()
    {
        var deleted = 0;

        try
        {
            var tableClient = _tableServiceClient.GetTableClient(_tableName);

            var entities = tableClient
                .QueryAsync<T>(
                    select: new List<string> { "PartitionKey", "RowKey" }, maxPerPage: 1000);

            await entities.AsPages()
                .ForEachAwaitAsync(async page =>
                {
                    // Since we don't know how many rows the table has and the results are ordered by PartitionKey+RowKey
                    // we'll delete each page immediately and not cache the whole table in memory
                    var responses =
                        await BatchManipulateEntities(tableClient, page.Values, TableTransactionActionType.Delete)
                            .ConfigureAwait(false);
                    deleted += responses.Sum(response => response.Value.Count);
                });
        }
        catch (RequestFailedException fex)
        {
            _logger.LogError(fex,
                "GenericCloudTableRepository DeleteAll: error for table '{_tableName}'. Returning 0 results.", _tableName);
        }

        return deleted;
    }

    public async Task<int> DeleteByPartitionKey(string partitionKey)
    {
        var deleted = 0;

        try
        {
            var tableClient = _tableServiceClient.GetTableClient(_tableName);

            var entities = tableClient
                .QueryAsync<T>(
                    filter: e => e.PartitionKey == partitionKey,
                    select: new List<string> { "PartitionKey", "RowKey" }, maxPerPage: 1000);

            await entities.AsPages()
                .ForEachAwaitAsync(async page =>
                {
                    // Since we don't know how many rows the table has and the results are ordered by PartitionKey+RowKey
                    // we'll delete each page immediately and not cache the whole table in memory
                    var responses = await BatchManipulateEntities(tableClient, page.Values, TableTransactionActionType.Delete).ConfigureAwait(false);
                    deleted += responses.Sum(response => response.Value.Count);
                });
        }
        catch (RequestFailedException fex)
        {
            _logger.LogError(fex,
                "GenericCloudTableRepository DeleteByPartitionKey: error for table '{_tableName}' partition key '{partitionKey}'. Returning 0 results.",
                _tableName, partitionKey);
        }

        return deleted;
    }

    public async Task DeleteTable()
    {
        try
        {
            var response = await _tableServiceClient.DeleteTableAsync(_tableName);

            _logger.LogInformation("Deleted table {table} returned with response {status} {reasonPhrase}.",
                _tableName, response?.Status, response?.ReasonPhrase);
        }
        catch (RequestFailedException fex)
        {
            _logger.LogError(fex,
                "GenericCloudTableRepository DeleteTable: error for table '{_tableName}'. Returning 0 results.", _tableName);
            throw;
        }
    }

    public async Task<IList<T>> GetAll()
    {
        var results = new List<T>();

        try
        {
            var tableClient = _tableServiceClient.GetTableClient(_tableName);
            if (tableClient is null)
            {
                _logger.LogWarning(
                    "GenericCloudTableRepository GetAll: table '{_tableName}' not found. Returning 0 results.",
                    _tableName);
                return results;
            }

            var queryResults = tableClient
                .QueryAsync<T>();

            //var cancellationToken = default(CancellationToken);

            await foreach (var page in queryResults.AsPages()
                          //.WithCancellation(cancellationToken)
                          )
            {
                results.AddRange(page.Values);
            }
        }
        catch (AggregateException aex)
        {
            if (_environment != "LOCAL" || aex.InnerException is not TaskCanceledException)
            {
                throw;
            }

            //Workaround to avoid displaying exceptions for local dev
            _logger.LogWarning("GenericCloudTableRepository GetAll: ignoring error '{exceptionTypeName}' for table '{_tableName}' when running in local environment. Returning 0 results.",
                aex.InnerException.GetType().Name, _tableName);
        }
        catch (RequestFailedException fex)
        {
            _logger.LogError(fex, "GenericCloudTableRepository GetAll: error for table '{_tableName}'. Returning 0 results.", _tableName);
        }

        return results;
    }

    public async Task<int> Save(IList<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            return 0;
        }

        var tableClient = _tableServiceClient.GetTableClient(_tableName);

        await tableClient.CreateIfNotExistsAsync();

        var responses =
            await BatchManipulateEntities(tableClient, entities, TableTransactionActionType.UpsertReplace)
                .ConfigureAwait(false);

        return responses.Sum(response => response.Value.Count);
    }

    //https://medium.com/medialesson/deleting-all-rows-from-azure-table-storage-as-fast-as-possible-79e03937c331
    /// <summary>
    /// Groups entities by PartitionKey into batches of max 100 for valid transactions
    /// </summary>
    /// <returns>List of Azure Responses for Transactions</returns>
    public async Task<List<Response<IReadOnlyList<Response>>>> BatchManipulateEntities<TEntity>(
            TableClient tableClient,
            IEnumerable<TEntity> entities,
            TableTransactionActionType tableTransactionActionType)
        where TEntity : class, ITableEntity, new()
    {
        var groups = entities.GroupBy(x => x.PartitionKey);
        var responses = new List<Response<IReadOnlyList<Response>>>();
        foreach (var group in groups)
        {
            var items = group.AsEnumerable();
            // ReSharper disable PossibleMultipleEnumeration - collection is replaced in loop
            while (items.Any())
            {
                var batch = items.Take(TableBatchSize);
                items = items.Skip(100);

                var actions = new List<TableTransactionAction>();
                actions.AddRange(batch.Select(e => new TableTransactionAction(tableTransactionActionType, e)));
                var response = await tableClient.SubmitTransactionAsync(actions).ConfigureAwait(false);
                responses.Add(response);
            }
            // ReSharper restore PossibleMultipleEnumeration
        }
        return responses;
    }
}