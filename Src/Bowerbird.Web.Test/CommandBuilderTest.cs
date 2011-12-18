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
				
namespace Bowerbird.Web.Test
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using Moq;

    using Bowerbird.Web;
    using Bowerbird.Test.Utils;
    using Bowerbird.Core.DesignByContract;

    #endregion

    [TestFixture] public class CommandBuilderTest
    {

        #region Infrastructure

        private Mock<IServiceLocator> _mockServiceLocator;
        private CommandBuilder _commandBuilder;

        [SetUp] public void TestInitialize() 
        {
            _mockServiceLocator = new Mock<IServiceLocator>();

            _commandBuilder = new CommandBuilder(_mockServiceLocator.Object);
        }

        [TearDown] public void TestCleanup() { }

        #endregion

        #region Helpers

        private void TestCommand(){}

        #endregion

        #region Constructor tests

        [Test] public void CommandBuilder_Constructor_Passing_Null_ServiceLocator_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                        BowerbirdThrows.Exception<DesignByContractException>(
                            () => new CommandBuilder(null)));
        }

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        //[Test] public CommandBuilder_Build_Passing_Null_Input_Throws_DesignByContractException()
        //{
        //    Assert.IsTrue(
        //                BowerbirdThrows.Exception<DesignByContractException>(
        //                    () => _commandBuilder.Build<object, object>(new object(), TestCommand()) 
        //                        .
                            
        //                    )
                            
        //                    );
        //}

        
        #endregion					
				
    }
}