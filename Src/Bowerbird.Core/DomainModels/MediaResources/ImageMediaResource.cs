using Bowerbird.Core.DesignByContract;

namespace Bowerbird.Core.DomainModels.MediaResources
{
    public class ImageMediaResource : MediaResource
    {
        #region Members

        #endregion

        #region Constructors

        protected ImageMediaResource() : base() { }

        public ImageMediaResource(
            string originalFileName,
            string fileFormat,
            string description,
            int width,
            int height) 
            : base(
            originalFileName,
            fileFormat,
            description)
        {
            Check.RequireGreaterThanZero(width, "width");
            Check.RequireGreaterThanZero(height, "height");

            SetDetails(
                width,
                height);
        }

        #endregion

        #region Properties

        public int Width { get; private set; }

        public int Height { get; private set; }

        #endregion

        #region Methods

        private void SetDetails(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion      
    }
}