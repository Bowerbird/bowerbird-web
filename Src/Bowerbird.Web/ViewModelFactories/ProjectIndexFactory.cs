/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModelFactories
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Linq;

    using Raven.Client;
    using Raven.Client.Linq;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Web.ViewModels;
    using Bowerbird.Core.DesignByContract;

    #endregion

    public class ProjectIndexFactory : ViewModelFactoryBase, IViewModelFactory<ProjectIndexInput, ProjectIndex>
    {
        #region Fields

        private readonly IPagedListFactory _pagedListFactory;

        #endregion

        #region Constructors

        public ProjectIndexFactory(
            IDocumentSession documentSession,
            IPagedListFactory pagedListFactory)
            : base(documentSession)
        {
            Check.RequireNotNull(pagedListFactory, "pagedListFactory");

            _pagedListFactory = pagedListFactory;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public ProjectIndex Make(ProjectIndexInput input)
        {
            Check.RequireNotNull(input, "input");

            return new ProjectIndex()
            {
                Project = DocumentSession.Load<Project>(input.ProjectId)
            };
        }

        #endregion
    }
}       