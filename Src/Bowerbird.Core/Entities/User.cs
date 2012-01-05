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
            Check.RequireNotNullOrWhitespace(id, "id");
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

            AddMembership(new GlobalMember(this, roles));

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

        public List<DenormalisedMemberReference> Memberships { get; private set; }

        #endregion

        #region Methods

        private void InitMembers()
        {
            Memberships = new List<DenormalisedMemberReference>();
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
            Check.RequireValidEmail(email, "email");

            Email = email;

            return this;
        }

        public User UpdatePassword(string password)
        {
            HashedPassword = GetHashedPassword(password);

            return this;
        }

        public virtual User UpdateDetails(string firstName, string lastName, string description)
        {
            SetDetails(
                firstName,
                lastName,
                description);

            EventProcessor.Raise(new EntityUpdatedEvent<User>(this, this));

            return this;
        }

        public virtual User UpdateLastLoggedIn()
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

        public User AddMembership(Member member)
        {
            Check.RequireNotNull(member, "member");

            if (Memberships.All(x => x.Type != member.GetType().Name.ToLower() && x.Id != member.Id))
            {
                Memberships.Add(member);
            }
            
            return this;
        }

        public User RemoveMembership(string memberType, string memberId)
        {
            Check.RequireNotNullOrWhitespace(memberType, "memberType");
            Check.RequireNotNullOrWhitespace(memberId, "memberId");

            Memberships.RemoveAll(x => x.Type == memberType && x.Id == memberId);

            return this;
        }

        public User IncrementFlaggedItemsOwned()
        {
            FlaggedItemsOwned++;

            return this;
        }

        public User IncrementFlagsRaised()
        {
            FlagsRaised++;

            return this;
        }

        #endregion

    }
}