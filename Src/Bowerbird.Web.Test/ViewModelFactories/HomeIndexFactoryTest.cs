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
    using Bowerbird.Core.Entities;
    using Bowerbird.Core.Entities.MediaResources;

    #endregion

    [TestFixture] 
    public class HomeIndexFactoryTest
    {

        #region Test Infrastructure

        private IDocumentStore _store;
        private Mock<IPagedListFactory> _mockPagedListFactory;

        [SetUp] 
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
            _mockPagedListFactory = new Mock<IPagedListFactory>();
        }

        [TearDown] 
        public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        private static void SaveUser(IDocumentSession session, User user)
        {
            session.Store(user);
            session.SaveChanges();
        }

        private static void GenerateObservations(IDocumentSession session, User user, int count)
        {
            for (int i = 0; i < count; i++ )
                session.Store(CreateFakeObservation(user, i));
            
            session.SaveChanges();
        }

        private static void GeneratePosts(IDocumentSession session, User user, int count)
        {
            for (int i = 0; i < count; i++)
                session.Store(CreateFakePost(user));

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
                new List<MediaResource>(){new ImageMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description,100,100)}
                );
        }

        private static Post CreateFakePost(User user)
        {
            return new Post(user, FakeValues.Subject, FakeValues.Message);
        }

        private static User TestUser()
        {
            return new User(
                    FakeValues.KeyString,
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
        public void HomeIndexFactory_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new HomeIndexFactory(null,new Mock<IPagedListFactory>().Object)));
        }

        [Test, Category(TestCategory.Unit)] 
        public void HomeIndexFactory_Constructor_Passing_Null_PagedListFactory_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new HomeIndexFactory(new Mock<IDocumentSession>().Object,null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)] 
        public void HomeIndexFactory_Make_Passing_Null_HomeIndexInput_Throws_DesignByContractException()
        {
            using (var session = _store.OpenSession())
            {
                Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new HomeIndexFactory(session,_mockPagedListFactory.Object).Make(null)));
            }

        }

        /// <summary>
        /// Requires User, Observations and Posts to be injected into RavenDB
        /// </summary>
        [Test, Category(TestCategory.Persistance)] 
        public void HomeIndexFactory_Make_Passing_HomeIndexInput_Returns_HomeIndex()
        {
            var recordCount = 15;

            var homeIndexInput = new HomeIndexInput()
            {
                Page = FakeValues.Page,
                PageSize = FakeValues.PageSize,
                UserId = "users/abc"
            };

            var user = TestUser();

            using (var session = _store.OpenSession())
            {
                SaveUser(session, user);
                GenerateObservations(session, user, recordCount);
                GeneratePosts(session, user, recordCount);

                var homeIndexFactory = new HomeIndexFactory(session, new PagedListFactory());

                var homeIndex = homeIndexFactory.Make(homeIndexInput);

                Assert.AreEqual(homeIndex.StreamItems.PageSize, FakeValues.PageSize);
                Assert.AreEqual(homeIndex.StreamItems.PagedListItems.Count(), homeIndex.StreamItems.PageSize);
            }

        }

        #endregion					
				
    }
}