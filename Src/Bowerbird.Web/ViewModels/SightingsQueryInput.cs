/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/

namespace Bowerbird.Web.ViewModels
{
    public class SightingsQueryInput : PagingInput
    {
        #region Fields

        #endregion

        #region Constructors

        public SightingsQueryInput()
            : base()
        {
            InitMembers();
        }

        #endregion

        #region Properties

        public string Query { get; set; }

        public string Field { get; set; }

        public string Sort { get; set; }

        public string View { get; set; }

        public string Category { get; set; }

        public bool NeedsId { get; set; }

        public string Taxonomy { get; set; }

        ///// <summary>
        ///// Used in mustache views to determine whether to show search panel
        ///// </summary>
        //public bool ShowSearch { get
        //{
        //    return !string.IsNullOrWhiteSpace(Query) ||
        //           !string.IsNullOrWhiteSpace(Category) ||
        //           NeedsId ||
        //           !string.IsNullOrWhiteSpace(Taxonomy);
        //}}

        #endregion

        #region Methods

        private void InitMembers()
        {
            Query = string.Empty;
            Field = string.Empty;
            Sort = "newest";
            View = "thumbnails";
            Category = string.Empty;
            NeedsId = false;
            Taxonomy = string.Empty;
        }

        #endregion
    }
}
