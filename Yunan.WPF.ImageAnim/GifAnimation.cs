using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

using System.Windows.Threading;
using System.Runtime.CompilerServices;

namespace Yunan.WPF.ImageAnim
{
    class GifAnimation : Viewbox
    {
        private class GifFrame : Image
        {
            public int delayTime;
            public int disposalMethod;
            public int left;
            public int top;
            public int width;
            public int height;
        }

        // Gif Animation Fields
        private Canvas _canvas = null;

        private List<GifFrame> _frameList = null;

        private int _frameCounter = 0;
        private int _numberOfFrames = 0;

        private int _numberOfLoops = -1;
        private int _currentLoop = 0;

        private int _logicalWidth = 0;
        private int _logicalHeight = 0;

        private Geometry totalTransparentGeometry;

        private GifFrame _currentParseGifFrame;

        private WeakContainer _weakContainer = null;

        private DispatcherTimer _frameTimer = null;

        private DispatcherTimer frameTimer
        {
            get
            {
                return _frameTimer;
            }

            set
            {
                if (_weakContainer == null)
                {
                    _weakContainer = new WeakContainer(this);
                }

                // unsubscribe old
                if (_frameTimer != null)
                {
                    _frameTimer.Tick -= new EventHandler(_weakContainer.NextFrame);
                }

                // subscribe new
                _frameTimer = value;
                if (_frameTimer != null)
                {
                    _frameTimer.Tick += new EventHandler(_weakContainer.NextFrame);
                }
            }
        }

        // Greg Schechter's Weak Delegate Implmentation (thanks Greg! google it for more info)
        // Container = GifAnimation
        // Containee = DispatchTimer
        // Handler = NextFrame
        private class WeakContainer : WeakReference
        {
            public WeakContainer(GifAnimation target) : base(target) { }

            public void NextFrame(object sender, EventArgs args)
            {
                GifAnimation b = (GifAnimation)this.Target;
                if (b != null)
                {
                    b.NextFrame(sender, args);
                }
                else
                {
                    DispatcherTimer c = sender as DispatcherTimer;
                    if (c != null)
                    {
                        c.Tick -= new EventHandler(this.NextFrame);
                    }
                }
            }

            ~WeakContainer()
            {
            }
        }

        public GifAnimation()
        {
            _canvas = new Canvas();
            this.Child = _canvas;
        }

        private void Reset()
        {
            if (_frameList != null)
            {
                _frameList.Clear();
            }
            _frameList = null;
            _frameCounter = 0;
            _numberOfFrames = 0;
            _numberOfLoops = -1;
            _currentLoop = 0;
            _logicalWidth = 0;
            _logicalHeight = 0;
            if (frameTimer != null)
            {
                frameTimer.Stop();
                frameTimer = null;
            }
        }

        private void ParseGif(byte[] gifData)
        {
            _frameList = new List<GifFrame>();
            _currentParseGifFrame = new GifFrame();
            ParseGifDataStream(gifData, 0);
        }

        private int ParseBlock(byte[] gifData, int offset)
        {
            switch (gifData[offset])
            {
                case 0x21:
                    if (gifData[offset + 1] == 0xF9)
                    {
                        return ParseGraphicControlExtension(gifData, offset);
                    }
                    else
                    {
                        return ParseExtensionBlock(gifData, offset);
                    }
                case 0x2C:
                    offset = ParseGraphicBlock(gifData, offset);
                    _frameList.Add(_currentParseGifFrame);
                    _currentParseGifFrame = new GifFrame();
                    return offset;
                case 0x3B:
                    return -1;
                default:
                    throw new Exception("GIF format incorrect: missing graphic block or special-purpose block. ");
            }
        }

        private int ParseGraphicControlExtension(byte[] gifData, int offset)
        {
            int returnOffset = offset;
            // Extension Block
            int length = gifData[offset + 2];
            returnOffset = offset + length + 2 + 1;

            byte packedField = gifData[offset + 3];
            _currentParseGifFrame.disposalMethod = (packedField & 0x1C) >> 2;

            // Get DelayTime
            int delay = BitConverter.ToUInt16(gifData, offset + 4);
            _currentParseGifFrame.delayTime = delay;
            while (gifData[returnOffset] != 0x00)
            {
                returnOffset = returnOffset + gifData[returnOffset] + 1;
            }

            returnOffset++;

            return returnOffset;
        }

        private int ParseLogicalScreen(byte[] gifData, int offset)
        {
            _logicalWidth = BitConverter.ToUInt16(gifData, offset);
            _logicalHeight = BitConverter.ToUInt16(gifData, offset + 2);

            byte packedField = gifData[offset + 4];
            bool hasGlobalColorTable = (int)(packedField & 0x80) > 0 ? true : false;

            int currentIndex = offset + 7;
            if (hasGlobalColorTable)
            {
                int colorTableLength = packedField & 0x07;
                colorTableLength = (int)Math.Pow(2, colorTableLength + 1) * 3;
                currentIndex = currentIndex + colorTableLength;
            }
            return currentIndex;
        }

        private int ParseGraphicBlock(byte[] gifData, int offset)
        {
            _currentParseGifFrame.left = BitConverter.ToUInt16(gifData, offset + 1);
            _currentParseGifFrame.top = BitConverter.ToUInt16(gifData, offset + 3);
            _currentParseGifFrame.width = BitConverter.ToUInt16(gifData, offset + 5);
            _currentParseGifFrame.height = BitConverter.ToUInt16(gifData, offset + 7);
            if (_currentParseGifFrame.width > _logicalWidth)
            {
                _logicalWidth = _currentParseGifFrame.width;
            }
            if (_currentParseGifFrame.height > _logicalHeight)
            {
                _logicalHeight = _currentParseGifFrame.height;
            }
            byte packedField = gifData[offset + 9];
            bool hasLocalColorTable = (int)(packedField & 0x80) > 0 ? true : false;

            int currentIndex = offset + 9;
            if (hasLocalColorTable)
            {
                int colorTableLength = packedField & 0x07;
                colorTableLength = (int)Math.Pow(2, colorTableLength + 1) * 3;
                currentIndex = currentIndex + colorTableLength;
            }
            currentIndex++; // Skip 0x00

            currentIndex++; // Skip LZW Minimum Code Size;

            while (gifData[currentIndex] != 0x00)
            {
                int length = gifData[currentIndex];
                currentIndex = currentIndex + gifData[currentIndex];
                currentIndex++; // Skip initial size byte
            }
            currentIndex = currentIndex + 1;
            return currentIndex;
        }

        private int ParseExtensionBlock(byte[] gifData, int offset)
        {
            int returnOffset = offset;
            // Extension Block
            int length = gifData[offset + 2];
            returnOffset = offset + length + 2 + 1;
            // check if netscape continousLoop extension
            if (gifData[offset + 1] == 0xFF && length > 10)
            {
                string netscape = System.Text.ASCIIEncoding.ASCII.GetString(gifData, offset + 3, 8);
                if (netscape == "NETSCAPE")
                {
                    _numberOfLoops = BitConverter.ToUInt16(gifData, offset + 16);
                    if (_numberOfLoops > 0)
                    {
                        _numberOfLoops++;
                    }
                }
            }
            while (gifData[returnOffset] != 0x00)
            {
                returnOffset = returnOffset + gifData[returnOffset] + 1;
            }

            returnOffset++;

            return returnOffset;
        }

        private int ParseHeader(byte[] gifData, int offset)
        {
            string str = System.Text.ASCIIEncoding.ASCII.GetString(gifData, offset, 3);
            if (str != "GIF")
            {
                throw new Exception("Not a proper GIF file: missing GIF header");
            }
            return 6;
        }

        private void ParseGifDataStream(byte[] gifData, int offset)
        {
            offset = ParseHeader(gifData, offset);
            offset = ParseLogicalScreen(gifData, offset);
            while (offset != -1)
            {
                offset = ParseBlock(gifData, offset);
            }
        }

        public void CreateGifAnimation(MemoryStream memoryStream)
        {
            Reset();

            byte[] gifData = memoryStream.GetBuffer();  // Use GetBuffer so that there is no memory copy

            GifBitmapDecoder decoder = new GifBitmapDecoder(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

            _numberOfFrames = decoder.Frames.Count;

            try
            {
                ParseGif(gifData);
            }
            catch
            {
                throw new FileFormatException("Unable to parse Gif file format.");
            }

            for (int f = 0; f < decoder.Frames.Count; f++)
            {
                _frameList[f].Source = decoder.Frames[f];
                _frameList[f].Visibility = Visibility.Hidden;
                _canvas.Children.Add(_frameList[f]);
                Canvas.SetLeft(_frameList[f], _frameList[f].left);
                Canvas.SetTop(_frameList[f], _frameList[f].top);
                Canvas.SetZIndex(_frameList[f], f);
            }
            _canvas.Height = _logicalHeight;
            _canvas.Width = _logicalWidth;

            _frameList[0].Visibility = Visibility.Visible;

            for (int i = 0; i < _frameList.Count; i++)
            {
                Console.WriteLine(_frameList[i].disposalMethod.ToString() + " " + _frameList[i].width.ToString() + " " + _frameList[i].delayTime.ToString());
            }

            if (_frameList.Count > 1)
            {
                if (_numberOfLoops == -1)
                {
                    _numberOfLoops = 1;
                }

                frameTimer = new System.Windows.Threading.DispatcherTimer();
                frameTimer.Interval = new TimeSpan(0, 0, 0, 0, _frameList[0].delayTime * 10);
                frameTimer.Start();
            }
        }

        public void NextFrame()
        {
            NextFrame(null, null);
        }

        public void NextFrame(object sender, EventArgs e)
        {
            frameTimer.Stop();
            if (_numberOfFrames == 0) return;
            if (_frameList[_frameCounter].disposalMethod == 2)
            {
                // dispose = background, tricky code to make transparent last frames region
                for (int fc = 0; fc < _frameCounter; fc++)
                {
                    if (_frameList[fc].Visibility == Visibility.Visible)
                    {
                        GifFrame gf = _frameList[_frameCounter];
                        RectangleGeometry rg2 = new RectangleGeometry(new Rect(gf.left, gf.top, gf.width, gf.height));
                        totalTransparentGeometry = new CombinedGeometry(GeometryCombineMode.Union, totalTransparentGeometry, rg2);

                        GifFrame gfBack = _frameList[fc];
                        RectangleGeometry rgBack = new RectangleGeometry(new Rect(gfBack.left, gfBack.top, gfBack.width, gfBack.height));
                        CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Exclude, rgBack, totalTransparentGeometry);
                        GeometryDrawing gd = new GeometryDrawing(Brushes.Black, new Pen(Brushes.Black, 0), cg);
                        DrawingBrush db = new DrawingBrush(gd);
                        _frameList[fc].OpacityMask = db;
                    }
                }
                
                _frameList[_frameCounter].Visibility = Visibility.Hidden;
            }
            if (_frameList[_frameCounter].disposalMethod >= 3)
            {
                _frameList[_frameCounter].Visibility = Visibility.Hidden;
            }
            _frameCounter++;

            if (_frameCounter < _numberOfFrames)
            {
                _frameList[_frameCounter].Visibility = Visibility.Visible;
                frameTimer.Interval = new TimeSpan(0, 0, 0, 0, _frameList[_frameCounter].delayTime * 10);
                frameTimer.Start();
            }
            else
            {
                if (_numberOfLoops != 0)
                {
                    _currentLoop++;
                }
                if (_currentLoop < _numberOfLoops || _numberOfLoops == 0)
                {
                    for (int f = 0; f < _frameList.Count; f++)
                    {
                        _frameList[f].Visibility = Visibility.Hidden;
                        _frameList[f].OpacityMask = null;
                    }
                    totalTransparentGeometry = null;
                    _frameCounter = 0;
                    _frameList[_frameCounter].Visibility = Visibility.Visible;
                    frameTimer.Interval = new TimeSpan(0, 0, 0, 0, _frameList[_frameCounter].delayTime * 10);
                    frameTimer.Start();
                }
            }
        }
    }
}
