using System;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>エッジパディング</summary>
        public PNGPixelArray EdgePadding(int left_pad, int right_pad, int top_pad, int bottom_pad) {
            if (left_pad < 0 || right_pad < 0 || top_pad < 0 || bottom_pad < 0) {
                throw new ArgumentOutOfRangeException($"{left_pad}, {right_pad}, {top_pad}, {bottom_pad}");
            }

            PNGPixelArray pixelarray = new PNGPixelArray(checked(Width + left_pad + right_pad), checked(Height + top_pad + bottom_pad));

            for (int oy = top_pad, iy = 0; iy < Height; iy++, oy++) {
                Array.Copy(Pixels, iy * Width * 4, pixelarray.Pixels, (left_pad + oy * pixelarray.Width) * 4, Width * 4);

                PNGPixel pixel = pixelarray[left_pad, oy];

                for (int ox = 0; ox < left_pad; ox++) {
                    pixelarray[ox, oy] = pixel;
                }

                pixel = pixelarray[Width + left_pad - 1, oy];

                for (int ox = Width + left_pad; ox < pixelarray.Width; ox++) {
                    pixelarray[ox, oy] = pixel;
                }
            }

            for (int oy = 0; oy < top_pad; oy++) {
                Array.Copy(pixelarray.Pixels, top_pad * pixelarray.Width * 4, pixelarray.Pixels, oy * pixelarray.Width * 4, pixelarray.Width * 4);
            }

            for (int oy = Height + top_pad; oy < pixelarray.Height; oy++) {
                Array.Copy(pixelarray.Pixels, (Height + top_pad - 1) * pixelarray.Width * 4, pixelarray.Pixels, oy * pixelarray.Width * 4, pixelarray.Width * 4);
            }

            return pixelarray;
        }

        /// <summary>ゼロパディング</summary>
        public PNGPixelArray ZeroPadding(int left_pad, int right_pad, int top_pad, int bottom_pad) {
            if (left_pad < 0 || right_pad < 0 || top_pad < 0 || bottom_pad < 0) {
                throw new ArgumentOutOfRangeException($"{left_pad}, {right_pad}, {top_pad}, {bottom_pad}");
            }

            PNGPixelArray pixelarray = new PNGPixelArray(checked(Width + left_pad + right_pad), checked(Height + top_pad + bottom_pad));

            for (int oy = top_pad, iy = 0; iy < Height; iy++, oy++) {
                Array.Copy(Pixels, iy * Width * 4, pixelarray.Pixels, (left_pad + oy * pixelarray.Width) * 4, Width * 4);
            }

            return pixelarray;
        }
    }
}
