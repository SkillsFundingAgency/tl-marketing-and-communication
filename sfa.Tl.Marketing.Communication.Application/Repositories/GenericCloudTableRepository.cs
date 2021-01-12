using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using sfa.Tl.Marketing.Communication.Application.Entities;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Repositories
{
    public class GenericCloudTableRepository<T, TKey> : ICloudTableRepository<T>
        where T : Entity<TKey>, ITableEntity, new()
    {
        private readonly CloudTableClient _cloudTableClient;

        private readonly string _tableName;

        public GenericCloudTableRepository(CloudTableClient cloudTableClient)
        {
            _cloudTableClient = cloudTableClient ?? throw new ArgumentNullException(nameof(cloudTableClient));

            _tableName = typeof(T).Name;
            const string suffix = "Entity";
            if (_tableName.EndsWith(suffix))
            {
                _tableName = _tableName.Remove(_tableName.Length - suffix.Length);
            }
        }

        public async Task<IList<T>> GetAll()
        {
            var results = new List<T>();

            var cloudTable = _cloudTableClient.GetTableReference(_tableName);
            if (!cloudTable.Exists())
            {
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
                //results.AddRange(
                //    queryResults
                //        .Select(entity =>
                //            new Qualification
                //            {
                //                Id = entity.Id,
                //                Name = entity.Name
                //            }));

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

            //var inserted = 0;
            var batchOperation = new TableBatchOperation();
            var batchPartitionKey = entities.First().Id.ToString();

            foreach (var entity in entities)
            {
                //TODO: Check if exists, then update if it has changed

                //TODO: Add a ctor with row key? or always do this in the entity?
                entity.RowKey = entity.Id.ToString();
                //TODO: Do we need a partition?
                //Possibly use "Qualifications" and "Providers" with a single table, or partition by Provider id with locations below?
                entity.PartitionKey = batchPartitionKey;

                //var tableOperation = TableOperation.Insert(entity);
                batchOperation.InsertOrReplace(entity);
                //var tableResult = await cloudTable.ExecuteAsync(tableOperation);

                //TODO: Move to repository, _cloudTable created for that

                //TODO: Do we need to look for deleted providers or venues - will need a pass over the data to see what's in table but not in data

                //inserted++;
            }

            var batchResult = await cloudTable.ExecuteBatchAsync(batchOperation);

            //TODO: Can do a batch insert
            //https://docs.microsoft.com/en-us/azure/cosmos-db/table-storage-how-to-use-java
            //Performance
            //https://stackoverflow.com/questions/17955557/painfully-slow-azure-table-insert-and-delete-batch-operations

            //return inserted;
            return batchResult.Count;
        }
    }
}