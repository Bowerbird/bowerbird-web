/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Test.ProxyRepositories;
using Raven.Client;

namespace Bowerbird.Core.Test.CommandHandlers
{
    #region Namespaces

    using System.Collections.Generic;

    using NUnit.Framework;
    using Moq;

    using Bowerbird.Core.CommandHandlers;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.Repositories;
    using Bowerbird.Core.Commands;

    #endregion

    public class ProjectMemberDeleteCommandHandlerTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        private ProjectMemberDeleteCommandHandler TestProjectMemberDeleteCommandHandler(IDocumentSession documentSession)
        {
            var repository = new Repository<ProjectMember>(documentSession);
            var proxyProjectMemberRepository = new ProxyProjectMemberRepository(repository);
            return new ProjectMemberDeleteCommandHandler(proxyProjectMemberRepository);
        }

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberDeleteCommandHandler_Constructor_Passing_Null_ProjectMemberRepository_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(()=>
                    new ProjectMemberDeleteCommandHandler(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void ProjectMemberDeleteCommandHandler_Handle_Passing_Null_ProjectMemberDeleteCommand_Throws_DesignByContractException()
        {
            var projectMemberDeleteCommandHandler = new ProjectMemberDeleteCommandHandler(new Mock<IRepository<ProjectMember>>().Object);

            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(() =>
                    projectMemberDeleteCommandHandler.Handle(null)));
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ProjectMemberDeleteCommandHandler_Handle_Deletes_ProjectMember()
        {
            ProjectMember result = null;

            using (var session = _store.OpenSession())
            {
                var projectMember = FakeObjects.TestProjectMember();

                session.Store(projectMember);

                session.SaveChanges();

                var projectMemberDeleteCommandHandler = TestProjectMemberDeleteCommandHandler(session);

                projectMemberDeleteCommandHandler.Handle(new ProjectMemberDeleteCommand()
                                                              {
                                                                  DeletedByUserId = FakeValues.UserId,
                                                                  ProjectId = FakeValues.KeyString,
                                                                  UserId = FakeValues.UserId
                                                              });

                session.SaveChanges();

                result = session.Load<ProjectMember>(projectMember.Id);
            }

            Assert.IsNull(result);
        }

        #endregion 
    }
}