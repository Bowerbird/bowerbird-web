namespace Bowerbird.Core.Entities.MediaResources
{
    public class OtherMediaResource : MediaResource
    {

        #region Members

        #endregion

        #region Constructors

        protected OtherMediaResource() : base() { }

        public OtherMediaResource(
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
