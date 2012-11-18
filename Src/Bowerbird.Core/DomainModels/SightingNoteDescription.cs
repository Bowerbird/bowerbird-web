/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels.DenormalisedReferences;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Bowerbird.Core.DomainModels
{
    public class SightingNoteDescription
    {
        #region Members

        #endregion

        #region Constructors

        protected SightingNoteDescription()
            : base()
        {
        }

        public SightingNoteDescription(
            string id,
            string group,
            string label,
            string description,
            string text)
            : this()
        {
            Check.RequireNotNullOrWhitespace(id, "id");
            Check.RequireNotNullOrWhitespace(group, "group");
            Check.RequireNotNullOrWhitespace(label, "label");
            Check.RequireNotNullOrWhitespace(description, "description");
            Check.RequireNotNullOrWhitespace(text, "text");

            Id = id;
            Group = group;
            Label = label;
            Description = description;
            Text = text;
        }

        #endregion

        #region Properties

        public string Id { get; private set; }

        public string Group { get; private set; }

        public string Label { get; private set; }

        public string Description { get; private set; }

        public string Text { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}