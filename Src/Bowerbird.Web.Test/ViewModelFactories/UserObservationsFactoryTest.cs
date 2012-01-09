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

namespace Bowerbird.Web.Test.ViewModelFactories
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Moq;
    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Web.ViewModels;
    using Bowerbird.Web.ViewModelFactories;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.MediaResources;

    #endregion

    [TestFixture] 
    public class UserObservationsFactoryTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;
        private IPagedListFactory _pagedListFactory;

        [SetUp] 
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
            _pagedListFactory = new PagedListFactory();
        }

        [TearDown] 
        public void TestCleanup()
        {

        }

        #endregion

        #region Test Helpers

        private static void SaveUser(IDocumentSession session, User user)
        {
            session.Store(user);
            session.SaveChanges();
        }

        private static void SaveObservations(IDocumentSession session, User user, int count)
        {
            for (int i = 0; i < count; i++)
                session.Store(CreateFakeObservation(user, i));

            session.SaveChanges();
        }

        private static Observation CreateFakeObservation(User user, int addDays)
        {
            return new Observation(user,
                FakeValues.Title,
                FakeValues.CreatedDateTime.AddDays(addDays),
                FakeValues.Latitude,
                FakeValues.Longitude,
                FakeValues.Address,
                FakeValues.IsTrue,
                FakeValues.Category,
                new List<MediaResource>() { new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description, 100, 100) }
                );
        }

        private static User TestUser()
        {
            return new User(
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles()
                    );
        }

        private static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        private static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };

        }

        #endregion

        #region Constructor tests

        [Test, Category(TestCategory.Unit)] 
        public void UserObservationsFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new HomeIndexFactory(null,new Mock<IPagedListFactory>().Object)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void UserObservationsFactory_Constructor_Passing_Null_PagedListFactory_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new HomeIndexFactory(new Mock<IDocumentSession>().Object,null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)] 
        public void UserObservationsFactory_Make_Passing_Null_ObservationListInput_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new UserObservationsFactory(new Mock<IDocumentSession>().Object,new Mock<IPagedListFactory>().Object).Make(null)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void UserObservationsFactory_Make_Passing_ObservationListInput_Returns_PagedList_Of_Observations()
        {
            var user = TestUser();

            using (var session = _store.OpenSession())
            {
                var userObservationsFactory = new UserObservationsFactory(session, _pagedListFactory);

                SaveUser(session, user);

                var userId = user.Id;

                SaveObservations(session, user, FakeValues.PageSize);

                var observationListInput = new ObservationListInput()
                {
                    Page = FakeValues.Page,
                    PageSize = FakeValues.PageSize,
                    UserId = userId
                };

                var observations = userObservationsFactory.Make(observationListInput);

                Assert.AreEqual(observations.TotalResultCount, FakeValues.PageSize);
            }
        }

        #endregion					
    }
}