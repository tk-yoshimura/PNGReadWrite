using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>WICビットマップから変換</summary>
        private void FromWICBitmap(BitmapSource bitmap) {
            if (bitmap == null) {
                Clear();
                throw new ArgumentNullException(nameof(bitmap));
            }

            PNGFormat format = bitmap.Format.ToPNGFormat();
            if (format == PNGFormat.Undefined) {
                Clear();
                throw new NotSupportedException("Invalid image pixel format");
            }

            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;

            switch (format) {
                case PNGFormat.RGB24: {
                    byte[] pixels = new byte[checked(3 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    this.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
                case PNGFormat.RGB48: {
                    ushort[] pixels = new ushort[checked(3 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    this.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
                case PNGFormat.RGBA32: {
                    byte[] pixels = new byte[checked(4 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    this.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
                case PNGFormat.RGBA64: {
                    ushort[] pixels = new ushort[checked(4 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    this.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
            }

            this.Metadata.DpiX = bitmap.DpiX;
            this.Metadata.DpiY = bitmap.DpiY;
            this.Width = bitmap.PixelWidth;
            this.Height = bitmap.PixelHeight;
        }

        /// <summary>WICビットマップへ変換</summary>
        private BitmapSource ToWICBitmap(PNGFormat format) {
            PixelFormat pixel_format = format.ToPixelFormat();
            Array pixels = ToRawPixels(format);

            int stride = (Width * pixel_format.BitsPerPixel + 7) / 8;
            BitmapSource bitmap = BitmapSource.Create(Width, Height, Metadata.DpiX, Metadata.DpiY, pixel_format, null, pixels, stride);

            return bitmap;
        }

        /// <summary>WICビットマップから変換</summary>
        public static implicit operator PNGPixelArray(BitmapSource bitmap) {
            PNGPixelArray pixelarray = new PNGPixelArray();
            pixelarray.FromWICBitmap(bitmap);

            return pixelarray;
        }

        /// <summary>WICビットマップへ変換</summary>
        /// <remarks>各ピクセルデータの下位8ビットが損失する</remarks>
        public static explicit operator BitmapSource(PNGPixelArray pixelarray) {
            return pixelarray.ToWICBitmap(PNGFormat.RGBA32);
        }
    }
}
