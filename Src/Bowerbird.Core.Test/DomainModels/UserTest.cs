/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DomainModels.Members;

namespace Bowerbird.Core.Test.DomainModels
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;
    using NUnit.Framework;
    
    using Bowerbird.Core.DesignByContract;
    using Bowerbird.Core.DomainModels;
    using Bowerbird.Core.DomainModels.DenormalisedReferences;
    using Bowerbird.Core.Extensions;
    using Bowerbird.Test.Utils;

    #endregion

    [TestFixture]
    public class UserTest
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
        public void User_Constructor_With_Null_Roles_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    null)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_Password_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeObjects.TestRoles());

            var actual = user.ValidatePassword(FakeValues.Password);
            var expected = true;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_Email_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeObjects.TestRoles());

            var expected = FakeValues.Email;
            var actual = user.Email;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_FirstName_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeObjects.TestRoles());

            var expected = FakeValues.FirstName;
            var actual = user.FirstName;

            Assert.AreEqual(actual, expected);
        }
        
        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_LastName_Field()
        {
            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeObjects.TestRoles());

            var expected = FakeValues.LastName;
            var actual = user.LastName;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Constructor_Populates_Membership_As_GlobalMembership_With_Roles_Field()
        {
            var roles = FakeObjects.TestRoles();

            var user = new User(
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                roles);

            Assert.IsTrue(user.Memberships.Count == 1);

            var expected = roles.Count();
            var actual = user.Memberships[0].Roles.Count();

            Assert.AreEqual(actual, expected);

            foreach (var role in user.Memberships[0].Roles)
            {
                var derivedFromRole = roles.Where(x => x.Id == role).FirstOrDefault();

                Assert.AreEqual(role, derivedFromRole.Id);
            }
        }

        #endregion

        #region Property tests

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Email_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(FakeObjects.TestUser().Email);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_FirstName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(FakeObjects.TestUser().FirstName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_LastName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(FakeObjects.TestUser().LastName);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Description_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(FakeObjects.TestUser().Description);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_PasswordSalt_Is_TypeOf_Guid()
        {
            Assert.IsInstanceOf<Guid>(FakeObjects.TestUser().PasswordSalt);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_HashedPassword_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(FakeObjects.TestUser().HashedPassword);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_LastLoggedIn_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(FakeObjects.TestUser().LastLoggedIn);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_ResetPasswordKey_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(FakeObjects.TestUser().ResetPasswordKey);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_FlaggedItemsOwned_Is_TypeOf_Int()
        {
            Assert.IsInstanceOf<int>(FakeObjects.TestUser().FlaggedItemsOwned);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_FlagsRaised_Is_TypeOf_Int()
        {
            Assert.IsInstanceOf<int>(FakeObjects.TestUser().FlagsRaised);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_Memeberships_Is_ListOf_DenormalisedNamedDomainModelReference_AsGeneric_Membership()
        {
            Assert.IsInstanceOf<List<DenormalisedMemberReference>>(FakeObjects.TestUser().Memberships);
        }

        #endregion

        #region Method tests

        [Test]
        [Category(TestCategory.Unit)]
        public void User_ValidPassword_WithValidPassword_Returns_True()
        {
            Assert.IsTrue(FakeObjects.TestUser().ValidatePassword(FakeValues.Password));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_ValidPassword_WithInValidPassword_Returns_False()
        {
            Assert.IsFalse(FakeObjects.TestUser().ValidatePassword(FakeValues.InvalidPassword));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateEmail_WithValidEmail_Updates_Email()
        {
            Assert.IsTrue(FakeObjects.TestUser()
                .UpdateEmail("new@email.com")
                .Email
                .Equals("new@email.com"));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateEmail_WithInValidEmail_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => FakeObjects.TestUser()
                    .UpdateEmail(FakeValues.InvalidEmail)
                ));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdatePassword_Then_Validate_With_New_Password_Returns_True()
        {
            Assert.IsTrue(
                FakeObjects.TestUser()
                .UpdatePassword("newpassword")
                .ValidatePassword("newpassword")
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdatePassword_Then_Validate_With_Old_Password_Returns_False()
        {
            Assert.IsFalse(
                FakeObjects.TestUser()
                .UpdatePassword("newpassword")
                .ValidatePassword(FakeValues.Password)
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateLastLoggedIn_Increases_LastLoggedIn()
        {
            var testUser = FakeObjects.TestUser();
            var initialLastLoggedIn = testUser.LastLoggedIn;

            Assert.IsTrue(
                testUser
                .WaitForASecond() //ensure not same clock-cycle
                .UpdateLastLoggedIn()
                .LastLoggedIn
                .IsMoreRecentThan(initialLastLoggedIn)
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_UpdateResetPasswordKey_DoesInFact_UpdateResetPasswordKey()
        {
            var testUser = FakeObjects.TestUser();

            var originalValue = testUser
                .UpdateResetPasswordKey(FakeValues.KeyString)
                .ResetPasswordKey;

            var newValue = testUser
                .UpdateResetPasswordKey(FakeValues.KeyString + "abc")
                .ResetPasswordKey;

            Assert.AreNotEqual(originalValue, newValue);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_AddMemebership_Passing_Role_Adds_Membership()
        {
            var user = FakeObjects.TestUser();

            var expected = user.Memberships.Count + 1;

            var teamMember = new TeamMember(
                                   user,
                                   new Team(user, FakeValues.Name, FakeValues.Description, FakeValues.Website),
                                   user,
                                   FakeObjects.TestRoles());

            ((IAssignableId)teamMember).SetIdTo("teammember", (new Random(System.DateTime.Now.Millisecond)).Next().ToString());

            user.AddMembership(teamMember);

            var actual = user.Memberships.Count;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_AddMemebership_With_ExistingMembership_Doesnt_Add_Membership()
        {
            var user = FakeObjects.TestUser();

            var expected = user.Memberships.Count + 1;

            var teamMember = new TeamMember(
                                   user,
                                   new Team(user, FakeValues.Name, FakeValues.Description, FakeValues.Website),
                                   user,
                                   FakeObjects.TestRoles());

            ((IAssignableId)teamMember).SetIdTo("teammember", (new Random(System.DateTime.Now.Millisecond)).Next().ToString());

            user.AddMembership(teamMember);
            user.AddMembership(teamMember);
            
            var actual = user.Memberships.Count;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_AddMembership_Passing_InvalidValid_Memebership_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => FakeObjects.TestUser()
                        .AddMembership(
                            null
                            )
                    ));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_RemoveMembership_Passing_MemberType_And_MembershipId_Removes_Membership()
        {
            var user = FakeObjects.TestUser();

            var expected = user.Memberships.Count - 1;

            user.RemoveMembership(user.Memberships[0].Type, user.Memberships[0].Id);

            var actual = user.Memberships.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_RemoveMembership_Passing_InvalidValid_MemberId_Throws_DesignByContractException()
        {
            Assert.IsTrue(BowerbirdThrows.Exception<DesignByContractException>(() => FakeObjects.TestUser().RemoveMembership("teammember", string.Empty)));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_RemoveRole_Passing_InvalidValid_MemberType_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () =>
                    FakeObjects.TestUser()
                        .RemoveMembership(
                            string.Empty,
                            FakeObjects.TestUser().Memberships[0].Id
                        )
                    ));
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_IncrementFlagsRaised_DoesInFact_ImplementFlagsRaised()
        {
            Assert.AreEqual(
                FakeObjects.TestUser()
                .IncrementFlagsRaised()
                .FlagsRaised,
                FakeObjects.TestUser().FlagsRaised + 1
                );
        }

        [Test]
        [Category(TestCategory.Unit)]
        public void User_IncrementFlaggedItemsOwned_DoesInFact_ImplementFlaggedItemsOwned()
        {
            Assert.AreEqual(
                FakeObjects.TestUser()
                .IncrementFlaggedItemsOwned()
                .FlaggedItemsOwned,
                FakeObjects.TestUser().FlaggedItemsOwned + 1
                );
        }

        #endregion
    }
}