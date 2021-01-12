using System.Collections.Generic;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces
{
    public interface ICloudTableRepository<T>
    {
        Task<IList<T>> GetAll();

        Task<int> Save(IList<T> entities);
    }
}
