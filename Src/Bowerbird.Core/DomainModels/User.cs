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
using Bowerbird.Core.DesignByContract;
using System.Security.Cryptography;
using Bowerbird.Core.Events;
using Bowerbird.Core.Extensions;

namespace Bowerbird.Core.DomainModels
{
    public class User : DomainModel
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Member> _memberships;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Watchlist> _watchlists;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<UserSession> _sessions;
        private const string _constantSalt = "nf@hskdhI&%dynm^&%";

        #endregion

        #region Constructors

        protected User() 
            : base() 
        {
            InitMembers();

            EnableEvents();
        }

        public User(
            string password,
            string email,
            string firstName, 
            string lastName,
            MediaResource avatar,
            string defaultLicence) 
            : base() 
        {
            InitMembers();

            Email = email;
            PasswordSalt = Guid.NewGuid();
            HashedPassword = GetHashedPassword(password);
            LastLoggedIn = DateTime.UtcNow;

            SetDetails(
                firstName,
                lastName,
                avatar,
                defaultLicence);

            EnableEvents();
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

        public string DefaultLicence { get; private set; }

        public IEnumerable<Member> Memberships 
        {
            get { return _memberships; }
            private set { _memberships = new List<Member>(value); }
        }

        public IEnumerable<Watchlist> Watchlists
        {
            get { return _watchlists; }
            private set { _watchlists = new List<Watchlist>(value); }
        }

        public IEnumerable<UserSession> Sessions
        {
            get { return _sessions; }
            private set { _sessions = new List<UserSession>(value); }
        }

        #endregion

        #region Methods

        public string GetName()
        {
            return string.Format("{0} {1}", FirstName, LastName); 
        }

        private void InitMembers()
        {
            _memberships = new List<Member>();
            _watchlists = new List<Watchlist>();
            _sessions = new List<UserSession>();
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

        private void SetDetails(string firstName, string lastName, MediaResource avatar, string defaultLicence)
        {
            FirstName = firstName;
            LastName = lastName;
            Avatar = avatar;
            DefaultLicence = defaultLicence;
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

        public virtual User UpdateDetails(string firstName, string lastName, string description, MediaResource avatar, string defaultLicence)
        {
            SetDetails(
                firstName,
                lastName,
                avatar,
                defaultLicence);

            Description = description;

            FireEvent(new DomainModelUpdatedEvent<User>(this, this, this));

            return this;
        }

        public virtual User UpdateLastLoggedIn()
        {
            LastLoggedIn = DateTime.UtcNow;

            FireEvent(new UserLoggedInEvent(this, this));

            return this;
        }

        public User RequestPasswordReset()
        {
            ResetPasswordKey = Guid.NewGuid().ToString();

            FireEvent(new RequestPasswordResetEvent(this, this));

            return this;
        }

        public User AddMembership(User createdByUser, Group group, IEnumerable<Role> roles)
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(roles, "roles");

            var membership = _memberships.SingleOrDefault(x => x.Group.Id == group.Id);

            if (membership == null)
            {
                membership = new Member(createdByUser, group, roles);
                _memberships.Add(membership);

                FireEvent(new MemberCreatedEvent(membership, createdByUser, this, group));
            }
            else
            {
                membership.AddRoles(roles);

                FireEvent(new MemberUpdatedEvent(membership, createdByUser, this, group));
            }

            return this;
        }

        public User RemoveMembership(User modifiedByUser, Group group, IEnumerable<Role> roles)
        {
            var membership = _memberships.SingleOrDefault(x => x.Group.Id == group.Id);

            if (membership != null)
            {
                membership.RemoveRoles(roles.Select(x => x.Id));
            }

            if (membership.Roles.Count() == 0)
            {
                _memberships.Remove(membership);
            }

            return this;
        }

        public User RemoveMembership(User modifiedByUser, Group group)
        {
            _memberships.RemoveAll(x => x.Group.Id == group.Id);

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

        public User AddSession(string connectionId)
        {
            var session = new UserSession(connectionId);
            
            _sessions.Add(session);

            FireEvent(new DomainModelCreatedEvent<UserSession>(session, this, this));

            return this;
        }

        public User UpdateSessionLatestActivity(string connectionId)
        {
            var session = _sessions.SingleOrDefault(x => x.ConnectionId == connectionId);

            if (session != null)
            {
                session.UpdateLatestActivity(DateTime.UtcNow);

                FireEvent(new DomainModelUpdatedEvent<UserSession>(session, this, this));
            }

            return this;
        }

        public User RemoveSession(string connectionId)
        {
            _sessions.RemoveAll(x => x.ConnectionId == connectionId);

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