using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns></returns>
        Task CreateCommunicationsTemplateAsync(CommunicationsTemplateModel templateModel);

        /// <summary>
        /// Gets the communications templates asynchronously.
        /// </summary>
        /// <returns>An IEnumerable list of communications templates.</returns>
        Task<IEnumerable<CommunicationsTemplateModel>> GetCommunicationsTemplatesAsync();

        /// <summary>
        /// Deletes the communications templateModel by identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task DeleteCommunicationsTemplateByIdAsync(int id);

        /// <summary>
        /// Edits the communications templateModel asynchronously
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="templateModel">The templateModel.</param>
        /// <returns></returns>
        Task EditCommunicationsTemplateAsync(int id, CommunicationsTemplateModel templateModel);




    }

   
}