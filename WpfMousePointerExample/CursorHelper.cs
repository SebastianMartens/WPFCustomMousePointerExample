using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32.SafeHandles;

namespace WpfMousePointerExample
{
    public class CursorHelper
    {
        private static class NativeMethods
        {
            public struct IconInfo
            {
                public bool FIcon;
                public int XHotspot;
                public int YHotspot;
                public IntPtr HbmMask;
                public IntPtr HbmColor;
            }

            [DllImport("user32.dll")]
            public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

            [DllImport("user32.dll")]
            public static extern bool DestroyIcon(IntPtr hIcon);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        private class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeIconHandle()
                : base(true)
            {
            }

            override protected bool ReleaseHandle()
            {
                return NativeMethods.DestroyIcon(handle);
            }
        }

        private static Cursor InternalCreateCursor(System.Drawing.Bitmap bmp)
        {
            var iconInfo = new NativeMethods.IconInfo();
            NativeMethods.GetIconInfo(bmp.GetHicon(), ref iconInfo);

            iconInfo.XHotspot = 0;
            iconInfo.YHotspot = 0;
            iconInfo.FIcon = false;

            var cursorHandle = NativeMethods.CreateIconIndirect(ref iconInfo);
            return CursorInteropHelper.Create(cursorHandle);
        }

        public static Cursor CreateCursor(string cursorText)
        {
            // Text to render
            var formattedText = new FormattedText(cursorText,
                    new CultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                    12.0,  // FontSize
                    Brushes.Black);

            // The Visual to use as the source of the RenderTargetBitmap.
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawText(formattedText, new Point());                        
            drawingContext.Close();

            // The BitmapSource that is rendered with a Visual.
            var rtb = new RenderTargetBitmap(
                (int)drawingVisual.ContentBounds.Width,
                (int)drawingVisual.ContentBounds.Height,
                96,   // dpiX
                96,   // dpiY
                PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            // Encoding the RenderBitmapTarget into a bitmap (as PNG)
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                using (var bmp = new System.Drawing.Bitmap(ms))
                {
                    return InternalCreateCursor(bmp);
                }
            }
        }

        public static Cursor CreateCursor(UIElement element)
        {
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(new Point(), element.DesiredSize));

            var rtb = new RenderTargetBitmap(
                (int)element.DesiredSize.Width,
                (int)element.DesiredSize.Height,
                96, 96, PixelFormats.Pbgra32);

            rtb.Render(element);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                using (var bmp = new System.Drawing.Bitmap(ms))
                {
                    return InternalCreateCursor(bmp);
                }
            }
        }
    }
}
