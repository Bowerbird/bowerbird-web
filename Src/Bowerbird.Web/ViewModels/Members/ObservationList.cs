
using Bowerbird.Core.DomainModels;
using Bowerbird.Web.ViewModels.Shared;

namespace Bowerbird.Web.ViewModels.Members
{
    public class ObservationList : IViewModel
    {

        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string UserId { get; set; }

        public string ProjectId { get; set; }

        public string TeamId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public PagedList<Observation> Observations { get; set; }

        #endregion

        #region Methods

        #endregion      
      
    }
}
