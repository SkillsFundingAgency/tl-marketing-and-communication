using System.Collections.Generic;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface ICloudTableRepository<T>
{
    Task<int> Delete(IList<T> entities);

    Task<int> DeleteAll();

    Task<int> DeleteByPartitionKey(string partitionKey);

    Task DeleteTable();

    Task<IList<T>> GetAll();

    Task<IList<T>> GetByPartitionKey(string partitionKey);

    Task<int> Save(IList<T> entities);
}