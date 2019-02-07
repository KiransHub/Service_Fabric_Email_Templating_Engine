using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using TemplatingEngine.External.Models;

//Todo: Can't get this working for in .NET Core for some weird reason. 
//[assembly: InternalsVisibleTo("TemplatingEngine.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001006d19e6629c10b6f40173d992640cb27e128cf5d76ac5e74fcdc2135615e27d8f5b5f96a91056e3409225f06cde0071e37c8064dfb65faf81c5407f8383323bf6c61c2d3c18863282c13de903c5ccf630aa6479c2f91b6babfc4cb06031b6e8b07f93966597fb99c5293b61695324b4ff63f0eb8f2da8822bea55140d217f71cc")]//[assembly: InternalsVisibleTo("TemplatingEngine.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001006d19e6629c10b6f40173d992640cb27e128cf5d76ac5e74fcdc2135615e27d8f5b5f96a91056e3409225f06cde0071e37c8064dfb65faf81c5407f8383323bf6c61c2d3c18863282c13de903c5ccf630aa6479c2f91b6babfc4cb06031b6e8b07f93966597fb99c5293b61695324b4ff63f0eb8f2da8822bea55140d217f71cc")]
[assembly: InternalsVisibleTo("TemplatingEngine.Tests")]

namespace TemplatingEngine.Repositories
{
    public class ReliableRepositoryAsync<TModel> where TModel : IModel
    {
        private readonly IReliableStateManager _stateManager;

        internal ReliableRepositoryAsync(IReliableStateManager stateManager) => _stateManager = stateManager;

        public async Task<long> CreateAsync(TModel model)
        {
            var newId = await GetItemsCount();

            return await PerformWithTransaction(async (reliableDictionary, tx) =>
            {
                await reliableDictionary.AddAsync(tx, newId, model);
                return newId;
            });
        }

        public async Task<IAsyncEnumerable<KeyValuePair<long, TModel>>> ReadAsync()
        {
            return await PerformWithTransaction(async (reliableDictionary, tx)
                 => await reliableDictionary.CreateEnumerableAsync(tx));
        }

        public async Task Update(int id, TModel model)
        {
            await PerformWithTransaction(async (reliableDictionary, tx) =>
               await Task.Run(() =>
                {
                    reliableDictionary.SetAsync(tx, id, model);
                    return model;
                }));
        }

        public async Task<ConditionalValue<TModel>> Delete(int id)
        {
            return await PerformWithTransaction(async (reliableDictionary, tx) => 
                await reliableDictionary.TryRemoveAsync(tx, id));
        }

        public async Task<long> GetItemsCount()
        {
            return await PerformWithTransaction(async (reliableDictionary, tx)
                => await reliableDictionary.GetCountAsync(tx));
        }

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
