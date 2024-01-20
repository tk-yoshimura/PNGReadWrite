namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>Rチャネル</summary>
        public static unsafe float[] RedChannel(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 0; i < n; i++, idx += 4) {
                        v[i] = p[idx] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>Gチャネル</summary>
        public static unsafe float[] GreenChannel(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 1; i < n; i++, idx += 4) {
                        v[i] = p[idx] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>Bチャネル</summary>
        public static unsafe float[] BlueChannel(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 2; i < n; i++, idx += 4) {
                        v[i] = p[idx] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>Aチャネル</summary>
        public static unsafe float[] AlphaChannel(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 3; i < n; i++, idx += 4) {
                        v[i] = p[idx] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>RGBチャネルCHW順</summary>
        public static unsafe float[] RGBChannelFirst(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n * 3];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 0; i < n; i++, idx += 4) {
                        v[i] = p[idx] * PNGPixel.inv_rangef;
                        v[i + n] = p[idx + 1] * PNGPixel.inv_rangef;
                        v[i + n * 2] = p[idx + 2] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>RGBチャネルHWC順</summary>
        public static unsafe float[] RGBChannelLast(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n * 3];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 0; i < n; i++, idx += 4) {
                        v[i * 3] = p[idx] * PNGPixel.inv_rangef;
                        v[i * 3 + 1] = p[idx + 1] * PNGPixel.inv_rangef;
                        v[i * 3 + 2] = p[idx + 2] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>RGBAチャネルCHW順</summary>
        public static unsafe float[] RGBAChannelFirst(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n * 4];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 0; i < n; i++, idx += 4) {
                        v[i] = p[idx] * PNGPixel.inv_rangef;
                        v[i + n] = p[idx + 1] * PNGPixel.inv_rangef;
                        v[i + n * 2] = p[idx + 2] * PNGPixel.inv_rangef;
                        v[i + n * 3] = p[idx + 3] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>RGBAチャネルHWC順</summary>
        public static unsafe float[] RGBAChannelLast(PNGPixelArray pixelarray) {
            int n = pixelarray.PixelCounts;

            float[] vs = new float[n * 4];

            fixed (float* v = vs) {
                fixed (ushort* p = pixelarray.Pixels) {
                    for (int i = 0, idx = 0; i < n; i++, idx += 4) {
                        v[idx] = p[idx] * PNGPixel.inv_rangef;
                        v[idx + 1] = p[idx + 1] * PNGPixel.inv_rangef;
                        v[idx + 2] = p[idx + 2] * PNGPixel.inv_rangef;
                        v[idx + 3] = p[idx + 3] * PNGPixel.inv_rangef;
                    }
                }
            }

            return vs;
        }

        /// <summary>次元順CHWのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelFirst(float[] rgb, int width, int height) {
            ArgumentNullException.ThrowIfNull(rgb);

            if (rgb.Length != checked(3 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            int g_sft = pixelarray.PixelCounts;
            int b_sft = pixelarray.PixelCounts * 2;

            for (int i = 0, n = rgb.Length / 3; i < n; i++) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[i], rgb[i + g_sft], rgb[i + b_sft], 1);
            }

            return pixelarray;
        }

        /// <summary>次元順CHWのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelFirst(float[] rgb, float[] a, int width, int height) {
            ArgumentNullException.ThrowIfNull(rgb);
            ArgumentNullException.ThrowIfNull(a);

            if (rgb.Length != checked(3 * width * height) || a.Length != checked(width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            int g_sft = pixelarray.PixelCounts;
            int b_sft = pixelarray.PixelCounts * 2;

            for (int i = 0, n = rgb.Length / 3; i < n; i++) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[i], rgb[i + g_sft], rgb[i + b_sft], a[i]);
            }

            return pixelarray;
        }

        /// <summary>次元順CHWのテンソルから変換</summary>
        public static PNGPixelArray FromRGBAChannelFirst(float[] rgba, int width, int height) {
            if (rgba.Length != checked(4 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 4 x width x height)", nameof(rgba));
            }

            PNGPixelArray pixelarray = new(width, height);

            int g_sft = pixelarray.PixelCounts;
            int b_sft = pixelarray.PixelCounts * 2;
            int a_sft = pixelarray.PixelCounts * 3;

            for (int i = 0, n = rgba.Length / 4; i < n; i++) {
                pixelarray[i] = PNGPixel.FromSingle(rgba[i], rgba[i + g_sft], rgba[i + b_sft], rgba[i + a_sft]);
            }

            return pixelarray;
        }

        /// <summary>次元順HWCのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelLast(float[] rgb, int width, int height) {
            ArgumentNullException.ThrowIfNull(rgb);

            if (rgb.Length != checked(3 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0, j = 0, n = rgb.Length / 3; i < n; i++, j += 3) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[j], rgb[j + 1], rgb[j + 2], 1);
            }

            return pixelarray;
        }

        /// <summary>次元順HWCのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelLast(float[] rgb, float[] a, int width, int height) {
            if (rgb == null || a == null) {
                throw new ArgumentNullException($"{nameof(rgb)},{nameof(a)}");
            }

            if (rgb.Length != checked(3 * width * height) || a.Length != checked(width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0, j = 0, n = rgb.Length / 3; i < n; i++, j += 3) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[j], rgb[j + 1], rgb[j + 2], a[i]);
            }

            return pixelarray;
        }

        /// <summary>次元順HWCのテンソルから変換</summary>
        public static PNGPixelArray FromRGBAChannelLast(float[] rgba, int width, int height) {
            if (rgba.Length != checked(4 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 4 x width x height)", nameof(rgba));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0, j = 0, n = rgba.Length / 4; i < n; i++, j += 4) {
                pixelarray[i] = PNGPixel.FromSingle(rgba[j], rgba[j + 1], rgba[j + 2], rgba[j + 3]);
            }

            return pixelarray;
        }

        /// <summary>グレースケールfloat配列から変換</summary>
        public static PNGPixelArray FromGrayscale(float[] gray, int width, int height) {
            ArgumentNullException.ThrowIfNull(gray);

            if (gray.Length != checked(width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = width x height)", nameof(gray));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0; i < gray.Length; i++) {
                pixelarray[i] = PNGPixel.FromSingle(gray[i], gray[i], gray[i], 1);
            }

            return pixelarray;
        }
    }
}
