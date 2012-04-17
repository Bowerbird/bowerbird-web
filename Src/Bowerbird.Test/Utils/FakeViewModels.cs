/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Web.ViewModels;

namespace Bowerbird.Test.Utils
{
    #region Namespaces


    #endregion

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

        public static IdInput MakeIdInput()
        {
            return new IdInput()
            {
                Id = FakeValues.KeyString
            };
        }

        #endregion      
    }
}