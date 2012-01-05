/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Core.Test.Entities
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    using Bowerbird.Core.Entities;
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Test.Utils;

    #endregion

    public class MediaResourceTest
    {
        #region Test Infrastructure

        [SetUp]
        public void TestInitialize() { }

        [TearDown]
        public void TestCleanup() { }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_Constructor_Passing_Empty_FileName_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyObjects.ProxyMediaResource(string.Empty,FakeValues.FileFormat,FakeValues.Description)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_Constructor_Passing_Empty_FileFormat_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyObjects.ProxyMediaResource(FakeValues.Filename, string.Empty, FakeValues.Description)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_Constructor_Passing_Empty_Description_DoesNot_Throw_DesignByContractException()
        {
            Assert.IsFalse(BowerbirdThrows.Exception<DesignByContractException>(() => new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, string.Empty)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_Constructor_Populates_Property_Values()
        {
            var testMediaResource = new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description);

            Assert.AreEqual(testMediaResource.OriginalFileName, FakeValues.Filename);
            Assert.AreEqual(testMediaResource.FileFormat, FakeValues.FileFormat);
            Assert.AreEqual(testMediaResource.Description, FakeValues.Description);
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_OriginalFileName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description).OriginalFileName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_FileFormat_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description).FileFormat);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void MediaResource_Description_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(new ProxyObjects.ProxyMediaResource(FakeValues.Filename, FakeValues.FileFormat, FakeValues.Description).Description);
        }

        #endregion

        #region Method tests

        #endregion
    }
}