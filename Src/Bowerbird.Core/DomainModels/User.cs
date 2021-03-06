﻿/* Bowerbird V1 - Licensed under MIT 1.1 Public License

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
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using Bowerbird.Core.Events;

namespace Bowerbird.Core.DomainModels
{
    public class User : DomainModel
    {
        #region Members

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<Member> _memberships;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<UserSession> _sessions;
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        private List<string> _callsToAction;
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
            string name,
            MediaResource avatar,
            string defaultLicence,
            string timezone,
            DateTime joined) 
            : base() 
        {
            InitMembers();

            Email = email;
            PasswordSalt = Guid.NewGuid();
            HashedPassword = GetHashedPassword(password);
            LastLoggedIn = DateTime.UtcNow;
            Joined = joined;

            SetDetails(
                name,
                avatar,
                string.Empty,
                defaultLicence,
                timezone);

            AddCallToAction("user-welcome");
            AddCallToAction("first-create-project-welcome");
            AddCallToAction("first-create-observation-welcome");
            AddCallToAction("first-record-welcome");
            AddCallToAction("project-explore-welcome");
            AddCallToAction("organisation-explore-welcome");

            ApplyEvent(new DomainModelCreatedEvent<User>(this, this, this));
        }

        #endregion

        #region Properties

        public string Email { get; private set; }

        public string Name { get; private set; }

        public DateTime Joined { get; private set; }

        public string Description { get; private set; }

        public Guid PasswordSalt { get; private set; }

        public string HashedPassword { get; private set; }

        public DateTime LastLoggedIn { get; private set; }

        public string ResetPasswordKey { get; private set; }

        public int FlaggedItemsOwned { get; private set; }

        public int FlagsRaised { get; private set; }

        public MediaResource Avatar { get; private set; }

        public string DefaultLicence { get; private set; }

        public string Timezone { get; private set; }

        public IEnumerable<string> CallsToAction
        {
            get { return _callsToAction; }
            private set { _callsToAction = new List<string>(value); }
        }

        public IEnumerable<Member> Memberships 
        {
            get { return _memberships; }
            private set { _memberships = new List<Member>(value); }
        }

        public IEnumerable<UserSession> Sessions
        {
            get { return _sessions; }
            private set { _sessions = new List<UserSession>(value); }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DateTime? SessionLatestActivity
        {
            get
            {
                return _sessions.Count > 0 ? _sessions.Select(x => x.LatestActivity).OrderByDescending(x => x).First() : (DateTime?)null;
            }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DateTime? SessionLatestHeartbeat
        {
            get
            {
                return _sessions.Count > 0 ? _sessions.Select(x => x.LatestHeartbeat).OrderByDescending(x => x).First() : (DateTime?)null;
            }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DenormalisedGroupReference UserProject
        {
            get { return Memberships.First(x => x.Group.GroupType == "userproject" && x.Group.CreatedBy == this.Id).Group; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public DenormalisedGroupReference Favourites
        {
            get { return Memberships.First(x => x.Group.GroupType == "favourites").Group; }
        }

        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public IEnumerable<Member> FollowingUsers
        {
            get { return Memberships.Where(x => x.Group.GroupType == "userproject" && x.Group.CreatedBy != this.Id); }
        }

        #endregion

        #region Methods

        public string GetName()
        {
            return Name; 
        }

        private void InitMembers()
        {
            _memberships = new List<Member>();
            _sessions = new List<UserSession>();
            _callsToAction = new List<string>();
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

        private void SetDetails(string name, MediaResource avatar, string description, 
            string defaultLicence, string timezone)
        {
            Name = name;
            Avatar = avatar;
            Description = description;
            DefaultLicence = defaultLicence;
            Timezone = timezone;
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

        public virtual User UpdateDetails(string name, string description, MediaResource avatar, string defaultLicence, string timezone)
        {
            SetDetails(
                name,
                avatar,
                description,
                defaultLicence,
                timezone);

            ApplyEvent(new DomainModelUpdatedEvent<User>(this, this, this));

            return this;
        }

        public virtual User UpdateLastLoggedIn()
        {
            LastLoggedIn = DateTime.UtcNow;

            ApplyEvent(new UserLoggedInEvent(this, this));

            return this;
        }

        public User RequestPasswordUpdate(bool sendEmail)
        {
            ResetPasswordKey = Guid.NewGuid().ToString();

            ApplyEvent(new RequestPasswordUpdate(this, this, sendEmail));

            return this;
        }

        public User UpdateMembership(User modifiedByUser, Group group, IEnumerable<Role> roles)
        {
            Check.RequireNotNull(group, "group");
            Check.RequireNotNull(roles, "roles");

            // FInd if user is a member yet
            var member = _memberships.FirstOrDefault(x => x.Group.Id == group.Id);

            if (member != null)
            {
                member.UpdateRoles(roles);

                ApplyEvent(new MemberUpdatedEvent(member, modifiedByUser, this, group));
            }
            else
            {
                member = new Member(modifiedByUser, DateTime.UtcNow, group, roles);
                _memberships.Add(member);

                ApplyEvent(new MemberCreatedEvent(member, modifiedByUser, this, group));
            }

            return this;
        }

        public User RemoveMembership(User modifiedByUser, Group group)
        {
            _memberships.RemoveAll(x => x.Group.Id == group.Id);

            return this;
        }

        public User AddSession(string connectionId)
        {
            var session = new UserSession(connectionId);

            _sessions.Add(session);

            ApplyEvent(new DomainModelCreatedEvent<UserSession>(session, this, this));

            // Some housekeeping here. Remove all old sessions that are unlikely to still be live.
            _sessions.RemoveAll(x => x.LatestHeartbeat < DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)));

            return this;
        }

        public User UpdateSessionLatestActivity(string connectionId, DateTime latestHeartbeat, DateTime latestInteractivity)
        {
            var session = _sessions.SingleOrDefault(x => x.ConnectionId == connectionId);

            if (session != null)
            {
                session.UpdateLatestActivity(latestHeartbeat, latestInteractivity);

                ApplyEvent(new DomainModelUpdatedEvent<UserSession>(session, this, this));
            }

            return this;
        }

        public User RemoveSession(string connectionId)
        {
            _sessions.RemoveAll(x => x.ConnectionId == connectionId);

            return this;
        }

        public User AddCallToAction(string name)
        {
            _callsToAction.Add(name);

            return this;
        }

        public User RemoveCallToAction(string name)
        {
            _callsToAction.Remove(name);

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