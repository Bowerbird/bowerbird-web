/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using System;
    
    using NUnit.Framework;
    using Moq;

    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;

    #endregion

    public class ProjectObservationTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private static ProjectObservation TestProjectObservation()
        {
            return new ProjectObservation(
                new Mock<User>().Object,
                DateTime.Now,
                new Mock<Project>().Object,
                new Mock<Observation>().Object
                );
        }

        #endregion

        #region Constructor tests

        #endregion

        #region Method tests

        #endregion 
    }
}