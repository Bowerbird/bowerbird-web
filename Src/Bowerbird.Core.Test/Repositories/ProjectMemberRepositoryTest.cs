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

namespace Bowerbird.Core.Test.Repositories
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Core.DomainModels;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.Repositories;

    #endregion

    [TestFixture]
    public class ProjectMemberRepositoryTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;
        private IProjectMemberRepository _repository;
        private User _user;
        private IEnumerable<Permission> _permissions;
        private IEnumerable<Role> _roles;
        private Project _project;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();

            using (var session = _store.OpenSession())
            {
                _permissions = FakeObjects.TestPermissions();

                foreach (var permission in _permissions)
                {
                    session.Store(permission);
                }

                _roles = new List<Role>() { new Role(FakeValues.KeyString, FakeValues.Name, FakeValues.Description, _permissions) };

                _user = new User(
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    _roles
                    );

                session.Store(_user);

                _project = new Project(_user, FakeValues.Name, FakeValues.Description);

                session.Store(_project);
                session.SaveChanges();
            }
        }

        [TearDown]
        public void TestCleanup()
        {
            _store.Dispose();

            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Persistance)]
        public void ProjectMemberRepository_Can_Save_And_Load_ProjectMember()
        {
            var write = new ProjectMember(
                   _user,
                   _project,
                   _user,
                   _roles
                    );

            _repository = new ProjectMemberRepository(_store.OpenSession());

            _repository.Add(write);

            _repository.SaveChanges();

            var read = _repository.Load(_project.Id, _user.Id);

            Assert.AreEqual(write, read);
        }

        #endregion
    }
}