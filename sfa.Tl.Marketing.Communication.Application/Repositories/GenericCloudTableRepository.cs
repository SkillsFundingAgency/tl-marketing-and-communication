using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;
using sfa.Tl.Marketing.Communication.Models.Entities;

namespace sfa.Tl.Marketing.Communication.Application.Repositories
{
    public class GenericCloudTableRepository<T, TKey> : ICloudTableRepository<T>
        where T : Entity<TKey>, ITableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly ILogger<GenericCloudTableRepository<T, TKey>> _logger;

        private readonly string _tableName;

        private const int TableBatchSize = 100;

        public GenericCloudTableRepository(
            CloudTableClient cloudTableClient,
            ILogger<GenericCloudTableRepository<T, TKey>> logger)
        {
            _cloudTableClient = cloudTableClient ?? throw new ArgumentNullException(nameof(cloudTableClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _tableName = typeof(T).Name;
            const string suffix = "Entity";
            if (_tableName.EndsWith(suffix))
            {
                _tableName = _tableName.Remove(_tableName.Length - suffix.Length);
            }
        }

        public async Task<int> Delete(IList<T> entities)
        {
            //Need to get delete working properly - just return with 0 results for now
            //https://www.wintellect.com/deleting-entities-in-windows-azure-table-storage/
            //https://blog.bitscry.com/2019/03/25/efficiently-deleting-rows-from-azure-table-storage/
            return 0;

            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository Delete: table '{_tableName}' not found. Returning 0 results.");
                return 0;
            }

            //https://stackoverflow.com/questions/26326413/delete-all-azure-table-records

            var deleted = 0;

            var stopwatch = Stopwatch.StartNew();

            var rowOffset = 0;

            while (rowOffset < entities.Count)
            {
                var batchOperation = new TableBatchOperation();

                // next batch
                var batchEntities = entities.Skip(rowOffset).Take(TableBatchSize).ToList();

                batchEntities.ForEach(x =>
                {
                    //Add wildcard etag
                    x.ETag = "*";
                    batchOperation.Add(TableOperation.Delete(x));
                });

                var batchResult = await cloudTable.ExecuteBatchAsync(batchOperation);
                deleted += batchResult.Count;

                rowOffset += batchEntities.Count;

                stopwatch.Stop();
                _logger.LogInformation($"Delete from '{_tableName}' batch result {batchResult.Count} entities in rowOffset in {rowOffset} batches in {stopwatch.ElapsedMilliseconds:#,###}ms.");
            }

            return deleted;
        }

        public async Task<int> DeleteAll()
        {
            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository DeleteAll: table '{_tableName}' not found. Returning 0 results.");
                return 0;
            }

            //https://stackoverflow.com/questions/26326413/delete-all-azure-table-records

            var tableQuery = new TableQuery<T>();
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

        public async Task<IList<T>> GetAll()
        {
            var results = new List<T>();

            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository GetAll: table '{_tableName}' not found. Returning 0 results.");
                return results;
            }

            var tableQuery = new TableQuery<T>();
            var continuationToken = default(TableContinuationToken);

            var stopwatch = Stopwatch.StartNew();

            do
            {
                var queryResults = await cloudTable
                    .ExecuteQuerySegmentedAsync(
                        tableQuery,
                        continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);
            } while (continuationToken != null);

            stopwatch.Stop();
            _logger.LogInformation($"GetAll from '{_tableName}' returning {results.Count} results in {stopwatch.ElapsedMilliseconds:#,###}ms.");

            return results;
        }

        public async Task<int> Save(IList<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                _logger.LogInformation($"Save to '{_tableName}' was given no entities to save.");
                return 0;
            }

            var cloudTable = _cloudTableClient.GetTableReference(_tableName);

            await cloudTable.CreateIfNotExistsAsync();

            var inserted = 0;
            var batchCount = 0;
            var stopwatch = Stopwatch.StartNew();

            var batchPartitionKey = entities.First().Id.ToString();

            var rowOffset = 0;

            while (rowOffset < entities.Count)
            {
                var batchOperation = new TableBatchOperation();

                // next batch
                var batchEntities = entities.Skip(rowOffset).Take(TableBatchSize).ToList();

                foreach (var entity in batchEntities)
                {
                    //TODO: Check if exists, then update if it has changed
                    //TODO: Add a ctor with row key? or always do this in the entity?
                    entity.RowKey = entity.Id.ToString();
                    //TODO: Do we need a partition?
                    entity.PartitionKey = batchPartitionKey;

                    //TODO: Sort out object collections
                    // https://damieng.com/blog/2015/06/27/table-per-hierarchy-in-azure-table-storage
                    // https://stackoverflow.com/questions/19885219/insert-complex-objects-to-azure-table-with-tableserviceentity
                    // https://www.devprotocol.com/azure-table-storage-and-complex-types-stored-in-json/

                    batchOperation.InsertOrReplace(entity);
                }

                var batchResult = await cloudTable.ExecuteBatchAsync(batchOperation);
                inserted += batchResult.Count;
                batchCount++;

                rowOffset += batchEntities.Count;

                _logger.LogInformation($"Save to '{_tableName}' batch result {batchResult.Count} entities in rowOffset in {rowOffset} batches in {stopwatch.ElapsedMilliseconds:#,###}ms.");
            }

            //TODO: Can do a batch insert
            //https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-java
            //Performance
            //https://stackoverflow.com/questions/17955557/painfully-slow-azure-table-insert-and-delete-batch-operations

            stopwatch.Stop();
            _logger.LogInformation($"Save to '{_tableName}' saved {inserted} entities in {batchCount} batches in {stopwatch.ElapsedMilliseconds:#,###}ms.");

            return inserted;
        }
    }
}