using System.Collections.Generic;
using Bowerbird.Test.Utils;
using NUnit.Framework;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.Test.Extensions
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
        public void CollectionExtensions_IsNotNullAndHasItems_Passing_Null_Collection_Returns_False()
        {
            Assert.IsFalse(
                new List<object>()
                .MakeNull<IEnumerable<object>>()
                .IsNotNullAndHasItems());
        }

        [Test]
        public void CollectionExtensions_IsNotNullAndHasItems_Passing_Empty_Collection_Returns_False()
        {
            Assert.IsFalse(
                new List<object>()
                .IsNotNullAndHasItems());
        }

        [Test]
        public void CollectionExtensions_IsNotNullAndHasItems_Passing_Collection_Having_Items_Returns_False()
        {
            Assert.IsTrue(
                new List<object>(){new object(), new object()}
                .IsNotNullAndHasItems());
        }

        #endregion					

    }
}