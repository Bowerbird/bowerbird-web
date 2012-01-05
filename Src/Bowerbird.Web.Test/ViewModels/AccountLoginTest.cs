/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
namespace Bowerbird.Web.Test.ViewModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class AccountLoginTest
    {
	    #region Test Infrastructure

	    [SetUp] 
        public void TestInitialize() { }

        [TearDown] 
        public void TestCleanup() { }
					
	    #endregion
					
	    #region Test Helpers
					
	    #endregion
					
	    #region Constructor tests
					
	    #endregion

	    #region Property tests

        [Test, Category(TestCategory.Unit)] 
        public void AccountLogin_Username_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new AccountLogin() { Username = FakeValues.UserName }.Username);
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountLogin_RememberMe_Is_A_Bool()
        {
            Assert.IsInstanceOf<bool>(new AccountLogin() { RememberMe = FakeValues.IsTrue }.RememberMe);
        }

        [Test, Category(TestCategory.Unit)] 
        public void AccountLogin_ReturnUrl_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new AccountLogin() { ReturnUrl = FakeValues.Website }.ReturnUrl);
        }

	    #endregion

	    #region Method tests
					
	    #endregion					
    }
}