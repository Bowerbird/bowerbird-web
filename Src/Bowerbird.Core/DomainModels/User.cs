/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.Config;
using Bowerbird.Core.DesignByContract;
using System.Security.Cryptography;
using Bowerbird.Core.DomainModels.Members;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class User : DomainModel
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
            string password,
            string email,
            string firstName, 
            string lastName,
            IEnumerable<Role> roles) 
            : this() 
        {
            Check.RequireNotNull(roles, "roles");

            Email = email;
            PasswordSalt = Guid.NewGuid();
            HashedPassword = GetHashedPassword(password);
            LastLoggedIn = DateTime.Now;

            SetDetails(
                firstName,
                lastName,
                string.Empty,
                null);

            AddMembership(new GlobalMember(this, roles));

            var eventMessage = string.Format(
                ActivityMessage.Joined,
                GetName()
                );

            EventProcessor.Raise(new DomainModelCreatedEvent<User>(this, this, eventMessage));
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

        public MediaResource Avatar { get; set; }

        public List<DenormalisedMemberReference> Memberships { get; private set; }

        public List<DenormalisedNamedDomainModelReference<Watchlist>> Watchlists { get; private set; }

        #endregion

        #region Methods

        public string GetName()
        {
            return FirstName.AppendWith(" ").AppendWith(LastName);
        }

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

        private void SetDetails(string firstName, string lastName, string description, MediaResource avatar)
        {
            FirstName = firstName;
            LastName = lastName;
            Description = description;
            Avatar = avatar;
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

        /// <summary>
        /// Update password and set resetpasswordkey to null
        /// </summary>
        public User UpdatePassword(string password)
        {
            HashedPassword = GetHashedPassword(password);

            ResetPasswordKey = null; // Set to null indicating password is not resettable

            return this;
        }

        public virtual User UpdateDetails(string firstName, string lastName, string description, MediaResource avatar)
        {
            SetDetails(
                firstName,
                lastName,
                description,
                avatar);

            var eventMessage = string.Format(
                ActivityMessage.UpdatedTheirDetails,
                GetName()
                );

            EventProcessor.Raise(new DomainModelUpdatedEvent<User>(this, this, eventMessage));

            return this;
        }

        public virtual User UpdateLastLoggedIn()
        {
            LastLoggedIn = DateTime.Now;

            EventProcessor.Raise(new UserLoggedInEvent(this));

            return this;
        }

        public User RequestPasswordReset()
        {
            ResetPasswordKey = Guid.NewGuid().ToString();

            EventProcessor.Raise(new RequestPasswordResetEvent(){ User = this });

            return this;
        }

        public User AddMembership(Member member)
        {
            Check.RequireNotNull(member, "member");

            if (Memberships.All(x => (x.Type != member.GetType().Name.ToLower() && x.Id != member.Id)))
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

        public User AddWatchlist(Watchlist watchlist)
        {
            Check.RequireNotNull(watchlist, "watchlist");

            if(Watchlists.All(x => (x.Name != watchlist.Name)))
            {
                Watchlists.Add(watchlist);
            }

            return this;
        }

        public User RemoveWatchlist(string watchlistId)
        {
            Check.RequireNotNullOrWhitespace(watchlistId, "watchlistId");

            Watchlists.RemoveAll(x => (x.Id == watchlistId));

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