using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace PNGReadWrite {

    /// <summary>PNGピクセルデータ</summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public partial class PNGPixelArray : ICloneable {

        /// <summary>ピクセル配列</summary>
        public ushort[] Pixels { get; private set; } = new ushort[4];

        /// <summary>幅</summary>        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Width { get; private set; }

        /// <summary>高さ</summary>        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Height { get; private set; }

        /// <summary>ピクセル数</summary>
        public int PixelCounts => Width * Height;

        /// <summary>大きさ</summary>
        public (int width, int height) Size => (Width, Height);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private PNGMetadata metadata = PNGMetadata.Default;
        /// <summary>メタデータ</summary>
        public PNGMetadata Metadata {
            get {
                return metadata;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(metadata));
                }

                metadata = (PNGMetadata)value.Clone();
            }
        }

        /// <summary>コンストラクタ</summary>
        public PNGPixelArray() {
            Clear();
        }

        /// <summary>コンストラクタ 大きさ指定</summary>
        public PNGPixelArray((int width, int height) size) : this(size.width, size.height) { }

        /// <summary>コンストラクタ 大きさ指定</summary>
        public PNGPixelArray(int width, int height) {
            if (width <= 0 || height <= 0) {
                throw new ArgumentException("The specified size is invalid.", $"{nameof(width)}, {nameof(height)}");
            }

            this.Pixels = new ushort[checked(4 * width * height)];
            this.Width = width;
            this.Height = height;
        }

        /// <summary>コンストラクタ ピクセル配列指定</summary>
        /// <remarks>ピクセル配列はコピーされる</remarks>
        public PNGPixelArray(ushort[] pixels, int width, int height) {
            if (pixels == null) {
                throw new ArgumentNullException(nameof(pixels));
            }

            if (width <= 0 || height <= 0) {
                throw new ArgumentException("The specified size is invalid.", $"{nameof(width)}, {nameof(height)}");
            }

            if (pixels.Length != checked(4 * width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = 4 x width x height)", nameof(pixels));
            }

            this.Pixels = (ushort[])pixels.Clone();
            this.Width = width;
            this.Height = height;
        }

        /// <summary>コンストラクタ ピクセル配列指定</summary>
        /// <remarks>ピクセル配列はコピーされる</remarks>
        public PNGPixelArray(PNGPixel[,] pixels) {
            if (pixels == null) {
                throw new ArgumentNullException(nameof(pixels));
            }

            int width = pixels.GetLength(0), height = pixels.GetLength(1);

            if (width <= 0 || height <= 0) {
                throw new ArgumentException("The specified size is invalid.", $"{nameof(width)}, {nameof(height)}");
            }

            this.Pixels = new ushort[checked(4 * width * height)];
            this.Width = width;
            this.Height = height;

            unsafe {
                fixed (ushort* p_arr = this.Pixels) {
                    for (int x, y = 0, i = 0; y < height; y++) {
                        for (x = 0; x < width; x++, i += 4) {
                            PNGPixel cr = pixels[x, y];

                            p_arr[i] = cr.R;
                            p_arr[i + 1] = cr.G;
                            p_arr[i + 2] = cr.B;
                            p_arr[i + 3] = cr.A;
                        }
                    }
                }
            }
        }

        /// <summary>コンストラクタ ピクセル配列指定</summary>
        /// <remarks>ピクセル配列はコピーされる</remarks>
        public PNGPixelArray(PNGPixel[] pixels, Size size) : this(pixels, size.Width, size.Height) { }

        /// <summary>コンストラクタ ピクセル配列指定</summary>
        /// <remarks>ピクセル配列はコピーされる</remarks>
        public PNGPixelArray(PNGPixel[] pixels, int width, int height) {
            if (pixels == null) {
                throw new ArgumentNullException(nameof(pixels));
            }

            if (width <= 0 || height <= 0) {
                throw new ArgumentException("The specified size is invalid.", $"{nameof(width)}, {nameof(height)}");
            }

            if (pixels.Length != checked(width * height)) {
                throw new ArgumentException("The specified array length is invalid. (Length = width x height)", nameof(pixels));
            }

            this.Pixels = new ushort[checked(4 * width * height)];
            this.Width = width;
            this.Height = height;

            unsafe {
                fixed (ushort* p_arr = this.Pixels) {
                    for (int x, y = 0, i = 0, j = 0; y < height; y++) {
                        for (x = 0; x < width; x++, i += 4, j++) {
                            PNGPixel cr = pixels[j];

                            p_arr[i] = cr.R;
                            p_arr[i + 1] = cr.G;
                            p_arr[i + 2] = cr.B;
                            p_arr[i + 3] = cr.A;
                        }
                    }
                }
            }
        }

        /// <summary>コンストラクタ ファイル名指定</summary>
        /// <exception cref="FileNotFoundException">ファイルが存在しないとき</exception>
        /// <exception cref="FileFormatException">ファイル形式が不正であるとき</exception>
        /// <exception cref="NotSupportedException">コーデックが対応していないとき</exception>
        /// <exception cref="InvalidDataException">CRCまたはチャンクが不正であるとき</exception>
        /// <exception cref="OverflowException">データサイズが1GB以上、その他オーバーフローが発生したとき</exception>
        public PNGPixelArray(string filepath) {
            Read(filepath);
        }

        /// <summary>インデクサ</summary>
        /// <param name="i">左上から左方向に数えたインデクス</param>
        public PNGPixel this[int i] {
            get {
                int index = 4 * i;

                return new PNGPixel(Pixels[index], Pixels[index + 1], Pixels[index + 2], Pixels[index + 3]);
            }
            set {
                int index = 4 * i;

                Pixels[index] = value.R;
                Pixels[index + 1] = value.G;
                Pixels[index + 2] = value.B;
                Pixels[index + 3] = value.A;
            }
        }

        /// <summary>インデクサ</summary>
        /// <param name="x">左上を基点とするx座標</param>
        /// <param name="y">左上を基点とするy座標</param>
        public PNGPixel this[int x, int y] {
            get {
                if (!InRange(x, y)) {
                    throw new ArgumentOutOfRangeException($"{nameof(x)},{nameof(y)}", "The specified coordinates is out of bounds.");
                }

                int index = 4 * (x + y * Width);

                return new PNGPixel(Pixels[index], Pixels[index + 1], Pixels[index + 2], Pixels[index + 3]);
            }
            set {
                if (!InRange(x, y)) {
                    throw new ArgumentOutOfRangeException($"{nameof(x)},{nameof(y)}", "The specified coordinates is out of bounds.");
                }

                int index = 4 * (x + y * Width);

                Pixels[index] = value.R;
                Pixels[index + 1] = value.G;
                Pixels[index + 2] = value.B;
                Pixels[index + 3] = value.A;
            }
        }

        /// <summary>インデクサ</summary>
        /// <param name="x_index">左上を基点とするx座標</param>
        /// <param name="y_index">左上を基点とするy座標</param>
        public PNGPixel this[Index x_index, Index y_index] {
            get => this[x_index.GetOffset(Width), y_index.GetOffset(Height)];
            set => this[x_index.GetOffset(Width), y_index.GetOffset(Height)] = value;
        }

        /// <summary>ピクセル二次配列から変換</summary>
        public static implicit operator PNGPixelArray(PNGPixel[,] pixels) {
            return new PNGPixelArray(pixels);
        }

        /// <summary>ピクセル二次配列へ変換</summary>
        /// <remarks>メタデータが損失する</remarks>
        public static explicit operator PNGPixel[,](PNGPixelArray pixelarray) {
            int width = pixelarray.Width, height = pixelarray.Height;
            PNGPixel[,] pixels = new PNGPixel[width, height];

            unsafe {
                fixed (ushort* p_arr = pixelarray.Pixels) {
                    for (int x, y = 0, i = 0; y < height; y++) {
                        for (x = 0; x < width; x++, i += 4) {
                            pixels[x, y] = new PNGPixel(p_arr[i], p_arr[i + 1], p_arr[i + 2], p_arr[i + 3]);
                        }
                    }
                }
            }

            return pixels;
        }

        /// <summary>ピクセル一次配列へ変換</summary>
        /// <remarks>メタデータが損失する</remarks>
        public static explicit operator PNGPixel[](PNGPixelArray pixelarray) {
            PNGPixel[] pixels = new PNGPixel[pixelarray.PixelCounts];

            unsafe {
                fixed (ushort* p_arr = pixelarray.Pixels) {
                    for (int i = 0, j = 0, length = pixels.Length; i < length; i++, j += 4) {
                        pixels[i] = new PNGPixel(p_arr[j], p_arr[j + 1], p_arr[j + 2], p_arr[j + 3]);
                    }
                }
            }

            return pixels;
        }

        /// <summary>領域内か判定</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InRange(Point pt) {
            return InRange(pt.X, pt.Y);
        }

        /// <summary>領域内か判定</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InRange(int x, int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>領域内か判定</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InRange(uint x, uint y) {
            return x < Width && y < Height;
        }

        /// <summary>全ピクセルのアルファ値が0か判定</summary>
        public bool IsBlank() {
            unsafe {
                fixed (ushort* p_arr = Pixels) {
                    for (int i = 0; i < Pixels.Length; i += 4) {
                        if (p_arr[i + 3] != 0) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>透明度が設定されているか判定</summary>
        public bool HasAlphaChannel() {
            unsafe {
                fixed (ushort* p_arr = Pixels) {
                    for (int i = 0; i < Pixels.Length; i += 4) {
                        if (p_arr[i + 3] < 0xFFFF) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>初期化</summary>
        /// <remarks>1x1のピクセルデータとして初期化される</remarks>
        public void Clear() {
            Pixels = new ushort[4];
            Width = Height = 1;
            Metadata = PNGMetadata.Default;
        }

        /// <summary>クローン</summary>
        public object Clone() {
            return new PNGPixelArray(Pixels, Width, Height) {
                Metadata = (PNGMetadata)this.Metadata.Clone()
            };
        }

        /// <summary>ディープコピー</summary>
        public PNGPixelArray Copy() {
            return new PNGPixelArray(Pixels, Width, Height) {
                Metadata = (PNGMetadata)this.Metadata.Clone()
            };
        }

        /// <summary>文字列化</summary>
        /// <remarks>幅 x 高さ</remarks>
        public override string ToString() {
            return $"{Width} x {Height}";
        }
    }
}
