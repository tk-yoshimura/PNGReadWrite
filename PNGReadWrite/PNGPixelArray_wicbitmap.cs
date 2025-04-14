using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>WICビットマップから変換</summary>
        private static PNGPixelArray FromWICBitmap(BitmapSource bitmap, PNGPixelArray array) {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            PNGFormat format = bitmap.Format.ToPNGFormat();
            if (format == PNGFormat.Undefined) {
                throw new NotSupportedException("Invalid image pixel format.");
            }

            int stride = checked(bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;

            switch (format) {
                case PNGFormat.RGB24: {
                    byte[] pixels = new byte[checked(3 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    array.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
                case PNGFormat.RGB48: {
                    ushort[] pixels = new ushort[checked(3 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    array.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
                case PNGFormat.RGBA32: {
                    byte[] pixels = new byte[checked(4 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    array.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
                case PNGFormat.RGBA64: {
                    ushort[] pixels = new ushort[checked(4 * bitmap.PixelWidth * bitmap.PixelHeight)];
                    bitmap.CopyPixels(pixels, stride, 0);

                    array.Pixels = FromRawPixels(pixels, bitmap.PixelWidth, bitmap.PixelHeight, format);

                    break;
                }
            }

            array.Metadata.Dpi = (Math.Round(bitmap.DpiX * 32) / 32, Math.Round(bitmap.DpiY * 32) / 32);
            array.Width = bitmap.PixelWidth;
            array.Height = bitmap.PixelHeight;

            return array;
        }

        /// <summary>WICビットマップへ変換</summary>
        private BitmapSource ToWICBitmap(PNGFormat format) {
            PixelFormat pixel_format = format.ToPixelFormat();
            Array pixels = ToRawPixels(format);

            int stride = checked(Width * pixel_format.BitsPerPixel + 7) / 8;
            BitmapSource bitmap = BitmapSource.Create(Width, Height, Metadata.Dpi.x, Metadata.Dpi.y, pixel_format, null, pixels, stride);

            return bitmap;
        }

        /// <summary>WICビットマップから変換</summary>
        public static implicit operator PNGPixelArray(BitmapSource bitmap) {
            PNGPixelArray pixelarray = new();

            return FromWICBitmap(bitmap, pixelarray);
        }

        /// <summary>WICビットマップへ変換</summary>
        /// <remarks>各ピクセルデータの下位8ビットが損失する</remarks>
        public static explicit operator BitmapSource(PNGPixelArray pixelarray) {
            return pixelarray.ToWICBitmap(PNGFormat.RGBA32);
        }
    }
}
