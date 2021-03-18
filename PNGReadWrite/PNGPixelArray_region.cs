using System;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>領域コピー</summary>
        public PNGPixelArray RegionCopy(int x, int y, int width, int height) {
            if (width < 1 || height < 1) {
                throw new ArgumentException($"{nameof(width)}, {nameof(height)}");
            }
            if (x < 0 || y < 0 || x + width > Width || y + height > Height) {
                throw new ArgumentOutOfRangeException($"{x}, {y}");
            }

            PNGPixelArray pixelarray = new PNGPixelArray(width, height);

            for (int iy = y, oy = 0; oy < height; iy++, oy++) {
                Array.Copy(Pixels, (x + iy * Width) * 4, pixelarray.Pixels, oy * width * 4, width * 4); 
            }

            return pixelarray;
        }

        /// <summary>領域上書き</summary>
        public void RegionDraw(PNGPixelArray pixelarray, int x, int y) {
            if (x < 0 || y < 0 || x + pixelarray.Width > Width || y + pixelarray.Height > Height) {
                throw new ArgumentOutOfRangeException($"{x}, {y}");
            }

            for (int oy = y, iy = 0; iy < pixelarray.Height; iy++, oy++) {
                Array.Copy(pixelarray.Pixels, iy * pixelarray.Width * 4, Pixels, (x + oy * Width) * 4, pixelarray.Width * 4); 
            }
        }
    }
}
