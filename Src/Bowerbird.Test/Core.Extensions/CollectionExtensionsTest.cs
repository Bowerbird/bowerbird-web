﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System.Collections.Generic;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Test.Core.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTest
    {
        #region Infrastructure

        [SetUp]
        public void TestInitialize()
        {

        }

        [TearDown]
        public void TestCleanup()
        {
        }

        #endregion

        #region Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)] 
        public void CollectionExtensions_IsNotNullAndHasItems_Passing_Null_Collection_Returns_False()
        {
            Assert.IsFalse(
                new List<object>()
                .MakeNull<IEnumerable<object>>()
                .IsNotNullAndHasItems());
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void CollectionExtensions_IsNotNullAndHasItems_Passing_Empty_Collection_Returns_False()
        {
            Assert.IsFalse(
                new List<object>()
                .IsNotNullAndHasItems());
        }

        [Test]
        [Category(TestCategory.Unit)] 
        public void CollectionExtensions_IsNotNullAndHasItems_Passing_Collection_Having_Items_Returns_False()
        {
            Assert.IsTrue(
                new List<object>(){new object(), new object()}
                .IsNotNullAndHasItems());
        }

        #endregion					
    }
}