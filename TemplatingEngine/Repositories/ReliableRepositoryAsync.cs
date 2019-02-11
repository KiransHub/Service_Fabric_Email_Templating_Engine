using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using TemplatingEngine.External.Models;

namespace TemplatingEngine.Repositories
{
    internal class ReliableRepositoryAsync<TModel> : IReliableRepositoryAsync<TModel> where TModel : IModel
    {
        private readonly IReliableStateManager _stateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReliableRepositoryAsync{TModel}"/> class.
        /// </summary>
        /// <param name="stateManager">The state manager.</param>
        internal ReliableRepositoryAsync(IReliableStateManager stateManager) => _stateManager = stateManager;

        public async Task<long> Create(TModel model)
        {
            var newId = await GetItemsCount();

            return await PerformWithTransaction(async (reliableDictionary, tx) =>
            {
                await reliableDictionary.AddAsync(tx, newId, model);
                return newId;
            });
        }

        public async Task<IAsyncEnumerable<KeyValuePair<long, TModel>>> Read()
        {
            return await PerformWithTransaction(async (reliableDictionary, tx)
                 => await reliableDictionary.CreateEnumerableAsync(tx));
        }

        public async Task Update(long id, TModel model)
        {
            await PerformWithTransaction(async (reliableDictionary, tx) =>
               await Task.Run(() =>
                {
                    reliableDictionary.SetAsync(tx, id, model);
                    return model;
                }));
        }

        public async Task<ConditionalValue<TModel>> Delete(long id)
        {
            return await PerformWithTransaction(async (reliableDictionary, tx) =>
                await reliableDictionary.TryRemoveAsync(tx, id));
        }

        public async Task<long> GetItemsCount()
        {
            return await PerformWithTransaction(async (reliableDictionary, tx)
                => await reliableDictionary.GetCountAsync(tx));
        }

        /// <summary>
        /// Performs the with transaction.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="lambdaFunc">The lambda function.</param>
        /// <returns></returns>
        private async Task<TResult> PerformWithTransaction<TResult>(Func<IReliableDictionary<long, TModel>, ITransaction, Task<TResult>> lambdaFunc)
        {
            var templatingEngineReliableDictionaryName = Settings.Constants.ReliableDictionaryNames.TemplatingEngineReliableDictionaryName;

            var reliableDictionary =
                await _stateManager.GetOrAddAsync<IReliableDictionary<long, TModel>>(templatingEngineReliableDictionaryName);

            using (var tx = _stateManager.CreateTransaction())
            {
                var result = await lambdaFunc(reliableDictionary, tx);
                await tx.CommitAsync();
                return result;
            }
        }

    }
}
