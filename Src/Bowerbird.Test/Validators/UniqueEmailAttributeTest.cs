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

namespace Bowerbird.Test.Validators
{
    #region Namespaces

    using NUnit.Framework;
    using Raven.Client;

    using Bowerbird.Test.Utils;
    using Bowerbird.Web.Validators;

    #endregion

    [TestFixture]
    public class UniqueEmailAttributeTest
    {
        #region Test Infrastructure

        private IDocumentStore _store;

        [SetUp]
        public void TestInitialize()
        {
            _store = DocumentStoreHelper.TestDocumentStore();
        }

        [TearDown]
        public void TestCleanup()
        {
            _store = null;
        }

        #endregion

        #region Test Helpers

        #endregion

        #region Constructor tests

        #endregion

        #region Property tests

        [Test, Category(TestCategory.Integration), Category(TestCategory.Persistance)]
        public void UniqueEmailAttribute_IsValid_Passing_Unique_Email_Returns_True()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(FakeObjects.TestUser());

                session.SaveChanges();
            }

            bool result;

            using (var session = _store.OpenSession())
            {
                var uniqueEmailAttribute = new UniqueEmailAttribute()
                                         {
                                             DocumentSession = session
                                         };

                result = uniqueEmailAttribute.IsValid("this_email_is@unique.com");
            }

            Assert.IsTrue(result);
        }

        [Test, Category(TestCategory.Integration), Category(TestCategory.Persistance)]
        public void UniqueEmailAttribute_IsValid_Passing_Duplicate_Email_Returns_False()
        {
            using (var session = _store.OpenSession())
            {
                session.Store(FakeObjects.TestUser());

                session.SaveChanges();
            }

            bool result;

            using (var session = _store.OpenSession())
            {
                var uniqueEmailAttribute = new UniqueEmailAttribute()
                {
                    DocumentSession = session
                };

                result = uniqueEmailAttribute.IsValid(FakeValues.Email);
            }

            Assert.IsFalse(result);
        }

        #endregion

        #region Method tests

        #endregion
    }
}