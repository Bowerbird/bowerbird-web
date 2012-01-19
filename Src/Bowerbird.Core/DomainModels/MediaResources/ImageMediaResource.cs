﻿using System;
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
            User createdByUser,
            DateTime uploadedOn,
            string originalFileName,
            string fileFormat,
            string description,
            int width,
            int height) 
            : base(
            createdByUser,
            uploadedOn,
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

        public new void UpdateDetails(string description)
        {
            base.UpdateDetails(description);
        }

        private void SetDetails(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion      
    }
}