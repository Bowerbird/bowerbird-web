/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Indexes;
using Bowerbird.Core.Paging;
using Bowerbird.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
using Bowerbird.Web.Factories;
using Bowerbird.Core.Config;
using Bowerbird.Core.Queries;

namespace Bowerbird.Web.Controllers
{
    public class StreamItemController : ControllerBase
    {
        #region Members

        private readonly IUserContext _userContext;
        private readonly IDocumentSession _documentSession;
        private readonly IStreamItemQuery _streamItemQuery;

        #endregion

        #region Constructors

        public StreamItemController(
            IUserContext userContext,
            IDocumentSession documentSession,
            IStreamItemQuery streamItemQuery)
        {
            Check.RequireNotNull(userContext, "userContext");
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(streamItemQuery, "streamItemQuery");

            _userContext = userContext;
            _documentSession = documentSession;
            _streamItemQuery = streamItemQuery;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            return new JsonNetResult(_streamItemQuery.GetStreamItems(listInput, sortInput));
        }

        #endregion
    }
}