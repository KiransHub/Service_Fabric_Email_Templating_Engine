using System;
using System.Collections.Generic;
using System.Fabric;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TemplatingEngine.External.Contracts;
using TemplatingEngine.External.Models;
using TemplatingEngine.Repositories;

    
namespace TemplatingEngine.Services
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TemplatingEngineService : StatefulService, ITemplatingEngineService, ITemplatingEngineContract
    {
        private readonly IReliableRepositoryAsync<CommunicationsTemplateModel> _reliableRepository;

        public TemplatingEngineService(StatefulServiceContext context, IReliableRepositoryAsync<CommunicationsTemplateModel> reliableRepository)
            : base(context)
        {
            _reliableRepository = reliableRepository;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
            => this.CreateServiceRemotingReplicaListeners();

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var myDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
        }

        public Task<long> CreateTemplateAsync(CommunicationsTemplateModel templateModel)
            => _reliableRepository.Create(templateModel);
        
        public Task<long> GetTemplateCount() 
            => _reliableRepository.GetItemsCount();

        public Task<IAsyncEnumerable<KeyValuePair<long, CommunicationsTemplateModel>>> GetTemplatesAsync()
            => _reliableRepository.Read();

        public Task DeleteTemplateById(long templateId)
            => _reliableRepository.Delete(templateId);

        public Task UpdateTemplate(long templateId, CommunicationsTemplateModel templateModel)
            => _reliableRepository.Update(templateId, templateModel);

    }
}
