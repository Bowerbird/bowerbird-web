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
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Extensions;
using Newtonsoft.Json;

namespace Bowerbird.Core.DomainModels
{
    public class User : DomainModel
    {
        #region Members

        [JsonIgnore]
        private List<DenormalisedMemberReference> _memberships;
        [JsonIgnore]
        private List<Watchlist> _watchlists;
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
            string lastName) 
            : this() 
        {
            Email = email;
            PasswordSalt = Guid.NewGuid();
            HashedPassword = GetHashedPassword(password);
            LastLoggedIn = DateTime.Now;

            SetDetails(
                firstName,
                lastName,
                string.Empty,
                null);
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

        public MediaResource Avatar { get; private set; }

        public IEnumerable<DenormalisedMemberReference> Memberships 
        {
            get { return _memberships; }
            private set { _memberships = new List<DenormalisedMemberReference>(value); }
        }

        public IEnumerable<Watchlist> Watchlists
        {
            get { return _watchlists; }
            private set { _watchlists = new List<Watchlist>(value); }
        }

        #endregion

        #region Methods

        public string GetName()
        {
            return FirstName.AppendWith(" ").AppendWith(LastName);
        }

        private void InitMembers()
        {
            _memberships = new List<DenormalisedMemberReference>();
            _watchlists = new List<Watchlist>();
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

            FireEvent(new DomainModelUpdatedEvent<User>(this, this));

            return this;
        }

        public virtual User UpdateLastLoggedIn()
        {
            LastLoggedIn = DateTime.Now;

            FireEvent(new UserLoggedInEvent(this));

            return this;
        }

        public User RequestPasswordReset()
        {
            ResetPasswordKey = Guid.NewGuid().ToString();

            FireEvent(new RequestPasswordResetEvent() { User = this });

            return this;
        }

        public User AddMembership(Member member)
        {
            Check.RequireNotNull(member, "member");

            if (_memberships.All(x => x.Id != member.Id))
            {
                _memberships.Add(member);
            }
            
            return this;
        }

        public User RemoveMembership(string memberId)
        {
            _memberships.RemoveAll(x => x.Id == memberId);

            return this;
        }

        public User AddWatchlist(Watchlist watchlist)
        {
            Check.RequireNotNull(watchlist, "watchlist");

            if(_watchlists.All(x => (x.Name != watchlist.Name)))
            {
                _watchlists.Add(watchlist);
            }

            return this;
        }

        public User RemoveWatchlist(string watchlistId)
        {
            Check.RequireNotNullOrWhitespace(watchlistId, "watchlistId");

            _watchlists.RemoveAll(x => x.Id == watchlistId);

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