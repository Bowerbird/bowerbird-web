using Bowerbird.Web.ViewModels;
using Raven.Client;

namespace Bowerbird.Web.ViewModelFactories
{
    public class DefaultViewModelFactory : ViewModelFactoryBase<DefaultViewModel>
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

        public override DefaultViewModel Make()
        {
            return new DefaultViewModel();
        }

        #endregion

    }
}
