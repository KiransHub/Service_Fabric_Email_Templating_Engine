using System;
using System.Collections.Generic;
using System.Fabric;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
    internal sealed class TemplatingEngine : StatefulService, ITemplatingEngineService, ITemplatingEngineContract
    {
        private ReliableRepositoryAsync<CommunicationsTemplateModel> _reliableRepository;

        public TemplatingEngine(StatefulServiceContext context)
            : base(context)
        {
            _reliableRepository = new ReliableRepositoryAsync<CommunicationsTemplateModel>(StateManager);
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
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

        }

        public Task CreateCommunicationsTemplateAsync(CommunicationsTemplateModel templateModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CommunicationsTemplateModel>> GetCommunicationsTemplatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommunicationsTemplateByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditCommunicationsTemplateAsync(int id, CommunicationsTemplateModel templateModel)
        {
            throw new NotImplementedException();
        }
    }
}
