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

using Bowerbird.Core.Commands;

namespace Bowerbird.Core.CommandHandlers
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }

    public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand
    {
        TResult HandleReturn(TCommand command);
    }
}
