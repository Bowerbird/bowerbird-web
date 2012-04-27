//#region namespaces

//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Reflection;
//using System.Security.Cryptography;
//using System.Text;
//using Raven.Client.Indexes;

//#endregion

//namespace Bowerbird.Test.Indexes
//{
//    public class ContributionIndexesTest
//    {

//    }
//}

//#region index classes

//public class GroupContributionResults
//{
//    public string ContributionId { get; set; }
//    public string UserId { get; set; }
//    public DateTime CreatedDateTime { get; set; }
//    public string GroupId { get; set; }
//    public string GroupUserId { get; set; }
//    public DateTime GroupCreatedDateTime { get; set; }
//    public Observation Observation { get; set; }
//    public ObservationNote ObservationNote { get; set; }
//    public Post Post { get; set; }
//    public Contribution Contribution
//    {
//        get { return (Contribution)Observation ?? (Contribution)ObservationNote ?? (Contribution)Post; }
//    }
//}

//public class All_GroupContributionItems : AbstractMultiMapIndexCreationTask<GroupContributionResults>
//{
//    public All_GroupContributionItems()
//    {
//        AddMap<Observation>(observations =>
//            from c in observations
//            from gc in c.GroupContributions
//            select new
//            {
//                ContributionId = c.Id,
//                UserId = c.User.Id,
//                CreatedDateTime = c.CreatedOn,
//                gc.GroupId,
//                GroupUserId = gc.User.Id,
//                GroupCreatedDateTime = gc.CreatedDateTime
//            });

//        AddMap<Post>(posts =>
//            from c in posts
//            from gc in c.GroupContributions
//            select new
//            {
//                ContributionId = c.Id,
//                UserId = c.User.Id,
//                CreatedDateTime = c.CreatedOn,
//                gc.GroupId,
//                GroupUserId = gc.User.Id,
//                GroupCreatedDateTime = gc.CreatedDateTime
//            });

//        AddMap<ObservationNote>(observationNotes =>
//            from c in observationNotes
//            from gc in c.GroupContributions
//            select new
//            {
//                ContributionId = c.Id,
//                UserId = c.User.Id,
//                CreatedDateTime = c.CreatedOn,
//                gc.GroupId,
//                GroupUserId = gc.User.Id,
//                GroupCreatedDateTime = gc.CreatedDateTime
//            });

//        TransformResults = (database, results) =>
//            from result in results
//            let observation = database.Load<Observation>(result.ContributionId)
//            let observationNote = database.Load<ObservationNote>(result.ContributionId)
//            let post = database.Load<Post>(result.ContributionId)
//            select new
//            {
//                result.ContributionId,
//                result.UserId,
//                result.CreatedDateTime,
//                result.GroupId,
//                result.GroupUserId,
//                result.GroupCreatedDateTime,
//                Observation = observation,
//                ObservationNote = observationNote,
//                Post = post
//            };

//        Store(x => x.ContributionId, FieldStorage.Yes);
//        Store(x => x.UserId, FieldStorage.Yes);
//        Store(x => x.CreatedDateTime, FieldStorage.Yes);
//        Store(x => x.GroupId, FieldStorage.Yes);
//        Store(x => x.GroupUserId, FieldStorage.Yes);
//        Store(x => x.GroupCreatedDateTime, FieldStorage.Yes);
//    }
//}

//#endregion

//#region domain  model classes, helpers, interfaces et al

//public class ObservationNote : Contribution
//{
//    protected ObservationNote()
//        : base()
//    {
//        InitMembers();
//    }

//    public ObservationNote(
//        User createdByUser,
//        Observation observation,
//        string commonName,
//        string scientificName,
//        string taxonomy,
//        string tags,
//        IDictionary<string, string> descriptions,
//        IDictionary<string, string> references,
//        string notes,
//        DateTime createdOn)
//        : base(
//        createdByUser,
//        createdOn)
//    {
//        Observation = observation;

//        SetDetails(
//            commonName,
//            scientificName,
//            taxonomy,
//            tags,
//            descriptions,
//            references,
//            notes);
//    }

//    public DenormalisedObservationReference Observation { get; private set; }

//    public string ScientificName { get; private set; }

//    public string CommonName { get; private set; }

//    public string Taxonomy { get; private set; }

//    public string Tags { get; private set; }

//    public Dictionary<string, string> Descriptions { get; private set; }

//    public Dictionary<string, string> References { get; private set; }

//    public string Notes { get; private set; }

//    private void InitMembers()
//    {
//        Descriptions = new Dictionary<string, string>();

//        References = new Dictionary<string, string>();
//    }

//    protected void SetDetails(string commonName, string scientificName, string taxonomy, string tags, IDictionary<string, string> descriptions, IDictionary<string, string> references, string notes)
//    {
//        CommonName = commonName;
//        ScientificName = scientificName;
//        Taxonomy = taxonomy;
//        Tags = tags;
//        Notes = notes;
//        Descriptions = descriptions.ToDictionary(x => x.Key, x => x.Value);
//        References = references.ToDictionary(x => x.Key, x => x.Value);
//    }

//    public ObservationNote UpdateDetails(User updatedByUser, string commonName, string scientificName, string taxonomy, string tags, IDictionary<string, string> descriptions, IDictionary<string, string> references, string notes)
//    {
//        SetDetails(
//            commonName,
//            scientificName,
//            taxonomy,
//            tags,
//            descriptions,
//            references,
//            notes);

//        return this;
//    }
//}

//public class DenormalisedObservationReference : ValueObject
//{
//    public string Id { get; set; }

//    public string Title { get; set; }

//    public static implicit operator DenormalisedObservationReference(Observation observation)
//    {
//        return new DenormalisedObservationReference
//        {
//            Id = observation.Id,
//            Title = observation.Title
//        };
//    }
//}

//public class Post : Contribution
//{
//    private List<MediaResource> _mediaResources;

//    protected Post() : base() { }

//    public Post(
//        User createdByUser,
//        DateTime createdOn,
//        string subject,
//        string message,
//        IEnumerable<MediaResource> mediaResources,
//        Group group)
//        : base(
//        createdByUser,
//        createdOn)
//    {
//        InitMembers();

//        SetDetails(
//            subject,
//            message,
//            mediaResources
//            );

//        AddGroupContribution(group, createdByUser, createdOn);
//    }

//    public string Subject { get; private set; }

//    public string Message { get; private set; }

//    public IEnumerable<MediaResource> MediaResources { get { return _mediaResources; } }

//    public IEnumerable<Comment> Comments
//    {
//        get { return _comments; }

//        private set { _comments = value as List<Comment>; }
//    }

//    private void SetDetails(string subject, string message, IEnumerable<MediaResource> mediaResources)
//    {
//        Subject = subject;
//        Message = message;
//        _mediaResources = mediaResources.ToList();
//    }

//    public Post UpdateDetails(User updatedByUser, string subject, string message, IEnumerable<MediaResource> mediaResources)
//    {
//        SetDetails(
//            subject,
//            message,
//            mediaResources);

//        return this;
//    }

//    private void InitMembers()
//    {
//        _comments = new List<Comment>();

//        _mediaResources = new List<MediaResource>();
//    }
//}

//public class Comment
//{
//    protected Comment() : base() { }

//    public Comment(
//        string commentId,
//        User createdByUser,
//        DateTime commentedOn,
//        string message)
//        : this()
//    {
//        CommentedOn = commentedOn;
//        User = createdByUser;
//        Id = commentId;

//        SetDetails(
//            message,
//            CommentedOn);
//    }

//    public string Id { get; private set; }

//    public DenormalisedUserReference User { get; private set; }

//    public DateTime CommentedOn { get; private set; }

//    public DateTime EditedOn { get; private set; }

//    public string Message { get; private set; }

//    private void SetDetails(string message, DateTime editedOn)
//    {
//        Message = message;
//        EditedOn = editedOn;
//    }

//    public Comment UpdateDetails(User updatedByUser, DateTime editedOn, string message)
//    {
//        SetDetails(
//            message,
//            editedOn);

//        return this;
//    }
//}

//public abstract class MediaResource : DomainModel
//{
//    protected MediaResource() : base() { }

//    protected MediaResource(
//        User createdByUser,
//        DateTime uploadedOn,
//        string originalFileName,
//        string fileFormat,
//        string description)
//        : this()
//    {
//        SetDetails(
//            createdByUser,
//            uploadedOn,
//            originalFileName,
//            fileFormat,
//            description);
//    }

//    public DenormalisedUserReference CreatedByUser { get; set; }

//    public string OriginalFileName { get; private set; }

//    public string FileFormat { get; private set; }

//    public string Description { get; private set; }

//    public DateTime UploadedOn { get; private set; }

//    private void SetDetails(User createdByUser, DateTime uploadedOn, string originalFileName, string fileFormat, string description)
//    {
//        Id = Guid.NewGuid().ToString();
//        UploadedOn = uploadedOn;
//        CreatedByUser = createdByUser;
//        OriginalFileName = originalFileName;
//        FileFormat = fileFormat;
//        Description = description;
//    }

//    protected void UpdateDetails(string description)
//    {
//        Description = description;
//    }
//}

//public abstract class Member : DomainModel
//{
//    protected Member()
//        : base()
//    {
//        InitMembers();
//    }

//    protected Member(
//        User user,
//        IEnumerable<Role> roles)
//        : this()
//    {
//        User = user;

//        Id = (new Random(System.DateTime.Now.Millisecond)).Next().ToString();
//        Roles = roles.Select(x => (DenormalisedNamedDomainModelReference<Role>)x).ToList();
//    }

//    public DenormalisedUserReference User { get; private set; }

//    public string Type { get; private set; }

//    public List<DenormalisedNamedDomainModelReference<Role>> Roles { get; private set; }

//    private void InitMembers()
//    {
//        Roles = new List<DenormalisedNamedDomainModelReference<Role>>();
//    }

//    public Member AddRole(Role role)
//    {
//        SetRole(role);

//        return this;
//    }

//    internal Member AddRoles(IEnumerable<DenormalisedNamedDomainModelReference<Role>> roles)
//    {
//        foreach (var role in roles)
//        {
//            SetRole(role);
//        }

//        return this;
//    }

//    public Member RemoveRole(string roleId)
//    {
//        Roles.RemoveAll(x => x.Id == roleId);

//        return this;
//    }

//    private void SetRole(DenormalisedNamedDomainModelReference<Role> role)
//    {
//        if (Roles.All(x => x.Id != role.Id))
//        {
//            Roles.Add(role);
//        }
//    }
//}

//public class Watchlist : DomainModel, INamedDomainModel
//{
//    public Watchlist()
//        : base()
//    {
//    }

//    public Watchlist(
//        User createdByUser,
//        string name,
//        string querystringJson
//        )
//        : base()
//    {
//        User = createdByUser;

//        SetDetails(
//            name,
//            querystringJson);
//    }

//    public DenormalisedUserReference User { get; set; }

//    public string Name { get; set; }

//    public string QuerystringJson { get; set; }

//    private void SetDetails(string name, string querystringJson)
//    {
//        Name = name;
//        QuerystringJson = querystringJson;
//    }

//    public void UpdateDetails(User updatedByUser, string name, string querystringJson)
//    {
//        SetDetails(name, querystringJson);
//    }
//}

//public class GlobalMember : Member
//{
//    protected GlobalMember()
//        : base()
//    {
//    }

//    public GlobalMember(
//        User user,
//        IEnumerable<Role> roles)
//        : base(
//        user,
//        roles)
//    {
//    }
//}

//public class User : DomainModel
//{
//    private const string _constantSalt = "nf@hskdhI&%dynm^&%";

//    protected User()
//        : base()
//    {
//        InitMembers();
//    }

//    public User(
//        string password,
//        string email,
//        string firstName,
//        string lastName,
//        IEnumerable<Role> roles)
//        : this()
//    {
//        Email = email;
//        PasswordSalt = Guid.NewGuid();
//        HashedPassword = GetHashedPassword(password);
//        LastLoggedIn = DateTime.Now;

//        SetDetails(
//            firstName,
//            lastName,
//            string.Empty);

//        AddMembership(new GlobalMember(this, roles));
//    }

//    public string Email { get; private set; }

//    public string FirstName { get; private set; }

//    public string LastName { get; private set; }

//    public string Description { get; private set; }

//    public Guid PasswordSalt { get; private set; }

//    public string HashedPassword { get; private set; }

//    public DateTime LastLoggedIn { get; private set; }

//    public string ResetPasswordKey { get; private set; }

//    public int FlaggedItemsOwned { get; private set; }

//    public int FlagsRaised { get; private set; }

//    public List<DenormalisedMemberReference> Memberships { get; private set; }

//    public List<DenormalisedNamedDomainModelReference<Watchlist>> Watchlists { get; private set; }

//    private void InitMembers()
//    {
//        Memberships = new List<DenormalisedMemberReference>();
//    }

//    private string GetHashedPassword(string password)
//    {
//        string hashedPassword;

//        using (var sha = SHA256.Create())
//        {
//            var computedHash = sha.ComputeHash(
//                    PasswordSalt
//                        .ToByteArray()
//                        .Concat(Encoding.Unicode.GetBytes(PasswordSalt + password + _constantSalt)).ToArray());

//            hashedPassword = Convert.ToBase64String(computedHash);
//        }

//        return hashedPassword;
//    }

//    private void SetDetails(string firstName, string lastName, string description)
//    {
//        FirstName = firstName;
//        LastName = lastName;
//        Description = description;
//    }

//    public bool ValidatePassword(string password)
//    {
//        return HashedPassword == GetHashedPassword(password);
//    }

//    public User UpdateEmail(string email)
//    {
//        Email = email;

//        return this;
//    }

//    /// <summary>
//    /// Update password and set resetpasswordkey to null
//    /// </summary>
//    public User UpdatePassword(string password)
//    {
//        HashedPassword = GetHashedPassword(password);

//        ResetPasswordKey = null; // Set to null indicating password is not resettable

//        return this;
//    }

//    public virtual User UpdateDetails(string firstName, string lastName, string description)
//    {
//        SetDetails(
//            firstName,
//            lastName,
//            description);

//        return this;
//    }

//    public virtual User UpdateLastLoggedIn()
//    {
//        LastLoggedIn = DateTime.Now;

//        return this;
//    }

//    public User RequestPasswordReset()
//    {
//        ResetPasswordKey = Guid.NewGuid().ToString();

//        return this;
//    }

//    public User AddMembership(Member member)
//    {
//        if (Memberships.All(x => (x.Type != member.GetType().Name.ToLower() && x.Id != member.Id)))
//        {
//            Memberships.Add(member);
//        }

//        return this;
//    }

//    public User RemoveMembership(string memberType, string memberId)
//    {
//        Memberships.RemoveAll(x => x.Type == memberType && x.Id == memberId);

//        return this;
//    }

//    public User AddWatchlist(Watchlist watchlist)
//    {
//        if (Watchlists.All(x => (x.Name != watchlist.Name)))
//        {
//            Watchlists.Add(watchlist);
//        }

//        return this;
//    }

//    public User RemoveWatchlist(string watchlistId)
//    {
//        Watchlists.RemoveAll(x => (x.Id == watchlistId));

//        return this;
//    }

//    public User IncrementFlaggedItemsOwned()
//    {
//        FlaggedItemsOwned++;

//        return this;
//    }

//    public User IncrementFlagsRaised()
//    {
//        FlagsRaised++;

//        return this;
//    }
//}

//public class Permission : DomainModel, INamedDomainModel
//{
//    protected Permission() : base() { }

//    public Permission(
//        string id,
//        string name,
//        string description)
//        : this()
//    {
//        SetDetails(
//            id,
//            name,
//            description);
//    }

//    public string Name { get; private set; }

//    public string Description { get; private set; }

//    private void SetDetails(string id, string name, string description)
//    {
//        Id = "permissions/" + id;
//        Name = name;
//        Description = description;
//    }
//}

//public class Role : DomainModel, INamedDomainModel
//{
//    protected Role() : base() { }

//    public Role(
//        string id,
//        string name,
//        string description,
//        IEnumerable<Permission> permissions)
//        : this()
//    {
//        SetDetails(
//            id,
//            name,
//            description,
//            permissions);
//    }

//    public string Name { get; private set; }

//    public string Description { get; private set; }

//    public List<DenormalisedNamedDomainModelReference<Permission>> Permissions { get; private set; }

//    private void SetDetails(string id, string name, string description, IEnumerable<Permission> permissions)
//    {
//        Id = "roles/" + id;
//        Name = name;
//        Description = description;

//        Permissions = permissions.Select(permission =>
//        {
//            DenormalisedNamedDomainModelReference<Permission> denorm = permission;
//            return denorm;
//        }).ToList();
//    }
//}

//public class Observation : Contribution
//{
//    private List<MediaResource> _mediaResources;

//    protected Observation()
//        : base()
//    {
//        InitMembers();
//    }

//    public Observation(
//        User createdByUser,
//        string title,
//        DateTime createdOn,
//        DateTime observedOn,
//        string latitude,
//        string longitude,
//        string address,
//        bool isIdentificationRequired,
//        string observationCategory,
//        IEnumerable<MediaResource> mediaResources)
//        : base(
//        createdByUser,
//        createdOn)
//    {
//        Id = "observations/";

//        InitMembers();

//        SetDetails(
//            title,
//            observedOn,
//            latitude,
//            longitude,
//            address,
//            isIdentificationRequired,
//            observationCategory,
//            mediaResources);
//    }

//    public string Title { get; private set; }

//    public DateTime ObservedOn { get; private set; }

//    public string Latitude { get; private set; }

//    public string Longitude { get; private set; }

//    public string Address { get; private set; }

//    public bool IsIdentificationRequired { get; private set; }

//    public string ObservationCategory { get; private set; }

//    public IEnumerable<Comment> Comments
//    {
//        get { return _comments; }

//        private set { _comments = value as List<Comment>; }
//    }

//    public IEnumerable<MediaResource> MediaResources { get { return _mediaResources; } }

//    private void InitMembers()
//    {
//        _mediaResources = new List<MediaResource>();

//        _comments = new List<Comment>();
//    }

//    private void SetDetails(string title, DateTime observedOn, string latitude, string longitude, string address, bool isIdentificationRequired, string observationCategory, IEnumerable<MediaResource> mediaResources)
//    {
//        Title = title;
//        ObservedOn = observedOn;
//        Latitude = latitude;
//        Longitude = longitude;
//        Address = address;
//        IsIdentificationRequired = isIdentificationRequired;
//        ObservationCategory = observationCategory;
//        _mediaResources = mediaResources.ToList();
//    }

//    public virtual Observation UpdateDetails(User updatedByUser, string title, DateTime observedOn, string latitude, string longitude, string address, bool isIdentificationRequired, string observationCategory, IEnumerable<MediaResource> mediaResources)
//    {
//        SetDetails(
//            title,
//            observedOn,
//            latitude,
//            longitude,
//            address,
//            isIdentificationRequired,
//            observationCategory,
//            mediaResources);

//        return this;
//    }
//}

//public abstract class Contribution : DomainModel
//{
//    private List<GroupContribution> _groupContributions;

//    protected List<Comment> _comments;

//    protected Contribution()
//    {
//        InitMembers();
//    }

//    protected Contribution(
//        User createdByUser,
//        DateTime createdOn)
//        : this()
//    {
//        User = createdByUser;
//        CreatedOn = createdOn;
//    }

//    public DenormalisedUserReference User { get; private set; }

//    public DateTime CreatedOn { get; private set; }

//    public IEnumerable<GroupContribution> GroupContributions { get { return _groupContributions; } }

//    public void AddGroupContribution(Group group, User createdByUser, DateTime createdDateTime)
//    {
//        if (_groupContributions.All(x => x.GroupId != group.Id))
//        {
//            var groupContribution = new GroupContribution(group, createdByUser, createdDateTime);

//            _groupContributions.Add(groupContribution);
//        }
//    }

//    public void RemoveGroupContribution(string groupId)
//    {
//        if (_groupContributions.Any(x => x.GroupId == groupId))
//        {
//            _groupContributions.RemoveAll(x => x.GroupId == groupId);
//        }
//    }

//    public void AddComment(string message, User createdByUser, DateTime createdDateTime)
//    {
//        var commentId = "1";

//        var comments = _comments
//            .OrderBy(x => x.Id)
//            .Select(x => x.Id)
//            .ToList();

//        if (comments != null && comments.Count > 0)
//        {
//            int idOfLastComment;
//            if (Int32.TryParse(comments[comments.Count - 1], out idOfLastComment))
//            {
//                commentId = (++idOfLastComment).ToString();
//            }
//        }

//        var newComment = new Comment(commentId, createdByUser, createdDateTime, message);

//        _comments.Add(newComment);
//    }

//    public void RemoveComment(string commentId)
//    {
//        if (_comments.Any(x => x.Id == commentId))
//        {
//            _comments.RemoveAll(x => x.Id == commentId);
//        }
//    }

//    public void UpdateComment(string commentId, string message, User modifiedByUser, DateTime modifiedDateTime)
//    {
//        if (_comments.Any(x => x.Id == commentId))
//        {
//            var comment = _comments.Where(x => x.Id == commentId).FirstOrDefault();

//            comment.UpdateDetails(modifiedByUser, modifiedDateTime, message);
//        }
//    }

//    private void InitMembers()
//    {
//        _comments = new List<Comment>();

//        _groupContributions = new List<GroupContribution>();
//    }
//}

//public class GroupContribution : ValueObject
//{
//    protected GroupContribution()
//    {
//    }

//    public GroupContribution(
//        Group group,
//        User createdByUser,
//        DateTime createdDateTime)
//    {
//        SetDetails(group,
//            createdByUser,
//            createdDateTime);
//    }

//    public string GroupId { get; private set; }

//    public DenormalisedUserReference User { get; private set; }

//    public DateTime CreatedDateTime { get; private set; }

//    private void SetDetails(
//        Group group,
//        User createdByUser,
//        DateTime createdDateTime
//        )
//    {
//        GroupId = group.Id;
//        User = createdByUser;
//        CreatedDateTime = createdDateTime;
//    }
//}

//public class DenormalisedMemberReference : ValueObject
//{
//    public string Type { get; private set; }

//    public string Id { get; private set; }

//    public IEnumerable<string> Roles { get; private set; }

//    public static implicit operator DenormalisedMemberReference(Member member)
//    {
//        return new DenormalisedMemberReference
//        {
//            Type = SetMemberType(member),
//            Id = member.Id,
//            Roles = member.Roles.Select(x => x.Id)
//        };
//    }

//    private static string SetMemberType(Member member)
//    {
//        if (member is GroupMember) return "groupmember";

//        return "globalmember";
//    }
//}

//public class GroupMember : Member
//{
//    protected GroupMember()
//        : base()
//    {
//    }

//    public GroupMember(
//        User createdByUser,
//        Group group,
//        User user,
//        IEnumerable<Role> roles)
//        : base(
//        user,
//        roles)
//    {
//        Group = group;
//    }

//    public DenormalisedNamedDomainModelReference<Group> Group { get; private set; }
//}

//public class DenormalisedUserReference : ValueObject
//{
//    public string Id { get; private set; }

//    public string FirstName { get; private set; }

//    public string LastName { get; private set; }

//    public static implicit operator DenormalisedUserReference(User user)
//    {
//        return new DenormalisedUserReference
//        {
//            Id = user.Id,
//            FirstName = user.FirstName,
//            LastName = user.LastName
//        };
//    }
//}

//public abstract class Group : DomainModel, INamedDomainModel
//{
//    private List<GroupAssociation> _childGroupAssociations;

//    protected Group()
//    {
//        InitMembers();
//    }

//    protected Group(
//        string name)
//        : this()
//    {
//        Name = name;
//    }

//    public string Name { get; private set; }

//    public string ParentGroupId { get; protected set; }

//    public string Description { get; private set; }

//    public string Website { get; private set; }

//    public IEnumerable<GroupAssociation> ChildGroupAssociations { get { return _childGroupAssociations; } }

//    private void InitMembers()
//    {
//        _childGroupAssociations = new List<GroupAssociation>();
//    }

//    protected void SetDetails(string name, string description, string website, string parentGroupId = null)
//    {
//        Name = name;
//        Description = description;
//        Website = website;
//        ParentGroupId = parentGroupId;
//    }

//    public void AddGroupAssociation(Group group, User createdByUser, DateTime createdDateTime)
//    {
//        if (_childGroupAssociations.All(x => x.GroupId != group.Id))
//        {
//            var groupAssociation = new GroupAssociation(group, createdByUser, createdDateTime);

//            _childGroupAssociations.Add(groupAssociation);
//        }
//    }

//    public void RemoveGroupAssociation(string groupId)
//    {
//        if (_childGroupAssociations.Any(x => x.GroupId == groupId))
//        {
//            _childGroupAssociations.RemoveAll(x => x.GroupId == groupId);
//        }
//    }
//}

//public abstract class DomainModel : BaseObject, IAssignableId
//{
//    private const int HashMultiplier = 31;

//    private int? cachedHashcode;

//    public string Id { get; protected set; }

//    public override bool Equals(object obj)
//    {
//        var compareTo = obj as DomainModel;

//        if (ReferenceEquals(this, compareTo))
//        {
//            return true;
//        }

//        if (compareTo == null || !this.GetType().Equals(compareTo.GetTypeUnproxied()))
//        {
//            return false;
//        }

//        if (this.HasSameNonDefaultIdAs(compareTo))
//        {
//            return true;
//        }

//        return this.IsTransient() && compareTo.IsTransient() && this.HasSameObjectSignatureAs(compareTo);
//    }

//    public override int GetHashCode()
//    {
//        if (this.cachedHashcode.HasValue)
//        {
//            return this.cachedHashcode.Value;
//        }

//        if (this.IsTransient())
//        {
//            this.cachedHashcode = base.GetHashCode();
//        }
//        else
//        {
//            unchecked
//            {
//                var hashCode = this.GetType().GetHashCode();
//                this.cachedHashcode = (hashCode * HashMultiplier) ^ this.Id.GetHashCode();
//            }
//        }

//        return this.cachedHashcode.Value;
//    }

//    public virtual bool IsTransient()
//    {
//        return this.Id == null || this.Id.Equals(default(string));
//    }

//    protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
//    {
//        return
//            this.GetType().GetProperties().Where(
//                p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));
//    }

//    private bool HasSameNonDefaultIdAs(DomainModel compareTo)
//    {
//        return !this.IsTransient() && !compareTo.IsTransient() && this.Id.Equals(compareTo.Id);
//    }

//    public virtual bool IsValid()
//    {
//        return this.ValidationResults().Count == 0;
//    }

//    public virtual ICollection<ValidationResult> ValidationResults()
//    {
//        var validationResults = new List<ValidationResult>();
//        Validator.TryValidateObject(this, new ValidationContext(this, null, null), validationResults, true);
//        return validationResults;
//    }

//    void IAssignableId.SetIdTo(string prefix, string assignedId)
//    {
//        Id = string.Format("{0}/{1}", prefix, assignedId);
//    }

//}

//public interface IAssignableId
//{
//    void SetIdTo(string prefix, string assignedId);
//}

//public class DomainSignatureAttribute : Attribute
//{
//}

//public abstract class BaseObject
//{
//    private const int HashMultiplier = 31;

//    [ThreadStatic]
//    private static Dictionary<Type, IEnumerable<PropertyInfo>> signaturePropertiesDictionary;

//    public override bool Equals(object obj)
//    {
//        var compareTo = obj as BaseObject;

//        if (ReferenceEquals(this, compareTo))
//        {
//            return true;
//        }

//        return compareTo != null && this.GetType().Equals(compareTo.GetTypeUnproxied()) &&
//               this.HasSameObjectSignatureAs(compareTo);
//    }

//    public override int GetHashCode()
//    {
//        unchecked
//        {
//            var signatureProperties = this.GetSignatureProperties();

//            var hashCode = this.GetType().GetHashCode();

//            hashCode = signatureProperties.Select(property => property.GetValue(this, null))
//                                          .Where(value => value != null)
//                                          .Aggregate(hashCode, (current, value) => (current * HashMultiplier) ^ value.GetHashCode());

//            if (signatureProperties.Any())
//            {
//                return hashCode;
//            }

//            return base.GetHashCode();
//        }
//    }

//    public virtual IEnumerable<PropertyInfo> GetSignatureProperties()
//    {
//        IEnumerable<PropertyInfo> properties;

//        if (signaturePropertiesDictionary == null)
//        {
//            signaturePropertiesDictionary = new Dictionary<Type, IEnumerable<PropertyInfo>>();
//        }

//        if (signaturePropertiesDictionary.TryGetValue(this.GetType(), out properties))
//        {
//            return properties;
//        }

//        return signaturePropertiesDictionary[this.GetType()] = this.GetTypeSpecificSignatureProperties();
//    }

//    public virtual bool HasSameObjectSignatureAs(BaseObject compareTo)
//    {
//        var signatureProperties = this.GetSignatureProperties();

//        if ((from property in signatureProperties
//             let valueOfThisObject = property.GetValue(this, null)
//             let valueToCompareTo = property.GetValue(compareTo, null)
//             where valueOfThisObject != null || valueToCompareTo != null
//             where (valueOfThisObject == null ^ valueToCompareTo == null) || (!valueOfThisObject.Equals(valueToCompareTo))
//             select valueOfThisObject).Any())
//        {
//            return false;
//        }

//        return signatureProperties.Any() || base.Equals(compareTo);
//    }

//    protected abstract IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties();

//    protected virtual Type GetTypeUnproxied()
//    {
//        return this.GetType();
//    }
//}

//public abstract class ValueObject : BaseObject
//{
//    public static bool operator ==(ValueObject valueObject1, ValueObject valueObject2)
//    {
//        if ((object)valueObject1 == null)
//        {
//            return (object)valueObject2 == null;
//        }

//        return valueObject1.Equals(valueObject2);
//    }

//    public static bool operator !=(ValueObject valueObject1, ValueObject valueObject2)
//    {
//        return !(valueObject1 == valueObject2);
//    }

//    public override bool Equals(object obj)
//    {
//        return base.Equals(obj);
//    }

//    public override int GetHashCode()
//    {
//        return base.GetHashCode();
//    }

//    protected override IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
//    {
//        var invalidlyDecoratedProperties =
//            this.GetType().GetProperties().Where(
//                p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));

//        string message = "Properties were found within " + this.GetType() +
//                         @" having the
//                [DomainSignature] attribute. The domain signature of a value object includes all
//                of the properties of the object by convention; consequently, adding [DomainSignature]
//                to the properties of a value object's properties is misleading and should be removed. 
//                Alternatively, you can inherit from DomainModel if that fits your needs better.";

//        return this.GetType().GetProperties();
//    }
//}

//public class GroupAssociation : ValueObject
//{
//    protected GroupAssociation()
//        : base()
//    {
//    }

//    public GroupAssociation(
//        Group group,
//        User createdByUser,
//        DateTime createdDateTime)
//        : base()
//    {
//        GroupId = group.Id;
//        CreatedByUserId = createdByUser.Id;
//        CreatedDateTime = createdDateTime;
//    }

//    public string GroupId { get; private set; }

//    public string CreatedByUserId { get; private set; }

//    public DateTime CreatedDateTime { get; private set; }
//}

//public interface INamedDomainModel
//{
//    string Id { get; }

//    string Name { get; }
//}

//public class DenormalisedNamedDomainModelReference<T> : ValueObject where T : INamedDomainModel
//{
//    public string Id { get; set; }

//    public string Name { get; set; }

//    public static implicit operator DenormalisedNamedDomainModelReference<T>(T namedDomainModel)
//    {
//        return new DenormalisedNamedDomainModelReference<T>
//        {
//            Id = namedDomainModel.Id,
//            Name = namedDomainModel.Name
//        };
//    }
//}

//#endregion