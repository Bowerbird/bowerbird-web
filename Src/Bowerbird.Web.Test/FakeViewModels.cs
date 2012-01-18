using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Test.Utils;
using Bowerbird.Web.ViewModels;
using Bowerbird.Web.ViewModels.Public;

namespace Bowerbird.Web.Test
{
    public static class FakeViewModels
    {
            
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Methods

        public static AccountRegisterInput MakeAccountRegisterInput()
        {
            return new AccountRegisterInput()
            {
                FirstName = FakeValues.FirstName,
                LastName = FakeValues.LastName,
                Email = FakeValues.Email,
                Password = FakeValues.Password
            };
        }

        public static AccountRegister MakeAccountRegister()
        {
            return new AccountRegister()
            {
                FirstName = FakeValues.FirstName,
                LastName = FakeValues.LastName,
                Email = FakeValues.Email
            };
        }

        #endregion      
      
    }
}
