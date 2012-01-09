using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Raven.Client;

namespace Bowerbird.Core.Tasks
{
    public class UserTasks : IUserTasks
    {

        #region Members

        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public UserTasks(
            IDocumentSession documentSession)
        {
            Check.RequireNotNull(documentSession, "documentSession");

            _documentSession = documentSession;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public bool AreCredentialsValid(string email, string password)
        {
            Check.RequireNotNullOrWhitespace(email, "email");
            Check.RequireNotNullOrWhitespace(password, "password");

            var user = _documentSession
                .Query<User>()
                .Where(x => x.Email == email)
                .FirstOrDefault();

            return user != null && user.ValidatePassword(password);
        }


        public bool IsEmailAvailable(string email)
        {
            Check.RequireNotNullOrWhitespace(email, "email");

            return _documentSession.Query<User>().Where(x => x.Email == email).SingleOrDefault() == null;
        }

        #endregion      
      
    }
}
