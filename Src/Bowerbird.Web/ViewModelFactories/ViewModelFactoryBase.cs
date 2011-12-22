using Raven.Client;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.ViewModelFactories
{
    public abstract class ViewModelFactoryBase<TInput, TViewModel> : IViewModelFactory<TInput, TViewModel>
    {

        #region Members

        #endregion

        #region Constructors

        protected ViewModelFactoryBase(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            DocumentSession = documentSession;
        }

        #endregion

        #region Properties

        protected IDocumentSession DocumentSession { get; private set; } 

        #endregion

        #region Methods

        public abstract TViewModel Make(TInput input);

        #endregion      
      
    }

    public abstract class ViewModelFactoryBase<TViewModel> : IViewModelFactory<TViewModel>
    {

        #region Members

        #endregion

        #region Constructors

        protected ViewModelFactoryBase(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            DocumentSession = documentSession;
        }

        #endregion

        #region Properties

        protected IDocumentSession DocumentSession { get; private set; }

        #endregion

        #region Methods

        public abstract TViewModel Make();

        #endregion

    }
}