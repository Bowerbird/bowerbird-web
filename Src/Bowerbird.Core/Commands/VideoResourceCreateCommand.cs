/* Bowerbird V1 

 Licensed under MIT 1.1 Public License

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

namespace Bowerbird.Core.Commands
{
    public class VideoResourceCreateCommand : ICommand
    {
        public string Description { get; set; }

        public string LinkUri { get; set; }

        public string Title { get; set; }

        public DateTime UploadedOn { get; set; }

        public string Usage { get; set; }

        public string UserId { get; set; }
    }
}