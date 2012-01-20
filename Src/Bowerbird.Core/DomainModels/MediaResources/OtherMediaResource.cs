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

using System;

namespace Bowerbird.Core.DomainModels.MediaResources
{
    /// <summary>
    /// Removed reference to width and height. Does not fit for all types of Other Media..ie sound files, pdf etc.
    /// 
    /// TODO: Resolve ambiguity of OtherMediaResource. Needs further discussion. 
    /// Ie: How to encapsulate a video/pdf/soundbyte?
    /// What similar properties do these objects share? 
    /// Should this be an abstract class?
    /// </summary>
    public class OtherMediaResource : MediaResource
    {
        #region Members

        #endregion

        #region Constructors

        protected OtherMediaResource() : base() { }

        public OtherMediaResource(User createdByUser,
            DateTime uploadedOn,
            string originalFileName,
            string fileFormat,
            string description
            ) 
            : base(
            createdByUser,
            uploadedOn,
            originalFileName,
            fileFormat,
            description) 
        {
            //SetDetails(
            //    width,
            //    height);
        }

        #endregion

        #region Properties

        //public int Width { get; private set; }

        //public int Height { get; private set; }

        #endregion

        #region Methods

        //private void SetDetails(int width, int height)
        //{
        //    Width = width;
        //    Height = height;
        //}

        #endregion      
    }
}