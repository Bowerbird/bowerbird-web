using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Entities.DenormalisedReferences;
using Bowerbird.Core.Extensions;
using Bowerbird.Test.Utils;
using Moq;
using NUnit.Framework;

namespace Bowerbird.Core.Test.Entities
{
    [TestFixture]
    public class UserTest
    {

        #region Constructor tests

        [Test]
        public void User_Constructor_With_Null_Id_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new User(
                    null,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles())));
        }

        [Test]
        public void User_Constructor_With_Null_Roles_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    null)));
        }

        [Test]
        public void User_Constructor_Populates_Id_Field()
        {
            var user = new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles());

            var expected = FakeValues.KeyString.PrependWith("users/");
            var actual = user.Id;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void User_Constructor_Populates_Password_Field()
        {
            var user = new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles());


            var actual = user.ValidatePassword(FakeValues.Password);
            var expected = true;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void User_Constructor_Populates_Email_Field()
        {
            var user = new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles());

            var expected = FakeValues.Email;
            var actual = user.Email;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void User_Constructor_Populates_FirstName_Field()
        {
            var user = new User(
                                FakeValues.KeyString,
                                FakeValues.Password,
                                FakeValues.Email,
                                FakeValues.FirstName,
                                FakeValues.LastName,
                                FakeValues.Description,
                                TestRoles());

            var expected = FakeValues.FirstName;
            var actual = user.FirstName;

            Assert.AreEqual(actual, expected);
        }
        
        [Test]
        public void User_Constructor_Populates_LastName_Field()
        {
            var user = new User(
                                FakeValues.KeyString,
                                FakeValues.Password,
                                FakeValues.Email,
                                FakeValues.FirstName,
                                FakeValues.LastName,
                                FakeValues.Description,
                                TestRoles());

            var expected = FakeValues.LastName;
            var actual = user.LastName;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void User_Constructor_Populates_Description_Field()
        {
            var user = new User(
                                FakeValues.KeyString,
                                FakeValues.Password,
                                FakeValues.Email,
                                FakeValues.FirstName,
                                FakeValues.LastName,
                                FakeValues.Description,
                                TestRoles());

            var expected = FakeValues.Description;
            var actual = user.Description;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void User_Constructor_Populates_Membership_As_GlobalMembership_With_Roles_Field()
        {
            var roles = TestRoles();
            
            var user = new User(
                                FakeValues.KeyString,
                                FakeValues.Password,
                                FakeValues.Email,
                                FakeValues.FirstName,
                                FakeValues.LastName,
                                FakeValues.Description,
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
        public void User_Email_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUser().Email);
        }

        [Test]
        public void User_FirstName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUser().FirstName);
        }

        [Test]
        public void User_LastName_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUser().LastName);
        }

        [Test]
        public void User_Description_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUser().Description);
        }

        [Test]
        public void User_PasswordSalt_Is_TypeOf_Guid()
        {
            Assert.IsInstanceOf<Guid>(TestUser().PasswordSalt);
        }

        [Test]
        public void User_HashedPassword_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUser().HashedPassword);
        }

        [Test]
        public void User_LastLoggedIn_Is_TypeOf_DateTime()
        {
            Assert.IsInstanceOf<DateTime>(TestUser().LastLoggedIn);
        }

        [Test]
        public void User_ResetPasswordKey_Is_TypeOf_String()
        {
            Assert.IsInstanceOf<string>(TestUser().ResetPasswordKey);
        }

        [Test]
        public void User_FlaggedItemsOwned_Is_TypeOf_Int()
        {
            Assert.IsInstanceOf<int>(TestUser().FlaggedItemsOwned);
        }

        [Test]
        public void User_FlagsRaised_Is_TypeOf_Int()
        {
            Assert.IsInstanceOf<int>(TestUser().FlagsRaised);
        }

        [Test]
        public void User_Memeberships_Is_ListOf_DenormalisedNamedEntityReference_AsGeneric_Membership()
        {
            Assert.IsInstanceOf<List<DenormalisedMemberReference>>(TestUser().Memberships);
        }

        #endregion

        #region Method tests

        [Test]
        public void User_ValidPassword_WithValidPassword_Returns_True()
        {
            Assert.IsTrue(
                TestUser()
                .ValidatePassword(FakeValues.Password));
        }

        [Test]
        public void User_ValidPassword_WithInValidPassword_Returns_False()
        {
            Assert.IsFalse(
                TestUser()
                .ValidatePassword(FakeValues.InvalidPassword));
        }

        [Test]
        public void User_UpdateEmail_WithValidEmail_Updates_Email()
        {
            Assert.IsTrue(
                TestUser()
                .UpdateEmail("new@email.com")
                .Email
                .Equals("new@email.com"));
        }

        [Test]
        public void User_UpdateEmail_WithInValidEmail_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => TestUser()
                    .UpdateEmail(FakeValues.InvalidEmail)
                ));
        }

        [Test]
        public void User_UpdatePassword_Then_Validate_With_New_Password_Returns_True()
        {
            Assert.IsTrue(
                TestUser()
                .UpdatePassword("newpassword")
                .ValidatePassword("newpassword")
                );
        }

        [Test]
        public void User_UpdatePassword_Then_Validate_With_Old_Password_Returns_False()
        {
            Assert.IsFalse(
                TestUser()
                .UpdatePassword("newpassword")
                .ValidatePassword(FakeValues.Password)
                );
        }

        [Test]
        public void User_UpdateLastLoggedIn_Increases_LastLoggedIn()
        {
            var testUser = TestUser();
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
        public void User_UpdateResetPasswordKey_DoesInFact_UpdateResetPasswordKey()
        {
            var testUser = TestUser();

            var passwordUpdateKey = testUser
                .UpdateResetPasswordKey()
                .ResetPasswordKey;

            Assert.IsFalse(
                testUser
                .UpdateResetPasswordKey()
                .ResetPasswordKey
                .Equals(passwordUpdateKey)
                );
        }

        [Test]
        public void User_AddMemebership_Passing_Role_Adds_Membership()
        {
            var user = TestUser();

            var userMembershipCount_PriorToAddingMember = user.Memberships.Count;

            user.AddMembership(new TeamMember(
                                   TestUser(),
                                   new Mock<Team>().Object,
                                   TestUser(),
                                   TestRoles()));

            var expected = userMembershipCount_PriorToAddingMember + 1;
            var actual = user.Memberships.Count;

            Assert.AreEqual(actual, expected);

            //Assert.AreEqual(
            //    TestUser()
            //    .AddMembership(new TeamMember(
            //        TestUser(), 
            //        new Mock<Team>().Object, 
            //        TestUser(), 
            //        TestRoles())
            //        )
            //    .Memberships
            //    .Count,
            //    TestUser().Memberships.Count + 1);
        }

        [Test]
        public void User_AddMembership_Passing_InvalidValid_Memebership_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () => TestUser()
                        .AddMembership(
                            null
                            )
                    ));
        }

        [Test]
        public void User_RemoveMembership_Passing_MemberType_And_MembershipId_Removes_Membership()
        {
            Assert.AreEqual(
                TestUser()
                .RemoveMembership(
                    TestUser().Memberships[0].Type, 
                    TestUser().Memberships[0].Id)
                .Memberships
                .Count,
                TestUser().Memberships.Count - 1
                );
        }

        [Test]
        public void User_RemoveMembership_Passing_InvalidValid_MemberId_Throws_DesignByContractException()
        {
            var user = TestUser()
                .RemoveMembership(
                    "teammember", 
                    string.Empty
                );

            Assert.AreEqual(
                TestUser().Memberships.Count,
                user.Memberships.Count);
        }

        [Test]
        public void User_RemoveRole_Passing_InvalidValid_MemberType_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                BowerbirdThrows.Exception<DesignByContractException>(
                    () =>
                    TestUser()
                        .RemoveMembership(
                            string.Empty,
                            TestUser().Memberships[0].Id
                        )
                    ));
        }

        [Test]
        public void User_IncrementFlagsRaised_DoesInFact_ImplementFlagsRaised()
        {
            Assert.AreEqual(
                TestUser()
                .IncrementFlagsRaised()
                .FlagsRaised,
                TestUser().FlagsRaised + 1
                );
        }

        [Test]
        public void User_IncrementFlaggedItemsOwned_DoesInFact_ImplementFlaggedItemsOwned()
        {
            Assert.AreEqual(
                TestUser()
                .IncrementFlaggedItemsOwned()
                .FlaggedItemsOwned,
                TestUser().FlaggedItemsOwned + 1
                );
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Id: "abc"
        /// Password: "password"
        /// Email: "padil@padil.gov.au"
        /// FirstName: "first name"
        /// LastName: "last name"
        /// Description: "description"
        /// Roles: "Member"
        /// </summary>
        /// <returns></returns>
        private static User TestUser()
        {
            return new User(
                FakeValues.KeyString,
                FakeValues.Password,
                FakeValues.Email,
                FakeValues.FirstName,
                FakeValues.LastName,
                FakeValues.Description,
                TestRoles()
            )
            .UpdateLastLoggedIn()
            .UpdateResetPasswordKey()
            .IncrementFlaggedItemsOwned()
            .IncrementFlagsRaised();
        }

        private static IEnumerable<Role> TestRoles()
        {
            return new List<Role>()
            {
                new Role
                (
                    "Member",
                    "Member role",
                    "Member description",
                    TestPermissions()
                )
            };
        }

        private static IEnumerable<Permission> TestPermissions()
        {
            return new List<Permission>
            {
                new Permission("Read", "Read permission", "Read description"),
                new Permission("Write", "Write permission", "Write description")
            };

        }

        #endregion

    }
}