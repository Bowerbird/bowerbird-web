/* Bowerbird V1 - Licensed under MIT 1.1 Public License

 Developers: 
 * Frank Radocaj : frank@radocaj.com
 * Hamish Crittenden : hamish.crittenden@gmail.com
 
 Project Manager: 
 * Ken Walker : kwalker@museum.vic.gov.au
 
 Funded by:
 * Atlas of Living Australia
 
*/
				
using System;

namespace Bowerbird.Core.Utilities
{
    public class ImageDimensions
    {

        #region Fields

        public enum OrientationType { Portrait, Landscape }

        #endregion

        #region Constructors

        public ImageDimensions(
            int width,
            int height)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Properties

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public double Proportion
        {
            get
            {
                if (Width > Height)
                {
                    return Convert.ToDouble(Height) / Convert.ToDouble(Width);
                }
                else
                {
                    return Convert.ToDouble(Width) / Convert.ToDouble(Height);
                }
            }
        }

        public OrientationType Orientation
        {
            get
            {
                if (Width > Height)
                {
                    return OrientationType.Landscape;
                }
                else
                {
                    return OrientationType.Portrait;
                }
            }
        }

        #endregion

        #region Methods

        #endregion

    }
}
