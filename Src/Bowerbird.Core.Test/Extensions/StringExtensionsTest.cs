using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using NUnit.Framework;

namespace Bowerbird.Core.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {

        #region Infrastructure

        #endregion

        #region Helpers

        #endregion

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

        [Test]
        public void StringExtensions_IsValidEmailAddress_With_Valid_EmailAddress_Returns_True()
        {
            Assert.True("hamish@isa.dude".IsValidEmailAddress());
        }

        [Test]
        public void StringExtensions_FormatWith_Passing_Invalid_Format_String_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => "abc".FormatWith("def")));
        }

        [Test]
        public void StringExtensions_FormatWith_PassingFormatString_And_ParamString_Returns_FormattedText()
        {
            Assert.IsTrue("abc{0}ghi".FormatWith("def").Equals("abcdefghi"));
        }

        [Test]
        public void StringExtensions_AppendWith_Passing_Starting_Text_And_Appender_Text_Returns_FormattedText()
        {
            Assert.IsTrue("abc".AppendWith("def").Equals("abcdef"));
        }

        [Test]
        public void StringExtensions_PrependWith_Passing_Starting_Text_And_Prepender_Text_Returns_FormattedText()
        {
            Assert.IsTrue("def".PrependWith("abc").Equals("abcdef"));
        }

        #endregion

    }
}