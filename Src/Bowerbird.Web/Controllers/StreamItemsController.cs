/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Web.Mvc;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Web.Builders;
using Bowerbird.Web.ViewModels;

namespace Bowerbird.Web.Controllers
{
    public class StreamItemsController : ControllerBase
    {
        #region Members

        private readonly IStreamItemsViewModelBuilder _viewModelBuilder;

        #endregion

        #region Constructors

        public StreamItemsController(
            IStreamItemsViewModelBuilder streamItemViewModelBuilder
            )
        {
            Check.RequireNotNull(streamItemViewModelBuilder, "streamItemQuery");

            _viewModelBuilder = streamItemViewModelBuilder;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        [HttpGet]
        public ActionResult List(StreamItemListInput listInput, StreamSortInput sortInput)
        {
            return new JsonNetResult(_viewModelBuilder.BuildStreamItems(listInput, sortInput));
        }

        #endregion
    }
}