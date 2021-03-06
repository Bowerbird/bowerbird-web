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

namespace Bowerbird.Core.Commands
{
    public class ChatMessageCreateCommand : ICommand
    {

        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public string ChatId { get; set; }

        public string UserId { get; set; }

        public DateTime Timestamp { get; set; }

        public string MessageId { get; set; }

        public string Message { get; set; }

        #endregion

        #region Methods

        #endregion

    }
}