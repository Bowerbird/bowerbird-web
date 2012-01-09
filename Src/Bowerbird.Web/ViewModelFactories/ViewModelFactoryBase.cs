using Raven.Client;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.ViewModelFactories
{
    public abstract class ViewModelFactoryBase
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

        #endregion      
      
    }
}