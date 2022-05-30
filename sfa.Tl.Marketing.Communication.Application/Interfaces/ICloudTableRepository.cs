using System.Collections.Generic;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface ICloudTableRepository<T, TN>
{
    Task<int> Delete(IList<TN> entities);

    Task<int> DeleteAll();

    Task<int> DeleteByPartitionKey(string partitionKey);

    Task DeleteTable();

    Task<IList<TN>> GetAll();

    Task<int> Save(IList<T> entities);
}