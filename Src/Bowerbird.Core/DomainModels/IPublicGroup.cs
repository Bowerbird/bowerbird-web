using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bowerbird.Core.DomainModels
{
    /// <summary>
    /// Groups that are public facing
    /// </summary>
    public interface IPublicGroup
    {
        string Id { get; }

        string Name { get; }

        string Description { get; }

        string Website { get; }

        MediaResource Avatar { get; }

        MediaResource Background { get; }

        string GroupType { get; }
    }
}
