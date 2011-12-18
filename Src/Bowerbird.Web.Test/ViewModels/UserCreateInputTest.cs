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
    public class UserCreateInputTest
    {
        #region Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test]public void UserCreatedInput_Username_Is_A_String()
        {
            Assert.IsInstanceOf<string>(new UserCreateInput() { Username = FakeValues.UserName }.Username );
        }

        #endregion

        #region Method tests

        #endregion

    }
}