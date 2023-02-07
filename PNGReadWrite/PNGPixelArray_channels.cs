namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>Rチャネル</summary>
        public unsafe float[] RedArray {
            get {
                float[] vs = new float[PixelCounts];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 0; i < PixelCounts; i++, idx += 4) {
                            v[i] = p[idx] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>Gチャネル</summary>
        public unsafe float[] GreenArray {
            get {
                float[] vs = new float[PixelCounts];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 1; i < PixelCounts; i++, idx += 4) {
                            v[i] = p[idx] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>Bチャネル</summary>
        public unsafe float[] BlueArray {
            get {
                float[] vs = new float[PixelCounts];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 2; i < PixelCounts; i++, idx += 4) {
                            v[i] = p[idx] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>Aチャネル</summary>
        public unsafe float[] AlphaArray {
            get {
                float[] vs = new float[PixelCounts];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 3; i < PixelCounts; i++, idx += 4) {
                            v[i] = p[idx] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>RGBチャネルCHW順</summary>
        public unsafe float[] RGBChannelFirstArray {
            get {
                float[] vs = new float[PixelCounts * 3];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 0; i < PixelCounts; i++, idx += 4) {
                            v[i] = p[idx] * PNGPixel.inv_rangef;
                        }
                        for (int i = 0, idx = 1; i < PixelCounts; i++, idx += 4) {
                            v[i + PixelCounts] = p[idx] * PNGPixel.inv_rangef;
                        }
                        for (int i = 0, idx = 2; i < PixelCounts; i++, idx += 4) {
                            v[i + PixelCounts * 2] = p[idx] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>RGBチャネルHWC順</summary>
        public unsafe float[] RGBChannelLastArray {
            get {
                float[] vs = new float[PixelCounts * 3];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 0; i < PixelCounts; i++, idx += 4) {
                            v[i * 3] = p[idx] * PNGPixel.inv_rangef;
                            v[i * 3 + 1] = p[idx + 1] * PNGPixel.inv_rangef;
                            v[i * 3 + 2] = p[idx + 2] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>RGBAチャネルCHW順</summary>
        public unsafe float[] RGBAChannelFirstArray {
            get {
                float[] vs = new float[PixelCounts * 4];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 0; i < PixelCounts; i++, idx += 4) {
                            v[i] = p[idx] * PNGPixel.inv_rangef;
                        }
                        for (int i = 0, idx = 1; i < PixelCounts; i++, idx += 4) {
                            v[i + PixelCounts] = p[idx] * PNGPixel.inv_rangef;
                        }
                        for (int i = 0, idx = 2; i < PixelCounts; i++, idx += 4) {
                            v[i + PixelCounts * 2] = p[idx] * PNGPixel.inv_rangef;
                        }
                        for (int i = 0, idx = 3; i < PixelCounts; i++, idx += 4) {
                            v[i + PixelCounts * 3] = p[idx] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>RGBAチャネルHWC順</summary>
        public unsafe float[] RGBAChannelLastArray {
            get {
                float[] vs = new float[PixelCounts * 4];

                fixed (float* v = vs) {
                    fixed (ushort* p = Pixels) {
                        for (int i = 0, idx = 0; i < PixelCounts; i++, idx += 4) {
                            v[idx] = p[idx] * PNGPixel.inv_rangef;
                            v[idx + 1] = p[idx + 1] * PNGPixel.inv_rangef;
                            v[idx + 2] = p[idx + 2] * PNGPixel.inv_rangef;
                            v[idx + 3] = p[idx + 3] * PNGPixel.inv_rangef;
                        }
                    }
                }

                return vs;
            }
        }

        /// <summary>次元順CHWのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelFirstArray(float[] rgb, int width, int height) {
            if (rgb == null) {
                throw new ArgumentNullException(nameof(rgb));
            }

            if (rgb.Length != checked(3 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            int g_sft = pixelarray.PixelCounts;
            int b_sft = pixelarray.PixelCounts * 2;

            for (int i = 0; i < rgb.Length / 3; i++) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[i], rgb[i + g_sft], rgb[i + b_sft], 1);
            }

            return pixelarray;
        }

        /// <summary>次元順CHWのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelFirstArray(float[] rgb, float[] a, int width, int height) {
            if (rgb == null || a == null) {
                throw new ArgumentNullException($"{nameof(rgb)},{nameof(a)}");
            }

            if (rgb.Length != checked(3 * width * height) || a.Length != checked(width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            int g_sft = pixelarray.PixelCounts;
            int b_sft = pixelarray.PixelCounts * 2;

            for (int i = 0; i < rgb.Length / 3; i++) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[i], rgb[i + g_sft], rgb[i + b_sft], a[i]);
            }

            return pixelarray;
        }

        /// <summary>次元順CHWのテンソルから変換</summary>
        public static PNGPixelArray FromRGBAChannelFirstArray(float[] rgba, int width, int height) {
            if (rgba.Length != checked(4 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 4 x width x height)", nameof(rgba));
            }

            PNGPixelArray pixelarray = new(width, height);

            int g_sft = pixelarray.PixelCounts;
            int b_sft = pixelarray.PixelCounts * 2;
            int a_sft = pixelarray.PixelCounts * 3;

            for (int i = 0; i < rgba.Length / 4; i++) {
                pixelarray[i] = PNGPixel.FromSingle(rgba[i], rgba[i + g_sft], rgba[i + b_sft], rgba[i + a_sft]);
            }

            return pixelarray;
        }

        /// <summary>次元順HWCのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelLastArray(float[] rgb, int width, int height) {
            if (rgb == null) {
                throw new ArgumentNullException(nameof(rgb));
            }

            if (rgb.Length != checked(3 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0, j = 0; i < rgb.Length / 3; i++, j += 3) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[j], rgb[j + 1], rgb[j + 2], 1);
            }

            return pixelarray;
        }

        /// <summary>次元順HWCのテンソルから変換</summary>
        public static PNGPixelArray FromRGBChannelLastArray(float[] rgb, float[] a, int width, int height) {
            if (rgb == null || a == null) {
                throw new ArgumentNullException($"{nameof(rgb)},{nameof(a)}");
            }

            if (rgb.Length != checked(3 * width * height) || a.Length != checked(width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 3 x width x height)", nameof(rgb));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0, j = 0; i < rgb.Length / 3; i++, j += 3) {
                pixelarray[i] = PNGPixel.FromSingle(rgb[j], rgb[j + 1], rgb[j + 2], a[i]);
            }

            return pixelarray;
        }

        /// <summary>次元順HWCのテンソルから変換</summary>
        public static PNGPixelArray FromRGBAChannelLastArray(float[] rgba, int width, int height) {
            if (rgba.Length != checked(4 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 4 x width x height)", nameof(rgba));
            }

            PNGPixelArray pixelarray = new(width, height);

            for (int i = 0, j = 0; i < rgba.Length / 4; i++, j += 4) {
                pixelarray[i] = PNGPixel.FromSingle(rgba[j], rgba[j + 1], rgba[j + 2], rgba[j + 3]);
            }

            return pixelarray;
        }

        /// <summary>グレースケールfloat配列から変換</summary>
        public static PNGPixelArray FromGrayscale(float[] gray, int width, int height) {
            if (gray == null) {
                throw new ArgumentNullException(nameof(gray));
            }

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
