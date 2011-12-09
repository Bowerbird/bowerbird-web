using Bowerbird.Core.Extensions;
using NUnit.Framework;

namespace Bowerbird.Core.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {

        #region Constructor tests


        #endregion

        #region Property tests


        #endregion

        #region Method tests

        [Test]
        public void StringExtensions_IsValidEmailAddress_With_Missing_at_Symbol_Returns_False()
        {
            Assert.False("hamish.is-a.dude".IsValidEmailAddress());
        }

        [Test]
        public void StringExtensions_IsValidEmailAddress_With_Missing_Pre_at_Symbol_characters_Returns_False()
        {
            Assert.False("@hamish.is-a.dude".IsValidEmailAddress());
        }

        [Test]
        public void StringExtensions_IsValidEmailAddress_With_Missing_Post_at_Symbol_characters_Returns_False()
        {
            Assert.False("hamish@dude".IsValidEmailAddress());
        }

        #endregion

        #region Helpers


        #endregion

    }
}