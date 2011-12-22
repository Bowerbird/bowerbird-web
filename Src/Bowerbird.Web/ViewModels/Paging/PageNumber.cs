
namespace Bowerbird.Web.ViewModels
{
    public class PageNumber
    {

        #region Members

        #endregion

        #region Constructors

        public PageNumber(int number, int pageStartNumber, int pageEndNumber, bool selected, string name)
        {
            Number = number;
            PageStartNumber = pageStartNumber;
            PageEndNumber = pageEndNumber;
            Selected = selected;
            Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The display page number
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The ordinal number of the first object in this page
        /// </summary>
        public int PageStartNumber { get; private set; }

        /// <summary>
        /// The ordinal number of the last object in this page
        /// </summary>
        public int PageEndNumber { get; private set; }

        /// <summary>
        /// The currently selected page, in whatever context it is being used
        /// </summary>
        public bool Selected { get; private set; }

        /// <summary>
        /// The summary name of the page
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Methods

        #endregion      

    }
}