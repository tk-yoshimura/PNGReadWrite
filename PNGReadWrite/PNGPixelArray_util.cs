namespace PNGReadWrite {

    /// <summary>ユーティリティ</summary>
    public partial class PNGPixelArray {

        /// <summary>アルファ値が0のピクセルに仮色を設定する</summary>
        /// <param name="pixelarray">ピクセルデータ</param>
        /// <param name="expands">アルファ値が非0のピクセルからの距離</param>
        public static PNGPixelArray FillZeroAlphaPixels(PNGPixelArray pixelarray, int expands = 16) {
            ArgumentNullException.ThrowIfNull(pixelarray);

            pixelarray = pixelarray.Copy();

            int w = pixelarray.Width, h = pixelarray.Height;

            int generation = 0;
            bool updated = true;
            int[,] generation_table = new int[w, h];

            for (int x, y = 0; y < h; y++) {
                for (x = 0; x < w; x++) {
                    generation_table[x, y] = (pixelarray[x, y].A > 0) ? -1 : int.MaxValue;
                }
            }

            while (updated && (generation < expands || expands < 0)) {
                updated = false;

                for (int x, y = 0; y < h; y++) {
                    for (x = 0; x < w; x++) {
                        if (generation_table[x, y] < generation) {
                            continue;
                        }

                        int r = 0, g = 0, b = 0, n = 0;

                        void add_color(PNGPixel cr) {
                            r += cr.R; g += cr.G; b += cr.B;
                            n++;
                        }

                        if (x >= 1 && generation_table[x - 1, y] < generation) {
                            add_color(pixelarray[x - 1, y]);
                        }
                        if (x < w - 1 && generation_table[x + 1, y] < generation) {
                            add_color(pixelarray[x + 1, y]);
                        }
                        if (y >= 1 && generation_table[x, y - 1] < generation) {
                            add_color(pixelarray[x, y - 1]);
                        }
                        if (y < h - 1 && generation_table[x, y + 1] < generation) {
                            add_color(pixelarray[x, y + 1]);
                        }

                        if (n > 0) {
                            pixelarray[x, y] = new PNGPixel((ushort)(r / n), (ushort)(g / n), (ushort)(b / n), 0);
                            updated = true;
                            generation_table[x, y] = generation;
                        }
                    }
                }

                generation++;
            }

            return pixelarray;
        }

        /// <summary>ピクセル毎に関数適用</summary>
        /// <param name="pixelarray">ピクセルデータ</param>
        /// <param name="transform_func">合成関数</param>
        public static PNGPixelArray Transform(PNGPixelArray pixelarray, Func<PNGPixel, PNGPixel> transform_func) {
            ArgumentNullException.ThrowIfNull(pixelarray);
            ArgumentNullException.ThrowIfNull(transform_func);

            PNGPixelArray png = new(pixelarray.Size);

            for (int i = 0, length = png.PixelCounts; i < length; i++) {
                png[i] = transform_func(pixelarray[i]);
            }

            return png;
        }

        public static int Count(PNGPixelArray pixelarray, Func<PNGPixel, bool> condition) {
            ArgumentNullException.ThrowIfNull(pixelarray);
            ArgumentNullException.ThrowIfNull(condition);

            int count = 0;

            foreach (PNGPixel pixel in pixelarray) {
                if (condition(pixel)) {
                    count++;
                }
            }

            return count;
        }

        public static bool Any(PNGPixelArray pixelarray, Func<PNGPixel, bool> condition) {
            ArgumentNullException.ThrowIfNull(pixelarray);
            ArgumentNullException.ThrowIfNull(condition);

            foreach (PNGPixel pixel in pixelarray) {
                if (condition(pixel)) {
                    return true;
                }
            }

            return false;
        }

        public static bool All(PNGPixelArray pixelarray, Func<PNGPixel, bool> condition) {
            ArgumentNullException.ThrowIfNull(pixelarray);
            ArgumentNullException.ThrowIfNull(condition);

            foreach (PNGPixel pixel in pixelarray) {
                if (!condition(pixel)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>PNGを合成する</summary>
        /// <param name="pixelarray1">ピクセルデータ1</param>
        /// <param name="pixelarray2">ピクセルデータ2</param>
        /// <param name="blend_func">合成関数</param>
        public static PNGPixelArray Blend(PNGPixelArray pixelarray1, PNGPixelArray pixelarray2, Func<PNGPixel, PNGPixel, PNGPixel> blend_func) {
            ArgumentNullException.ThrowIfNull(pixelarray1);
            ArgumentNullException.ThrowIfNull(pixelarray2);
            ArgumentNullException.ThrowIfNull(blend_func);

            if (pixelarray1.Size != pixelarray2.Size) {
                throw new ArgumentException("Mismatch array size.", $"{nameof(pixelarray1)}, {nameof(pixelarray2)}");
            }

            PNGPixelArray png = new(pixelarray1.Size);

            for (int i = 0, length = png.PixelCounts; i < length; i++) {
                png[i] = blend_func(pixelarray1[i], pixelarray2[i]);
            }

            return png;
        }

        /// <summary>PNGを加算合成する</summary>
        /// <param name="pixelarray1">ピクセルデータ1</param>
        /// <param name="pixelarray2">ピクセルデータ2</param>
        public static PNGPixelArray operator +(PNGPixelArray pixelarray1, PNGPixelArray pixelarray2) {
            ArgumentNullException.ThrowIfNull(pixelarray1);
            ArgumentNullException.ThrowIfNull(pixelarray2);

            if (pixelarray1.Size != pixelarray2.Size) {
                throw new ArgumentException("Mismatch array size.", $"{nameof(pixelarray1)}, {nameof(pixelarray2)}");
            }

            PNGPixelArray png = new(pixelarray1.Size);

            for (int i = 0, length = png.PixelCounts; i < length; i++) {
                png[i] = pixelarray1[i] + pixelarray2[i];
            }

            return png;
        }

        /// <summary>PNGを減算合成する</summary>
        /// <param name="pixelarray1">ピクセルデータ1</param>
        /// <param name="pixelarray2">ピクセルデータ2</param>
        public static PNGPixelArray operator -(PNGPixelArray pixelarray1, PNGPixelArray pixelarray2) {
            ArgumentNullException.ThrowIfNull(pixelarray1);
            ArgumentNullException.ThrowIfNull(pixelarray2);

            if (pixelarray1.Size != pixelarray2.Size) {
                throw new ArgumentException("Mismatch array size.", $"{nameof(pixelarray1)}, {nameof(pixelarray2)}");
            }

            PNGPixelArray png = new(pixelarray1.Size);

            for (int i = 0, length = png.PixelCounts; i < length; i++) {
                png[i] = pixelarray1[i] - pixelarray2[i];
            }

            return png;
        }

        /// <summary>PNGをアルファ合成する</summary>
        /// <param name="pixelarray1">ピクセルデータ1</param>
        /// <param name="pixelarray2">ピクセルデータ2</param>
        public static PNGPixelArray operator *(PNGPixelArray pixelarray1, PNGPixelArray pixelarray2) {
            ArgumentNullException.ThrowIfNull(pixelarray1);
            ArgumentNullException.ThrowIfNull(pixelarray2);

            if (pixelarray1.Size != pixelarray2.Size) {
                throw new ArgumentException("Mismatch array size.", $"{nameof(pixelarray1)}, {nameof(pixelarray2)}");
            }

            PNGPixelArray png = new(pixelarray1.Size);

            for (int i = 0, length = png.PixelCounts; i < length; i++) {
                png[i] = pixelarray1[i] * pixelarray2[i];
            }

            return png;
        }
    }
}