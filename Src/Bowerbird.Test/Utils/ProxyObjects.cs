using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bowerbird.Core.CommandHandlers;
using Bowerbird.Core.Commands;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.Events;

namespace Bowerbird.Test.Utils
{
    public static class ProxyObjects
    {

        public class ProxyDomainEvent : IDomainEvent
        {
            public string Id
            {
                get { return FakeValues.KeyString; }
            }
        }

        public class ProxyCommand : ICommand
        {
            public bool IsValid()
            {
                throw new NotImplementedException();
            }

            public ICollection<ValidationResult> ValidationResults()
            {
                throw new NotImplementedException();
            }
        }

        public class ProxyResult { }

        public class ProxyCommandHandler : ICommandHandler<ProxyCommand>
        {
            public virtual void Handle(ProxyCommand command)
            {
                return;
            }
        }

        public class ProxyCommandHandlerWithResult : ICommandHandler<ProxyCommand, ProxyResult>
        {
            public virtual ProxyResult Handle(ProxyCommand command)
            {
                return new ProxyResult();
            }
        }

        public class ProxyMediaResource : MediaResource
        {
            public ProxyMediaResource(string originalFileName, string fileFormat, string description)
                : base(FakeObjects.TestUser(),
                FakeValues.CreatedDateTime,
                originalFileName,
                fileFormat,
                description) { }
        }

        public class ProxyMember : Member
        {
            public ProxyMember(
                User user,
                IEnumerable<Role> roles
                )
                : base(user, roles)
            {

            }
        }

        public class ProxyValueObject : ValueObject
        {
            public ProxyValueObject(
                User testUser,
                Project testProject,
                Team testTeam
                )
            {
                User = testUser;
                Project = testProject;
                Team = testTeam;
            }

            public User User { get; set; }

            public Project Project { get; set; }

            public Team Team { get; set; }
        }

        public class ProxyBaseObject : BaseObject
        {
            public ProxyBaseObject(
                User testUser,
                Project testProject,
                Team testTeam
                )
            {
                User = testUser;
                Project = testProject;
                Team = testTeam;
            }

            public User User { get; set; }

            public Project Project { get; set; }

            public Team Team { get; set; }

            protected override IEnumerable<System.Reflection.PropertyInfo> GetTypeSpecificSignatureProperties()
            {
                var invalidlyDecoratedProperties =
                this.GetType().GetProperties().Where(
                    p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));

                string message = "Properties were found within " + this.GetType() +
                                 @" having the
                [DomainSignature] attribute. The domain signature of a value object includes all
                of the properties of the object by convention; consequently, adding [DomainSignature]
                to the properties of a value object's properties is misleading and should be removed. 
                Alternatively, you can inherit from DomainModel if that fits your needs better.";

                Check.Require(
                    !invalidlyDecoratedProperties.Any(),
                    message);

                return this.GetType().GetProperties();
            }
        }

    }
}