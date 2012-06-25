using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Extensions;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Factories
{
    public class UserViewFactory : IUserViewFactory
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public object Make(User user)
        {
            Check.RequireNotNull(user, "user");

            var userId = user.Id.MinifyId<User>();

            return new
            {
                Id = userId,
                Avatar = user.Avatar,
                user.LastLoggedIn,
                Name = user.GetName(),
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        #endregion   
    }
}
