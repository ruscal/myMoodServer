using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;


namespace Discover.Drawing
{
    /// <summary>
    /// This class is used to crop images so that any solid colour containing an image is cropped automatically in order
    /// to produce a larger actual image with less empty space around it.
    /// </summary>
    public class CroppableImage : IDisposable
    {
        #region VerticalAlignment Enumeration
        public enum VerticalAlignment
        {
            Top,
            Middle,
            Bottom
        }
        #endregion

        #region Private Fields
        private Double _aspectRatio;
        private Bitmap _bitMap;
        private Bitmap _croppedImage;
        private string _imagePath;
        private bool _maintainAspectRatio;
        private int _topRowsToCrop;
        private int _bottomRowsToCrop;
        private int _leftColumnsToCrop;
        private int _rightColumnsToCrop;
        private string _outPutLog;
        private int _originalWidth;
        private int _originalHeight;
        private int _croppedWidth;
        private int _croppedHeight;
        private bool _drawAsciiGraph;
        private VerticalAlignment _imageVerticalAlignment;
        private IEnumerable<Color> _coloursToCrop;
        private Color _currentStartingPixelColour;
        private Color _backgroundColour;
        private Color _topLeftPixelColour;
        #endregion

        #region Public Properties
        public string OutputLog { get { return _outPutLog; } }

        public int TopRowsToCrop { get { return _topRowsToCrop; } }
        public int BottomRowsToCrop { get { return _bottomRowsToCrop; } }
        public int LeftColumnsToCrop { get { return _leftColumnsToCrop; } }
        public int RightColumnsToCrop { get { return _rightColumnsToCrop; } }

        public int OriginalWidth { get { return _originalWidth; } }
        public int OriginalHeight { get { return _originalHeight; } }
        public int CroppedWidth { get { return _croppedWidth; } }
        public int CroppedHeight { get { return _croppedHeight; } }

        public VerticalAlignment ImageVerticalAlignment { get { return _imageVerticalAlignment; } }
        #endregion

        #region File Path Based Constructors
        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">The path to the image - eg c:\myfolder\myfile.txt</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="backgroundColour">The background colour to use for the new canvas</param>
        /// <param name="aspectRatio">The aspect ration eg 1.6666667</param>
        public CroppableImage(string imagePath, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, Color backgroundColour, Double aspectRatio) :

            this(imagePath, maintainAspectRatio, drawAsciiGraph, coloursToCrop, verticalAlignment, aspectRatio)
        {
            _backgroundColour = backgroundColour;
        }

        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">The path to the image - eg c:\myfolder\myfile.txt</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="backgroundColour">The background colour to use for the new canvas</param>
        /// <param name="aspectRatioWidth">The aspect ratio width</param>        
        /// <param name="aspectRatioHeight">The aspect ratio height</param>        
        public CroppableImage(string imagePath, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, Color backgroundColour, int aspectRatioWidth, int aspectRatioHeight) :

            this(imagePath, maintainAspectRatio, drawAsciiGraph, coloursToCrop, verticalAlignment, (double)aspectRatioWidth / (double)aspectRatioHeight)
        {
            _backgroundColour = backgroundColour;
        }

        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">The path to the image - eg c:\myfolder\myfile.txt</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="aspectRatioWidth">The aspect ratio width</param>        
        /// <param name="aspectRatioHeight">The aspect ratio height</param>             
        public CroppableImage(string imagePath, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, int aspectRatioWidth, int aspectRatioHeight) :

            this(imagePath, maintainAspectRatio, drawAsciiGraph, coloursToCrop, verticalAlignment, (double)aspectRatioWidth / (double)aspectRatioHeight)
        {

        }

        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">The path to the image - eg c:\myfolder\myfile.txt</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="aspectRatio">The aspect ration eg 1.6666667</param>         
        public CroppableImage(string imagePath, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, Double aspectRatio)
        {
            _imagePath = imagePath;
            _drawAsciiGraph = drawAsciiGraph;
            _maintainAspectRatio = maintainAspectRatio;

            _bitMap = new Bitmap(_imagePath);

            _originalWidth = _bitMap.Width;
            _originalHeight = _bitMap.Height;

            _coloursToCrop = coloursToCrop;
            _aspectRatio = aspectRatio;
            _imageVerticalAlignment = verticalAlignment;
        }
        #endregion

        #region Byte Array based Constructors
        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">An array of bytes representing the image</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="backgroundColour">The background colour to use for the new canvas</param>
        /// <param name="aspectRatio">The aspect ration eg 1.6666667</param>
        public CroppableImage(byte[] byteArray, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, Color backgroundColour, Double aspectRatio) :

            this(byteArray, maintainAspectRatio, drawAsciiGraph, coloursToCrop, verticalAlignment, aspectRatio)
        {
            _backgroundColour = backgroundColour;
        }

        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">An array of bytes representing the image</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="backgroundColour">The background colour to use for the new canvas</param>
        /// <param name="aspectRatioWidth">The aspect ratio width</param>        
        /// <param name="aspectRatioHeight">The aspect ratio height</param>        
        public CroppableImage(byte[] byteArray, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, Color backgroundColour, int aspectRatioWidth, int aspectRatioHeight) :

            this(byteArray, maintainAspectRatio, drawAsciiGraph, coloursToCrop, verticalAlignment, (double)aspectRatioWidth / (double)aspectRatioHeight)
        {
            _backgroundColour = backgroundColour;
        }

        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">An array of bytes representing the image</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="aspectRatioWidth">The aspect ratio width</param>        
        /// <param name="aspectRatioHeight">The aspect ratio height</param>          
        public CroppableImage(byte[] byteArray, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, int aspectRatioWidth, int aspectRatioHeight) :

            this(byteArray, maintainAspectRatio, drawAsciiGraph, coloursToCrop, verticalAlignment, (double)aspectRatioWidth / (double)aspectRatioHeight)
        {

        }

        /// <summary>
        /// Class constructor - one of several overloads
        /// </summary>
        /// <param name="imagePath">An array of bytes representing the image</param>
        /// <param name="maintainAspectRatio">A true or false value to indicate if you want to keep to a specific aspect ratio</param>
        /// <param name="drawAsciiGraph">Used for debugging - populates a property with an ascii art representation of the image</param>
        /// <param name="coloursToCrop">An IEnumerable collection of colours to crop by - only the first match will be used though</param>
        /// <param name="verticalAlignment">How to align the image within the new canvas</param>
        /// <param name="aspectRatio">The aspect ration eg 1.6666667</param>
        public CroppableImage(byte[] byteArray, bool maintainAspectRatio, bool drawAsciiGraph, IEnumerable<Color> coloursToCrop, VerticalAlignment verticalAlignment, Double aspectRatio)
        {
            _drawAsciiGraph = drawAsciiGraph;
            _maintainAspectRatio = maintainAspectRatio;

            _bitMap = BytesToBitmap(byteArray);

            _originalWidth = _bitMap.Width;
            _originalHeight = _bitMap.Height;

            _coloursToCrop = coloursToCrop;
            _aspectRatio = aspectRatio;
            _imageVerticalAlignment = verticalAlignment;
        }
        #endregion

        #region IsPixelCroppable
        private bool IsPixelCroppable(Color pixelColour)
        {
            foreach (Color c in _coloursToCrop)
            {
                if (AreColoursEqual(pixelColour, c) && AreColoursEqual(_currentStartingPixelColour, pixelColour) && AreColoursEqual(pixelColour, _topLeftPixelColour))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region AreColoursEqual
        public static bool AreColoursEqual(Color a, Color b)
        {
            return (a.R == b.R && a.G == b.G && a.B == b.B);
        }
        #endregion

        #region MaintainAspectRatio
        private void MaintainAspectRatio()
        {
            double canvasWidth;
            double canvasHeight;

            canvasWidth = _croppedWidth + 4;
            canvasHeight = canvasWidth * _aspectRatio;

            if (canvasWidth < _croppedWidth || canvasHeight < _croppedHeight)
            {
                canvasHeight = _croppedHeight + 4;
                canvasWidth = canvasHeight / _aspectRatio;
            }

            Bitmap canvas = new Bitmap(Convert.ToInt32(canvasWidth), Convert.ToInt32(canvasHeight));

            int leftMargin = Convert.ToInt32((canvasWidth - _croppedWidth) / 2);
            int topMargin;

            if (_imageVerticalAlignment == CroppableImage.VerticalAlignment.Top)
            {
                topMargin = 2;
            }
            else if (_imageVerticalAlignment == CroppableImage.VerticalAlignment.Middle)
            {
                topMargin = Convert.ToInt32((canvasHeight - _croppedHeight) / 2);
            }
            else
            {
                topMargin = Convert.ToInt32((canvasHeight - _croppedHeight - 2));
            }

            using (Graphics gfx = Graphics.FromImage(canvas))
            {
                if (_backgroundColour.IsEmpty)
                {
                    _backgroundColour = _topLeftPixelColour;
                }

                using (SolidBrush brush = new SolidBrush(_backgroundColour))
                {
                    gfx.FillRectangle(brush, 0, 0, Convert.ToInt32(canvasWidth), Convert.ToInt32(canvasHeight));
                }

                gfx.DrawImage(_croppedImage, leftMargin, topMargin, _croppedImage.Width, _croppedImage.Height);
            }

            _croppedHeight = canvas.Height;
            _croppedWidth = canvas.Width;

            _croppedImage = canvas;
        }
        #endregion

        #region AnalysePixels
        private void AnalysePixels()
        {
            StringBuilder outputLog = new StringBuilder();

            bool allSame;

            _topLeftPixelColour = _bitMap.GetPixel(0, 0);

            //Work out the number of rows to crop at the top
            for (int h = 0; h < _bitMap.Height; h++)
            {
                allSame = true;

                _currentStartingPixelColour = _bitMap.GetPixel(0, h);

                for (int w = 0; w < _bitMap.Width; w++)
                {
                    var pixelColour = _bitMap.GetPixel(w, h);

                    if (!IsPixelCroppable(pixelColour))
                    {
                        allSame = false;

                        break;
                    }
                }

                if (allSame)
                {
                    _topRowsToCrop++;
                }
                else
                {
                    break;
                }
            }

            //Work out the number of rows to crop at the bottom
            for (int h = _bitMap.Height - 1; h > 0; h--)
            {
                allSame = true;

                _currentStartingPixelColour = _bitMap.GetPixel(0, h);

                for (int w = 0; w < _bitMap.Width; w++)
                {
                    var pixelColour = _bitMap.GetPixel(w, h);

                    if (!IsPixelCroppable(pixelColour))
                    {
                        allSame = false;

                        break;
                    }
                }

                if (allSame)
                {
                    _bottomRowsToCrop++;
                }
                else
                {
                    break;
                }
            }

            //Work out the number of columns to crop at the left
            for (int w = 0; w < _bitMap.Width; w++)
            {
                allSame = true;

                _currentStartingPixelColour = _bitMap.GetPixel(w, 0);

                for (int h = 0; h < _bitMap.Height; h++)
                {
                    var pixelColour = _bitMap.GetPixel(w, h);

                    if (!IsPixelCroppable(pixelColour))
                    {
                        allSame = false;

                        break;
                    }
                }

                if (allSame)
                {
                    _leftColumnsToCrop++;
                }
                else
                {
                    break;
                }
            }

            //Work out the number of columns to crop at the right
            for (int w = _bitMap.Width - 1; w > 0; w--)
            {
                allSame = true;

                _currentStartingPixelColour = _bitMap.GetPixel(w, 0);

                for (int h = 0; h < _bitMap.Height; h++)
                {
                    var pixelColour = _bitMap.GetPixel(w, h);

                    if (!IsPixelCroppable(pixelColour))
                    {
                        allSame = false;

                        break;
                    }
                }

                if (allSame)
                {
                    _rightColumnsToCrop++;
                }
                else
                {
                    break;
                }
            }
        }
        #endregion

        #region Crop
        public Bitmap Crop()
        {
            if (_drawAsciiGraph)
            {
                CreateAsciiMap();
            }

            AnalysePixels();

            _croppedImage = CropBitmap(_leftColumnsToCrop, _topRowsToCrop, (_bitMap.Width - _leftColumnsToCrop - _rightColumnsToCrop), (_bitMap.Height - _topRowsToCrop - _bottomRowsToCrop));

            _croppedWidth = _croppedImage.Width;
            _croppedHeight = _croppedImage.Height;

            if (_maintainAspectRatio)
            {
                MaintainAspectRatio();
            }

            return _croppedImage;
        }
        #endregion

        #region CropBitmap
        private Bitmap CropBitmap(int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);

            Bitmap cropped = _bitMap.Clone(rect, _bitMap.PixelFormat);

            return cropped;
        }
        #endregion

        #region BytesToBitmap
        private Bitmap BytesToBitmap(byte[] byteArray)
        {
            Bitmap img;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                img = (Bitmap)Image.FromStream(ms);
            }

            return img;
        }
        #endregion

        #region CreateAsciiMap
        private void CreateAsciiMap()
        {
            StringBuilder outputLog = new StringBuilder();

            bool allSame;

            for (int h = 0; h < _bitMap.Height; h++)
            {
                allSame = true;

                _currentStartingPixelColour = _bitMap.GetPixel(h, 0);

                for (int w = 0; w < _bitMap.Width; w++)
                {
                    var pixelColour = _bitMap.GetPixel(w, h);

                    if (IsPixelCroppable(pixelColour))
                    {
                        outputLog.Append("|");
                    }
                    else
                    {
                        allSame = false;

                        outputLog.Append("-");
                    }
                }

                if (allSame)
                {
                    outputLog.Append(" Solid");
                }
                else
                {
                    outputLog.Append(" Not Solid");
                }

                outputLog.AppendLine();
            }

            _outPutLog = outputLog.ToString();
        }
        #endregion

        public void Dispose()
        {
            _bitMap.Dispose();
        }
    }
}
