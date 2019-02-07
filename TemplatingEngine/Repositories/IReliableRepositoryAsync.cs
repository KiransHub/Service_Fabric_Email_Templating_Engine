using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemplatingEngine.Repositories
{
    internal interface IReliableRepositoryAsync<TModel>
    {
        Task<long> CreateAsync(TModel item);
        Task<TModel> Get(int id);
        Task Delete(int id);
        Task<long> GetItemsCount();
    }
}