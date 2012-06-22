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

        MediaResource Avatar { get; }

        string GroupType { get; }
    }
}
