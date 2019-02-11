using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using TemplatingEngine.External.Models;

namespace TemplatingEngine.External.Contracts
{
    /// <summary>
    /// A public contract interface for the templateModel remoting service
    /// </summary>
    /// <seealso cref="Microsoft.ServiceFabric.Services.Remoting.IService" />
    public interface ITemplatingEngineContract
    {
        /// <summary>
        /// Creates the communications templateModel asynchronously.
        /// </summary>
        /// <param name="templateModel">The templateModel.</param>
        /// <returns>The templateId of the new item.</returns>
        Task<long> CreateTemplateAsync(CommunicationsTemplateModel templateModel);

        /// <summary>
        /// Gets the communications templates asynchronously.
        /// </summary>
        /// <returns>An IEnumerable list of communications templates.</returns>
        Task<IAsyncEnumerable<KeyValuePair<long, CommunicationsTemplateModel>>> GetTemplatesAsync();

        /// <summary>
        /// Deletes the communications templateModel by identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task DeleteTemplateById(long id);

        /// <summary>
        /// Edits the communications templateModel asynchronously
        /// </summary>
        /// <param name="templateId">The identifier.</param>
        /// <param name="templateModel">The templateModel.</param>
        /// <returns></returns>
        Task UpdateTemplate(long templateId, CommunicationsTemplateModel templateModel);




    }

   
}