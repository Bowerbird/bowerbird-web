/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Commands
{
    #region Namespaces

    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Bowerbird.Core.Commands;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserCreateCommandTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        public UserCreateCommand TestUserCreateCommand()
        {
            return new UserCreateCommand()
                       {
                           Description = FakeValues.Description,
                           Email = FakeValues.Email,
                           FirstName = FakeValues.FirstName,
                           LastName = FakeValues.LastName,
                           Password = FakeValues.Password,
                           Roles = new List<string>(){"Member"},
                           Username = FakeValues.UserName
                       };
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #endregion 
    }
}