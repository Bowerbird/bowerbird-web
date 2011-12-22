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
    public class ViewModelFactoryBaseTest
    {
        #region Test Infrastructure

        private Mock<IDocumentSession> _mockDocumentSession;

        [SetUp] 
        public void TestInitialize()
        {
            _mockDocumentSession = new Mock<IDocumentSession>();
        }

        [TearDown] 
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        private class ProxyViewModelFactoryWithInputOutput<TInput,TOutput> : ViewModelFactoryBase<TInput,TOutput> where TOutput : new()
        {
            public ProxyViewModelFactoryWithInputOutput(IDocumentSession session):base(session){ }

            public override TOutput  Make(TInput input)
            {
                return new TOutput();
            }

            public new IDocumentSession DocumentSession{ get { return base.DocumentSession; } }
        }

        private class ProxyViewModelFactoryWithOutput<TOutput> : ViewModelFactoryBase<TOutput> where TOutput : new()
        {
            public ProxyViewModelFactoryWithOutput(IDocumentSession session):base(session){ }

            public override TOutput Make()
            {
 	            return new TOutput();
            }

            public new IDocumentSession DocumentSession{ get { return base.DocumentSession; } }
        }

        #endregion

        #region Constructor tests

        [Test, Category(TestCategories.Unit)] 
        public void ViewModelFactoryBase_SubClass_Having_Input_Output_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
             Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyViewModelFactoryWithInputOutput<object, object>(null)));
        }

        [Test, Category(TestCategories.Unit)] 
        public void ViewModelFactoryBase_SubClass_Having_Output_Constructor_Passing_Null_DocumentSession_Throws_DesignByContractException()
        {
             Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyViewModelFactoryWithOutput<object>(null)));
        }

        #endregion

        #region Property tests

        [Test, Category(TestCategories.Unit)]
        public void ViewModelFactoryBase_SubClass_Having_Input_And_Output_DocumentSession_Is_A_IDocumentSession()
        {
            Assert.IsInstanceOf<IDocumentSession>(new ProxyViewModelFactoryWithInputOutput<object, object>(_mockDocumentSession.Object).DocumentSession);
        }

        [Test, Category(TestCategories.Unit)]
        public void ViewModelFactoryBase_SubClass_Having_Output_DocumentSession_Is_A_IDocumentSession()
        {
            Assert.IsInstanceOf<IDocumentSession>(new ProxyViewModelFactoryWithOutput<object>(_mockDocumentSession.Object).DocumentSession);
        }

        #endregion

        #region Method tests

        #endregion					
    }
}