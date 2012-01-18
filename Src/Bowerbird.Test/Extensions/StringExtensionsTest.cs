/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Test.Extensions
{
    #region Namespaces

    using NUnit.Framework;

    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture] 
    public class StringExtensionsTest
    {
        #region Test Infrastructure

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        #endregion

        #region Method tests

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_IsValidEmailAddress_With_Missing_at_Symbol_Returns_False()
        {
            Assert.False("hamish.is-a.dude".IsValidEmailAddress());
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_IsValidEmailAddress_With_Missing_Pre_at_Symbol_characters_Returns_False()
        {
            Assert.False("@hamish.is-a.dude".IsValidEmailAddress());
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_IsValidEmailAddress_With_Missing_Post_at_Symbol_characters_Returns_False()
        {
            Assert.False("hamish@dude".IsValidEmailAddress());
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_IsValidEmailAddress_With_Valid_EmailAddress_Returns_True()
        {
            Assert.True("hamish@isa.dude".IsValidEmailAddress());
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_FormatWith_Passing_Invalid_Format_String_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => "abc".FormatWith("def")));
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_FormatWith_PassingFormatString_And_ParamString_Returns_FormattedText()
        {
            Assert.IsTrue("abc{0}ghi".FormatWith("def").Equals("abcdefghi"));
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_AppendWith_Passing_Starting_Text_And_Appender_Text_Returns_FormattedText()
        {
            Assert.IsTrue("abc".AppendWith("def").Equals("abcdef"));
        }

        [Test, Category(TestCategory.Unit)] 
        public void StringExtensions_PrependWith_Passing_Starting_Text_And_Prepender_Text_Returns_FormattedText()
        {
            Assert.IsTrue("def".PrependWith("abc").Equals("abcdef"));
        }

        #endregion
    }
}