using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DesignByContract;
using System.Security.Cryptography;
using Bowerbird.Core.Events;
using Bowerbird.Core.Entities.DenormalisedReferences;

namespace Bowerbird.Core.Entities
{
    public class User : Entity
    {

        #region Members

        private const string _constantSalt = "nf@hskdhI&%dynm^&%";

        #endregion

        #region Constructors

        protected User() 
            : base() 
        {
            InitMembers();
        }

        public User(
            string id,
            string password,
            string email,
            string firstName, 
            string lastName,
            string description,
            IEnumerable<Role> roles) 
            : this() 
        {
            Check.RequireNotNull(roles, "roles");

            Id = "users/" + id;
            Email = email;
            PasswordSalt = Guid.NewGuid();
            HashedPassword = GetHashedPassword(password);
            LastLoggedIn = DateTime.Now;

            SetDetails(
                firstName,
                lastName,
                description);

            Roles = roles.Select(x => (DenormalisedNamedEntityReference<Role>)x).ToList();

            EventProcessor.Raise(new EntityCreatedEvent<User>(this, this));
        }

        #endregion

        #region Properties

        public string Email { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Description { get; private set; }

        public Guid PasswordSalt { get; private set; }

        public string HashedPassword { get; private set; }

        public DateTime LastLoggedIn { get; private set; }

        public string ResetPasswordKey { get; private set; }

        public int FlaggedItemsOwned { get; private set; }

        public int FlagsRaised { get; private set; }

        public List<DenormalisedNamedEntityReference<Role>> Roles { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Roles = new List<DenormalisedNamedEntityReference<Role>>();
        }

        private string GetHashedPassword(string password)
        {
            string hashedPassword;

            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(
                        PasswordSalt
                            .ToByteArray()
                            .Concat(Encoding.Unicode.GetBytes(PasswordSalt + password + _constantSalt)).ToArray());

                hashedPassword = Convert.ToBase64String(computedHash);
            }

            return hashedPassword;
        }

        private void SetDetails(string firstName, string lastName, string description)
        {
            FirstName = firstName;
            LastName = lastName;
            Description = description;
        }

        public bool ValidatePassword(string password)
        {
            return HashedPassword == GetHashedPassword(password);
        }

        public User UpdateEmail(string email)
        {
            Email = email;

            return this;
        }

        public User UpdatePassword(string password)
        {
            HashedPassword = GetHashedPassword(password);

            return this;
        }

        public User UpdateDetails(string firstName, string lastName, string description)
        {
            SetDetails(
                firstName,
                lastName,
                description);

            EventProcessor.Raise(new EntityUpdatedEvent<User>(this, this));

            return this;
        }

        public User UpdateLastLoggedIn()
        {
            LastLoggedIn = DateTime.Now;

            EventProcessor.Raise(new UserLoggedInEvent(this));

            return this;
        }

        public User UpdateResetPasswordKey()
        {
            ResetPasswordKey = Guid.NewGuid().ToString();

            return this;
        }

        public User AddRole(User updatedByUser, Role role)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");
            Check.RequireNotNull(role, "role");

            if (Roles.All(x => x.Id != role.Id))
            {
                Roles.Add((DenormalisedNamedEntityReference<Role>)role);
            }

            return this;
        }

        public User RemoveRole(User updatedByUser, string roleId)
        {
            Check.RequireNotNull(updatedByUser, "updatedByUser");

            Roles.RemoveAll(x => x.Id == roleId);

            return this;
        }

        #endregion

    }
}
