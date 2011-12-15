using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Bowerbird.Web.Extensions;
using Bowerbird.Test.Utils;
using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Web.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {

        [Test]
        public void StringExtensions_FormatWith_OneString_Having_Formattable_String_And_Passing_FormatText_Returns_StringFormattedWithText()
        { 
            Assert.IsTrue("abc{0}ghi".FormatWith("def").Equals("abcdefghi"));
        }

        [Test]
        public void StringExtensions_FormatWith_OneString_Having_UnFormattable_String_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => "abc".FormatWith("def")));
        }

        [Test]
        public void StringExtensions_FormatWith_TwoStrings_Having_Formattable_String_And_Passing_FormatText_Returns_StringFormattedWithText()
        {
            Assert.IsTrue(
                "abc{0}ghi{1}"
                .FormatWith("def", "jkl")
                .Equals("abcdefghijkl"));
        }

        [Test]
        public void StringExtensions_FormatWith_TwoStrings_Having_UnFormattable_String_And_Passing_FormatText_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => "abc".FormatWith(string.Empty, string.Empty)));
        }

        [Test]
        public void StringExtensions_FormatWith_ThreeStrings_Having_Formattable_String_And_Passing_FormatText_Returns_StringFormattedWithText()
        {
            Assert.IsTrue(
                "abc{0}ghi{1}mno{2}"
                .FormatWith("def", "jkl", "pqr")
                .Equals("abcdefghijklmnopqr"));
        }

        [Test]
        public void StringExtensions_FormatWith_ThreeStrings_Having_UnFormattable_String_And_Passing_FormatText_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => "abc".FormatWith(string.Empty, string.Empty, string.Empty)));
        }

        [Test]
        public void StringExtensions_ToTokenizedCollection_Passing_String_Having_CommaDelimitedText_Returns_StringArray_Of_Tokens()
        {
            Assert.IsTrue(
                    "abc,def,ghi,jkl"
                    .ToTokenizedCollection()
                    .Count() == 4
                );
        }

        [Test]
        public void StringExtensions_ToTokenizedCollection_Passing_Null_Throws_DesignByContractException()
        {
            string nullCollection = null;

            Assert.IsTrue(
                    BowerbirdThrows.Exception<DesignByContractException>(
                        () => nullCollection.ToTokenizedCollection()));
        }

    }
}