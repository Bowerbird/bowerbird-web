///* Bowerbird V1 - Licensed under MIT 1.1 Public License

// Developers: 
// * Frank Radocaj : frank@radocaj.com
// * Hamish Crittenden : hamish.crittenden@gmail.com
 
// Project Manager: 
// * Ken Walker : kwalker@museum.vic.gov.au
 
// Funded by:
// * Atlas of Living Australia
 
//*/
				
//using Bowerbird.Core.DomainModels;
//using Bowerbird.Core.DesignByContract;

//namespace Bowerbird.Web.Factories
//{
//    public class ObservationNoteViewFactory : IObservationNoteViewFactory
//    {
//        #region Members

//        private readonly IUserViewFactory _userViewFactory;

//        #endregion

//        #region Constructors

//        public ObservationNoteViewFactory(
//            IUserViewFactory userViewFactory
//            )
//        {
//            Check.RequireNotNull(userViewFactory, "userViewFactory");

//            _userViewFactory = userViewFactory;
//        }

//        #endregion

//        #region Properties

//        #endregion

//        #region Methods

//        public object Make(ObservationNote observationNote)
//        {
//            return new 
//            {
//                observationNote.Id,
//                ObservationId = observationNote.Observation.Id,
//                observationNote.CreatedOn,
//                observationNote.CommonName,
//                observationNote.ScientificName,
//                observationNote.Taxonomy,
//                observationNote.Descriptions,
//                observationNote.References,
//                observationNote.Tags,
//                CreatedBy = _userViewFactory.Make(observationNote.User.Id)
//            };
//        }

//        #endregion      
//    }
//}