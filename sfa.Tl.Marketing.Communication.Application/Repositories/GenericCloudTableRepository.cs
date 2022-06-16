using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Configuration;

namespace sfa.Tl.Marketing.Communication.Application.Repositories;

public class GenericCloudTableRepository<T, TN> : ICloudTableRepository<T, TN>
    where T : Microsoft.Azure.Cosmos.Table.ITableEntity, new()
    where TN : class, Azure.Data.Tables.ITableEntity,
    new()
{
    private readonly CloudTableClient _cloudTableClient;
    private readonly TableServiceClient _tableServiceClient;
    private readonly ILogger<GenericCloudTableRepository<T, TN>> _logger;

    private readonly string _environment;
    private readonly string _tableName;

    private const int TableBatchSize = 100;

    public GenericCloudTableRepository(
        CloudTableClient cloudTableClient,
        TableServiceClient tableServiceClient,
        ConfigurationOptions siteConfiguration,
        ILogger<GenericCloudTableRepository<T, TN>> logger)
    {
        _cloudTableClient = cloudTableClient ?? throw new ArgumentNullException(nameof(cloudTableClient));
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

    public async Task<int> Delete(IList<TN> entities)
    {
        var tableClient = _tableServiceClient.GetTableClient(_tableName);

        var deleteEntitiesBatch = entities
            .Select(entityToDelete => 
                new TableTransactionAction(TableTransactionActionType.Delete, entityToDelete))
            .ToList();
        //foreach (var entityToDelete in entities)
        //{
        //    deleteEntitiesBatch.Add(new TableTransactionAction(TableTransactionActionType.Delete, entityToDelete));
        //}

        // Submit the batch.
        var response = await tableClient.SubmitTransactionAsync(deleteEntitiesBatch).ConfigureAwait(false);

        return response.Value.Count;
        /*
        var cloudTable = _cloudTableClient.GetTableReference(_tableName);
        if (!cloudTable.Exists())
        {
            _logger.LogWarning("GenericCloudTableRepository Delete: table '{_tableName}' not found. Returning 0 results.", _tableName);
            return 0;
        }

        var deleted = 0;
        var rowOffset = 0;

        while (rowOffset < entities.Count)
        {
            var batchOperation = new TableBatchOperation();

            var batchEntities = entities.Skip(rowOffset).Take(TableBatchSize).ToList();

            batchEntities.ForEach(x =>
            {
                x.ETag = "*"; //Add wildcard etag
                batchOperation.Add(TableOperation.Delete(x));
            });

            var batchResult = await cloudTable.ExecuteBatchAsync(batchOperation);
            deleted += batchResult.Count;

            rowOffset += batchEntities.Count;
        }

        return deleted;
        */
    }

    public async Task<int> DeleteAll()
    {
        var tableClient = _tableServiceClient.GetTableClient(_tableName);

        var deleted = 0;
        var deleteEntitiesBatch = new List<TableTransactionAction>();

        var queryResults = tableClient
            .QueryAsync<TN>();

        //var cancellationToken = default(CancellationToken);

        await foreach (var page in queryResults.AsPages()
                      //.WithCancellation(cancellationToken)
                      )
        {
            deleteEntitiesBatch
                .AddRange(page.Values
                    .Select(x =>
                        new TableTransactionAction(TableTransactionActionType.Delete, x)));

            var response = await tableClient.SubmitTransactionAsync(deleteEntitiesBatch).ConfigureAwait(false);
            deleted += response.Value.Count;
        }

        /*
        do
        {
            var queryResults = await cloudTable
                .ExecuteQuerySegmentedAsync(
                    tableQuery,
                    continuationToken);

            continuationToken = queryResults.ContinuationToken;

            // Split into chunks of 100 for batching
            var rowsChunked = queryResults.Select((x, index) => new { Index = index, Value = x })
                .Where(x => x.Value != null)
                .GroupBy(x => x.Index / TableBatchSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            // Delete each chunk of 100 in a batch
            foreach (var rows in rowsChunked)
            {
                var batchOperation = new TableBatchOperation();
                rows.ForEach(x => batchOperation.Add(TableOperation.Delete(x)));

                await cloudTable.ExecuteBatchAsync(batchOperation);
            }

            deleted += queryResults.Count();

        } while (continuationToken != null);

        */
        return deleted;
    }

    public async Task<int> DeleteByPartitionKey(string partitionKey)
    {
        var cloudTable = _cloudTableClient.GetTableReference(_tableName);
        if (!cloudTable.Exists())
        {
            _logger.LogWarning("GenericCloudTableRepository DeleteByPartitionKey: table '{_tableName}' not found. Returning 0 results.", _tableName);
            return 0;
        }

        var tableQuery = new TableQuery<T>()
            .Where(TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                partitionKey));

        var continuationToken = default(TableContinuationToken);
        var deleted = 0;

        do
        {
            var queryResults = await cloudTable
                .ExecuteQuerySegmentedAsync(
                    tableQuery,
                    continuationToken);

            continuationToken = queryResults.ContinuationToken;

            // Split into chunks of 100 for batching
            var rowsChunked = queryResults.Select((x, index) => new { Index = index, Value = x })
                .Where(x => x.Value != null)
                .GroupBy(x => x.Index / TableBatchSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            // Delete each chunk of 100 in a batch
            foreach (var rows in rowsChunked)
            {
                var batchOperation = new TableBatchOperation();
                rows.ForEach(x => batchOperation.Add(TableOperation.Delete(x)));

                await cloudTable.ExecuteBatchAsync(batchOperation);
            }

            deleted += queryResults.Count();

        } while (continuationToken != null);

        return deleted;
    }

    public async Task DeleteTable()
    {
        var response = await _tableServiceClient.DeleteTableAsync(_tableName);

        //var tableClient = _tableServiceClient.GetTableClient(_tableName);
        //var response = await tableClient.DeleteAsync();
        _logger.LogInformation("Deleted table {table} returned with response {status} {reasonPhrase}.",
            _tableName, response?.Status, response?.ReasonPhrase);
    }

    public async Task<IList<TN>> GetAll()
    {
        var results = new List<TN>();

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

            //await tableClient.CreateIfNotExistsAsync();

            var queryResults = tableClient
                .QueryAsync<TN>();

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

        var cloudTable = _cloudTableClient.GetTableReference(_tableName);

        await cloudTable.CreateIfNotExistsAsync();

        //var tableClient = _tableServiceClient.GetTableClient(_tableName);
        //await tableClient.CreateIfNotExistsAsync();

        var inserted = 0;

        var rowOffset = 0;

        while (rowOffset < entities.Count)
        {
            var batchOperation = new TableBatchOperation();

            var batchEntities = entities.Skip(rowOffset).Take(TableBatchSize).ToList();

            foreach (var entity in batchEntities)
            {
                batchOperation.InsertOrReplace(entity);
            }

            var batchResult = await cloudTable.ExecuteBatchAsync(batchOperation);
            inserted += batchResult.Count;

            rowOffset += batchEntities.Count;
        }

        return inserted;
    }
}