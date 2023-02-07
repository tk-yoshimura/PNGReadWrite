namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>生配列からピクセル配列へ変換</summary>
        private static ushort[] FromRawPixels(Array pixels, int width, int height, PNGFormat format) {
            if (pixels == null) {
                throw new ArgumentNullException(nameof(pixels));
            }

            if (!Enum.IsDefined(typeof(PNGFormat), format) || format == PNGFormat.Undefined) {
                throw new NotSupportedException("Unsupported format.");
            }

            if (width <= 0 || height <= 0) {
                throw new ArgumentException("The specified size is invalid.", $"{nameof(width)}, {nameof(height)}");
            }

            if (format.Depth() == 8 && pixels.GetType().GetElementType() == typeof(byte)) {
                byte[] pixels_byte = (byte[])pixels;

                if (format.Channels() == 3) {
                    ushort[] pixels_ushort = UInt8toUInt16(AddAlphaChannel(RGBswapGBR(pixels_byte)));
                    return pixels_ushort;
                }
                if (format.Channels() == 4) {
                    ushort[] pixels_ushort = UInt8toUInt16(RGBAswapGBRA(pixels_byte));
                    return pixels_ushort;
                }
            }

            if (format.Depth() == 16 && pixels.GetType().GetElementType() == typeof(ushort)) {
                ushort[] pixels_ushort = (ushort[])pixels;

                if (format.Channels() == 3) {
                    pixels_ushort = AddAlphaChannel(pixels_ushort);
                    return pixels_ushort;
                }
                if (format.Channels() == 4) {
                    return pixels_ushort;
                }
            }

            throw new NotSupportedException("Unsupported format.");
        }

        /// <summary>PNGフォーマットに準拠した配列へ変換</summary>
        private Array ToRawPixels(PNGFormat format) {
            if (!Enum.IsDefined(typeof(PNGFormat), format) || format == PNGFormat.Undefined) {
                throw new NotSupportedException("Unsupported format.");
            }

            if (format.Depth() == 8) {
                ushort[] pixels_ushort = Pixels;

                byte[] pixels_byte = UInt16toUInt8(RGBAswapGBRA(pixels_ushort));

                if (format.Channels() == 3) {
                    pixels_byte = RemoveAlphaChannel(pixels_byte);
                }

                return pixels_byte;
            }
            else {
                ushort[] pixels_ushort = Pixels;

                if (format.Channels() == 3) {
                    pixels_ushort = RemoveAlphaChannel(pixels_ushort);
                }

                return pixels_ushort;
            }
        }

        /// <summary>uint16 -> uint8</summary>
        private static byte[] UInt16toUInt8(ushort[] arr) {
            byte[] ret = new byte[arr.Length];

            unsafe {
                fixed (byte* p_ret = ret) {
                    fixed (ushort* p_arr = arr) {

                        for (int i = 0; i < arr.Length; i++) {
                            p_ret[i] = (byte)(p_arr[i] >> 8);
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint8 -> uint16</summary>
        private static ushort[] UInt8toUInt16(byte[] arr) {
            ushort[] ret = new ushort[arr.Length];

            unsafe {
                fixed (ushort* p_ret = ret) {
                    fixed (byte* p_arr = arr) {

                        for (int i = 0; i < arr.Length; i++) {
                            p_ret[i] = (ushort)((p_arr[i] << 8) | p_arr[i]);
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint16 アルファチャネル追加</summary>
        private static ushort[] AddAlphaChannel(ushort[] arr) {
            if ((arr.Length % 3) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            ushort[] ret = new ushort[checked(arr.Length / 3 * 4)];

            unsafe {
                fixed (ushort* p_ret = ret) {
                    fixed (ushort* p_arr = arr) {

                        for (int i = 0, j = 0; i < arr.Length; i += 3, j += 4) {
                            p_ret[j] = p_arr[i];
                            p_ret[j + 1] = p_arr[i + 1];
                            p_ret[j + 2] = p_arr[i + 2];
                            p_ret[j + 3] = 0xFFFF;
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint8 アルファチャネル追加</summary>
        private static byte[] AddAlphaChannel(byte[] arr) {
            if ((arr.Length % 3) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            byte[] ret = new byte[checked(arr.Length / 3 * 4)];

            unsafe {
                fixed (byte* p_ret = ret) {
                    fixed (byte* p_arr = arr) {

                        for (int i = 0, j = 0; i < arr.Length; i += 3, j += 4) {
                            p_ret[j] = p_arr[i];
                            p_ret[j + 1] = p_arr[i + 1];
                            p_ret[j + 2] = p_arr[i + 2];
                            p_ret[j + 3] = 0xFF;
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint16 アルファチャネル除去</summary>
        /// <remarks>背景色 : 白</remarks>
        private static ushort[] RemoveAlphaChannel(ushort[] arr) {
            if ((arr.Length % 4) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            ushort[] ret = new ushort[checked(arr.Length / 4 * 3)];

            unsafe {
                fixed (ushort* p_ret = ret) {
                    fixed (ushort* p_arr = arr) {

                        for (int i = 0, j = 0; i < arr.Length; i += 4, j += 3) {
                            uint a = p_arr[i + 3];

                            unchecked {
                                p_ret[j] = (ushort)((p_arr[i] * a + 0xFFFF * (0x10000 - a)) >> 16);
                                p_ret[j + 1] = (ushort)((p_arr[i + 1] * a + 0xFFFF * (0x10000 - a)) >> 16);
                                p_ret[j + 2] = (ushort)((p_arr[i + 2] * a + 0xFFFF * (0x10000 - a)) >> 16);
                            }
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint8 アルファチャネル除去</summary>
        /// <remarks>背景色 : 白</remarks>
        private static byte[] RemoveAlphaChannel(byte[] arr) {
            if ((arr.Length % 4) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            byte[] ret = new byte[checked(arr.Length / 4 * 3)];

            unsafe {
                fixed (byte* p_ret = ret) {
                    fixed (byte* p_arr = arr) {

                        for (int i = 0, j = 0; i < arr.Length; i += 4, j += 3) {
                            uint a = arr[i + 3];

                            unchecked {
                                ret[j] = (byte)((arr[i] * a + 0xFF * (0x100 - a)) >> 8);
                                ret[j + 1] = (byte)((arr[i + 1] * a + 0xFF * (0x100 - a)) >> 8);
                                ret[j + 2] = (byte)((arr[i + 2] * a + 0xFF * (0x100 - a)) >> 8);
                            }
                        }

                    }
                }
            }
            return ret;
        }

        /// <summary>uint16 RGB swap GBR</summary>
        private static ushort[] RGBswapGBR(ushort[] arr) {
            if ((arr.Length % 3) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            ushort[] ret = new ushort[arr.Length];

            unsafe {
                fixed (ushort* p_ret = ret) {
                    fixed (ushort* p_arr = arr) {

                        for (int i = 0; i < arr.Length; i += 3) {
                            p_ret[i] = p_arr[i + 2];
                            p_ret[i + 1] = p_arr[i + 1];
                            p_ret[i + 2] = p_arr[i];
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint8 RGB swap GBR</summary>
        private static byte[] RGBswapGBR(byte[] arr) {
            if ((arr.Length % 3) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            byte[] ret = new byte[arr.Length];

            unsafe {
                fixed (byte* p_ret = ret) {
                    fixed (byte* p_arr = arr) {

                        for (int i = 0; i < arr.Length; i += 3) {
                            p_ret[i] = p_arr[i + 2];
                            p_ret[i + 1] = p_arr[i + 1];
                            p_ret[i + 2] = p_arr[i];
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint16 RGBA swap GBRA</summary>
        private static ushort[] RGBAswapGBRA(ushort[] arr) {
            if ((arr.Length % 4) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            ushort[] ret = new ushort[arr.Length];

            unsafe {
                fixed (ushort* p_ret = ret) {
                    fixed (ushort* p_arr = arr) {

                        for (int i = 0; i < arr.Length; i += 4) {
                            p_ret[i] = p_arr[i + 2];
                            p_ret[i + 1] = p_arr[i + 1];
                            p_ret[i + 2] = p_arr[i];
                            p_ret[i + 3] = p_arr[i + 3];
                        }

                    }
                }
            }

            return ret;
        }

        /// <summary>uint8 RGBA swap GBRA</summary>
        private static byte[] RGBAswapGBRA(byte[] arr) {
            if ((arr.Length % 4) != 0) {
                throw new ArgumentException(null, nameof(arr));
            }

            byte[] ret = new byte[arr.Length];

            unsafe {
                fixed (byte* p_ret = ret) {
                    fixed (byte* p_arr = arr) {

                        for (int i = 0; i < arr.Length; i += 4) {
                            p_ret[i] = p_arr[i + 2];
                            p_ret[i + 1] = p_arr[i + 1];
                            p_ret[i + 2] = p_arr[i];
                            p_ret[i + 3] = p_arr[i + 3];
                        }

                    }
                }
            }

            return ret;
        }
    }
}
