using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModels.Shared;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class DefaultViewModelFactory : ViewModelFactoryBase, IViewModelFactory<DefaultViewModel>
    {

        #region Members

        #endregion

        #region Constructors

        public DefaultViewModelFactory(
            IDocumentSession documentSession)
            : base(documentSession)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public DefaultViewModel Make()
        {
            return new DefaultViewModel();
        }

        #endregion

    }
}
