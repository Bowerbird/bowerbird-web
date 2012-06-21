using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

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
            return new
            {
                user.Id,
                Avatar = user.Avatar,
                user.LastLoggedIn,
                Name = user.GetName()
            };
        }

        #endregion   
    }
}
