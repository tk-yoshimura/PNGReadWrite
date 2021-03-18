using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>GDI+ビットマップから変換</summary>
        private void FromGDIBitmap(Bitmap bitmap) {
            if (bitmap == null) {
                Clear();
                throw new ArgumentNullException(nameof(bitmap));
            }

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            byte[] buf = new byte[checked(4 * bitmap.Width * bitmap.Height)];

            Marshal.Copy(data.Scan0, buf, 0, buf.Length);
            bitmap.UnlockBits(data);

            this.Pixels = FromRawPixels(buf, bitmap.Width, bitmap.Height, PNGFormat.RGBA32);

            this.Width = bitmap.Width;
            this.Height = bitmap.Height;
        }

        /// <summary>GDI+ビットマップへ変換</summary>
        /// <remarks>RGBA32で出力する</remarks>
        private Bitmap ToGDIBitmap() {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            byte[] pixels_byte = (byte[])ToRawPixels(PNGFormat.RGBA32);

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            Marshal.Copy(pixels_byte, 0, data.Scan0, pixels_byte.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        /// <summary>GDI+ビットマップから変換</summary>
        public static implicit operator PNGPixelArray(Bitmap bitmap) {
            PNGPixelArray pixelarray = new PNGPixelArray();
            pixelarray.FromGDIBitmap(bitmap);

            return pixelarray;
        }

        /// <summary>GDI+ビットマップへ変換</summary>
        /// <remarks>各ピクセルデータの下位8ビットが損失する</remarks>
        public static explicit operator Bitmap(PNGPixelArray pixelarray) {
            return pixelarray.ToGDIBitmap();
        }
    }
}
