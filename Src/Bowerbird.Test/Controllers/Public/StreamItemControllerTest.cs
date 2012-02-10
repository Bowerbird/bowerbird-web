/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Indexes;
using Bowerbird.Test.Utils;
using Bowerbird.Web.Config;
using Bowerbird.Web.Controllers.Public;
using Bowerbird.Web.ViewModels.Members;
using Moq;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace Bowerbird.Test.Controllers.Public
{
    [TestFixture]
    public class StreamItemControllerTest
    {
        #region Test Infrastructure

        private IDocumentStore _documentStore;
        private StreamItemController _controller;

        [SetUp]
        public void TestInitialize()
        {
            _documentStore = DocumentStoreHelper.InMemoryDocumentStore();

            _controller = new StreamItemController(
                _documentStore.OpenSession()
                );
        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        #endregion 
    }
}