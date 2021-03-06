﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Repositories
{
    public class GenericCloudTableRepository<T> : ICloudTableRepository<T>
        where T : ITableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly ILogger<GenericCloudTableRepository<T>> _logger;

        private readonly string _tableName;

        private const int TableBatchSize = 100;

        public GenericCloudTableRepository(
            CloudTableClient cloudTableClient,
            ILogger<GenericCloudTableRepository<T>> logger)
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
            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository Delete: table '{_tableName}' not found. Returning 0 results.");
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
        }

        public async Task<int> DeleteAll()
        {
            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository DeleteAll: table '{_tableName}' not found. Returning 0 results.");
                return 0;
            }

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

        public async Task<int> DeleteByPartitionKey(string partitionKey)
        {
            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository DeleteByPartitionKey: table '{_tableName}' not found. Returning 0 results.");
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
            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (cloudTable.Exists())
            {
                await cloudTable.DeleteAsync();
            }
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

            do
            {
                var queryResults = await cloudTable
                    .ExecuteQuerySegmentedAsync(
                        tableQuery,
                        continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);
            } while (continuationToken != null);

            return results;
        }

        public async Task<IList<T>> GetByPartitionKey(string partitionKey)
        {
            var results = new List<T>();

            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
                _logger.LogWarning($"GenericCloudTableRepository GetAll: table '{_tableName}' not found. Returning 0 results.");
                return results;
            }

            var tableQuery = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    partitionKey));

            var continuationToken = default(TableContinuationToken);

            do
            {
                var queryResults = await cloudTable
                    .ExecuteQuerySegmentedAsync(
                        tableQuery,
                        continuationToken);

                continuationToken = queryResults.ContinuationToken;

                results.AddRange(queryResults.Results);
            } while (continuationToken != null);

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
}