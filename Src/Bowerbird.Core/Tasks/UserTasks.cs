using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Repositories;
using Bowerbird.Core.Entities;
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

        public bool AreCredentialsValid(string username, string password)
        {
            string id = "users/" + username;
            var user = _documentSession.Query<User>().Where(x => x.Id == id).First();

            return user != null && user.ValidatePassword(password);
        }

        public bool IsUsernameAvailable(string username)
        {
            return _documentSession.Query<User>().Where(x => x.Id == username).SingleOrDefault() == null;
        }

        public bool IsEmailAvailable(string email)
        {
            return _documentSession.Query<User>().Where(x => x.Email == email).SingleOrDefault() == null;
        }

        #endregion      
      
    }
}
