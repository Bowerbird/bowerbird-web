using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Repositories;

namespace Bowerbird.Core.Test.ProxyRepositories
{
    public class ProxyUserRepository : ProxyRepository<User>
    {

        #region Members

        #endregion

        #region Constructors

        public ProxyUserRepository(IRepository<User> repository)
            : base(repository)
        {
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public User LoadByEmail(string email)
        {
            return _repository.LoadByEmail(email);
        }

        #endregion

    }
}
