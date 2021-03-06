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

namespace Bowerbird.Core.Commands
{
    public class PostUpdateCommand : ICommand
    {
        #region Members

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string Id { get; set; }

        public string UserId { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public string PostType { get; set; }

        public IList<string> MediaResources { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}