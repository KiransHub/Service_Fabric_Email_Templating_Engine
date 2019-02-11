using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using TemplatingEngine.External.Models;

namespace TemplatingEngine.Repositories
{

    public interface IReliableRepositoryAsync<TModel> where TModel : IModel
    {
        /// <summary>
        /// Asynchronously creates a model inside the reliable dictionary. 
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Task<long> Create(TModel model);
        /// <summary>
        /// Asynchronously deletes a model from the reliable dictionary. 
        /// </summary>
        /// <returns></returns>
        Task<IAsyncEnumerable<KeyValuePair<long, TModel>>> Read();
        /// <summary>
        /// Asynchronously updates a model inside the reliable dictionary. 
        /// </summary>
        /// <param name="templateId">The identifier.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Task Update(long templateId, TModel model);
        /// <summary>
        /// Asynchronously deletes a model from the reliable dictionary. 
        /// </summary>
        /// <param name="templateId">The identifier.</param>
        /// <returns></returns>
        Task<ConditionalValue<TModel>> Delete(long templateId);
        /// <summary>
        /// Gets the number of items inside the reliable dictionary.
        /// </summary>
        /// <returns></returns>
        Task<long> GetItemsCount();
    }
}   