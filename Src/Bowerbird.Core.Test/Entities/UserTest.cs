using System;
using System.Collections.Generic;
using System.Linq;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Entities;
using Bowerbird.Core.Entities.DenormalisedReferences;
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
                Throws.Exception<DesignByContractException>(
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

            var expected = FakeValues.KeyString;
            var actual = user.Id;

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void User_Constructor_With_Null_Password_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.KeyString,
                    null,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles())));
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
        public void User_Constructor_With_Null_Email_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    null,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles())));
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
        public void User_Constructor_With_Null_FirstName_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    null,
                    FakeValues.LastName,
                    FakeValues.Description,
                    TestRoles())));
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
        public void User_Constructor_With_Null_LastName_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    null,
                    FakeValues.Description,
                    TestRoles())));
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
        public void User_Constructor_With_Null_Description_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                () => new User(
                    FakeValues.KeyString,
                    FakeValues.Password,
                    FakeValues.Email,
                    FakeValues.FirstName,
                    FakeValues.LastName,
                    null,
                    TestRoles())));
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
        public void User_Constructor_With_Null_TestRoles_Throws_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
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
        public void User_Constructor_Populates_Roles_Field()
        {
            var user = new User(
                                FakeValues.KeyString,
                                FakeValues.Password,
                                FakeValues.Email,
                                FakeValues.FirstName,
                                FakeValues.LastName,
                                FakeValues.Description,
                                TestRoles());

            IEnumerable<Role> roles = TestRoles();

            var expected = roles.Count();
            var actual = user.Roles.Count;

            Assert.AreEqual(actual, expected);

            foreach (var role in user.Roles)
            {
                var derivedFromRole = roles.Where(x => x.Id == role.Id).FirstOrDefault();

                Assert.AreEqual(role.Id, derivedFromRole.Id);

                Assert.AreEqual(role.Name, derivedFromRole.Name);
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
        public void User_Roles_Is_ListOf_DenormalisedNamedEntityReference_AsGeneric_Role()
        {
            Assert.IsInstanceOf<List<DenormalisedNamedEntityReference<Role>>>(TestUser().Roles);
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
                Throws.Exception<DesignByContractException>(
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
                .LastLoggedIn.IsMoreRecentThan(initialLastLoggedIn)
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
        public void User_AddRole_Passing_Role_Adds_Role()
        {
            Assert.AreEqual(
                TestUser()
                .AddRole(new Role("testRole", "Test Role", "Test Role", TestPermissions()))
                .Roles
                .Count,
                TestUser().Roles.Count + 1
                );
        }

        [Test]
        public void User_AddRole_Passing_InvalidValid_Role_DesignByContractException()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                    () => TestUser()
                        .AddRole(
                            null
                            )
                    ));
        }

        [Test]
        public void User_RemoveRole_Passing_RoleId_Removes_Role()
        {
            Assert.AreEqual(
                TestUser()
                .RemoveRole(TestUser().Roles[0].Id)
                .Roles
                .Count,
                TestUser().Roles.Count - 1
                );
        }

        [Test]
        public void User_RemoveRole_Passing_InvalidValid_RoleId_Adds_Role()
        {
            Assert.IsTrue(
                Throws.Exception<DesignByContractException>(
                    () =>
                    TestUser()
                        .RemoveRole(
                            string.Empty
                        )
                    ));
        }

        [Test]
        public void User_IncrementFlagsRaised_DoesInFact_ImplementFlagsRaised()
        {
            Assert.AreEqual(
                TestUser()
                .IncrementFlagsRaised(),
                TestUser().FlagsRaised + 1
                );
        }

        [Test]
        public void User_IncrementFlaggedItemsOwned_DoesInFact_ImplementFlaggedItemsOwned()
        {
            Assert.AreEqual(
                TestUser()
                .IncrementFlaggedItemsOwned(),
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