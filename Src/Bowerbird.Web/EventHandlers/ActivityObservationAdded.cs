/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System.Linq;
using Bowerbird.Core.Events;
using Bowerbird.Core.DomainModels;
using Bowerbird.Core.DesignByContract;
using Bowerbird.Core.Services;
using Raven.Client;
using System.Dynamic;
using System.IO;
using Bowerbird.Core.EventHandlers;
using Bowerbird.Web.Config;
using Bowerbird.Web.Factories;
using Bowerbird.Web.Builders;
using SignalR.Hubs;
using Bowerbird.Web.Hubs;
using Bowerbird.Core.Config;

namespace Bowerbird.Web.EventHandlers
{
    /// <summary>
    /// Logs an activity item when an observation is added. The situations in which this can occur are:
    /// - A new observation is created, in which case we only add one activity item representing all groups the observation has been added to;
    /// - An observation being added to a group after the observation's creation.
    /// </summary>
    public class ActivityObservationAdded : DomainEventHandlerBase, 
        IEventHandler<DomainModelCreatedEvent<Observation>>, 
        IEventHandler<DomainModelCreatedEvent<ObservationGroup>>
    {
        #region Members

        private readonly IDocumentSession _documentSession;
        private readonly IUserViewFactory _userViewFactory;
        private readonly IUserViewModelBuilder _userViewModelBuilder;
        private readonly IUserContext _userContext;

        #endregion

        #region Constructors

        public ActivityObservationAdded(
            IDocumentSession documentSession,
            IUserViewFactory userViewFactory,
            IUserViewModelBuilder userViewModelBuilder,
            IUserContext userContext
            )
        {
            Check.RequireNotNull(documentSession, "documentSession");
            Check.RequireNotNull(userViewFactory, "userViewFactory");
            Check.RequireNotNull(userViewModelBuilder, "userViewModelBuilder");
            Check.RequireNotNull(userContext, "userContext");

            _documentSession = documentSession;
            _userViewFactory = userViewFactory;
            _userViewModelBuilder = userViewModelBuilder;
            _userContext = userContext;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Handle(DomainModelCreatedEvent<Observation> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} added an observation", domainEvent.User.GetName()), 
                domainEvent.DomainModel.Groups.Select(x => x.Group));

            activity.ObservationAdded = new
            {
                Observation = MakeObservation(domainEvent.DomainModel)
            };

            _documentSession.Store(activity);
            _userContext.SendActivityToGroupChannel(activity);
        }

        public void Handle(DomainModelCreatedEvent<ObservationGroup> domainEvent)
        {
            dynamic activity = MakeActivity(
                domainEvent, 
                "observationadded", 
                string.Format("{0} added an observation", domainEvent.User.GetName()), 
                new[] { domainEvent.DomainModel.Group });

            activity.ObservationAdded = new
            {
                Observation = MakeObservation(domainEvent.Sender as Observation)
            };

            _documentSession.Store(activity);
            _userContext.SendActivityToGroupChannel(activity);
        }

        private object MakeObservation(Observation observation)
        {
            return new
            {
                Id = observation.ShortId(),
                Title = observation.Title,
                ObservedOnDate = observation.ObservedOn.ToString("d MMM yyyy"),
                ObservedOnTime = observation.ObservedOn.ToShortTimeString(),
                Address = observation.Address,
                Latitude = observation.Latitude,
                Longitude = observation.Longitude,
                Category = observation.Category,
                IsIdentificationRequired = observation.IsIdentificationRequired,
                AnonymiseLocation = observation.AnonymiseLocation,
                Media = observation.Media, //observation.Media.Select(x => MakeObservationMediaItem(x, observation.GetPrimaryImage() == x)),
                PrimaryImage = observation.GetPrimaryImage(), //MakeObservationMediaItem(observation.GetPrimaryImage(), true),
                Projects = observation.Groups.Select(x => x.Group.Id)
            };
        }

        //private object MakeObservationMediaItem(ObservationMedia observationMedia, bool isPrimaryImage)
        //{
        //    if (observationMedia.MediaResource.Type == "image")
        //    {
        //        return new
        //        {
        //            IsPrimaryImage = isPrimaryImage,
        //            MediaResourceId = observationMedia.MediaResource.Id,
        //            observationMedia.MediaResource.Type,
        //            observationMedia.Description,
        //            observationMedia.Licence,
        //            CreatedByUser = observationMedia.MediaResource.CreatedByUser.Id,
        //            UploadedOn = observationMedia.MediaResource.UploadedOn,
        //            OriginalImage = observationMedia.MediaResource.Files["original"],
        //            LargeImage = observationMedia.MediaResource.Files["large"],
        //            MediumImage = observationMedia.MediaResource.Files["medium"],
        //            SmallImage = observationMedia.MediaResource.Files["small"],
        //            ThumbnailImage = observationMedia.MediaResource.Files["thumbnail"]
        //        };
        //    }

        //    throw new NotImplementedException();
        //}

        #endregion      
    }
}