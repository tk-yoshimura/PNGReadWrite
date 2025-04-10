namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>領域コピー</summary>
        public PNGPixelArray RegionCopy(int x, int y, int width, int height) {
            if (width < 1 || height < 1) {
                throw new ArgumentException(
                    "The specified size is invalid.",
                    $"{nameof(width)},{nameof(height)}"
                    );
            }
            if (x < 0 || y < 0 || x + width > Width || y + height > Height) {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(x)},{nameof(y)}",
                    "The specified coordinates is out of bounds."
                    );
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int iy = y, oy = 0; oy < height; iy++, oy++) {
                Array.Copy(Pixels, (x + iy * Width) * 4, pixelarray.Pixels, oy * width * 4, width * 4);
            }

            return pixelarray;
        }

        /// <summary>領域上書き</summary>
        public void RegionOverwrite(PNGPixelArray pixelarray, int x, int y) {
            if (x < 0 || y < 0 || x + pixelarray.Width > Width || y + pixelarray.Height > Height) {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(x)},{nameof(y)}",
                    "The specified coordinates is out of bounds."
                    );
            }

            for (int oy = y, iy = 0; iy < pixelarray.Height; iy++, oy++) {
                Array.Copy(pixelarray.Pixels, iy * pixelarray.Width * 4, Pixels, (x + oy * Width) * 4, pixelarray.Width * 4);
            }
        }

        /// <summary>領域インデクサ</summary>
        /// <param name="x_range">x軸範囲</param>
        /// <param name="y_range">y軸範囲</param>
        public PNGPixelArray this[Range x_range, Range y_range] {
            get {
                int x, y, width, height;

                try {
                    (x, width) = x_range.GetOffsetAndLength(Width);
                    (y, height) = y_range.GetOffsetAndLength(Height);
                }
                catch (ArgumentOutOfRangeException) {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(x_range)},{nameof(y_range)}",
                        "The specified coordinates is out of bounds."
                        );
                }

                return RegionCopy(x, y, width, height);
            }
            set {
                int x, y, width, height;

                try {
                    (x, width) = x_range.GetOffsetAndLength(Width);
                    (y, height) = y_range.GetOffsetAndLength(Height);
                }
                catch (ArgumentOutOfRangeException) {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(x_range)},{nameof(y_range)}",
                        "The specified coordinates is out of bounds."
                        );
                }

                if ((width, height) != value.Size) {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(x_range)},{nameof(y_range)}",
                        "Mismatch size."
                        );
                }

                RegionOverwrite(value, x, y);
            }
        }
    }
}