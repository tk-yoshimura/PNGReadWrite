using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PNGReadWrite {

    /// <summary>PNGピクセル</summary>
    /// <remarks>コンストラクタ(uint16)</remarks>
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{ToFloatString(),nq}")]
    public partial struct PNGPixel(ushort r, ushort g, ushort b, ushort a = 0xFFFF) {
        /// <summary>RGB輝度値およびアルファ値</summary>
        public ushort R = r, G = g, B = b, A = a;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal const float inv_rangef = 1f / ushort.MaxValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal const double inv_ranged = 1d / ushort.MaxValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal const float epsf = 5e-7f * ushort.MaxValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal const double epsd = 5e-15d * ushort.MaxValue;

        /// <summary>タプル変換</summary>
        public static implicit operator PNGPixel((ushort r, ushort g, ushort b) cr) {
            return new PNGPixel(cr.r, cr.g, cr.b);
        }

        /// <summary>タプル分解</summary>
        public readonly void Deconstruct(out ushort r, out ushort g, out ushort b) {
            (r, g, b) = (R, G, B);
        }

        /// <summary>タプル変換</summary>
        public static implicit operator PNGPixel((ushort r, ushort g, ushort b, ushort a) cr) {
            return new PNGPixel(cr.r, cr.g, cr.b, cr.a);
        }

        /// <summary>タプル分解</summary>
        public readonly void Deconstruct(out ushort r, out ushort g, out ushort b, out ushort a) {
            (r, g, b, a) = (R, G, B, A);
        }

        /// <summary>等色判定</summary>
        public static bool operator ==(PNGPixel cr1, PNGPixel cr2) {
            return cr1.R == cr2.R && cr1.G == cr2.G && cr1.B == cr2.B && cr1.A == cr2.A;
        }

        /// <summary>等色判定</summary>
        public static bool operator !=(PNGPixel cr1, PNGPixel cr2) {
            return !(cr1 == cr2);
        }

        /// <summary>加法混色</summary>
        public static PNGPixel operator +(PNGPixel cr1, PNGPixel cr2) {
            uint a1 = cr1.A, a2 = cr2.A;

            if (a1 == 0) {
                return cr2;
            }
            if (a2 == 0) {
                return cr1;
            }

            unchecked {
                uint a = 0xFFFFu - (((0xFFFFu - a1) * (0xFFFFu - a2) + 0xFFFFu) >> 16);

                if (a <= 0) {
                    return new PNGPixel();
                }

                int r = (int)(((long)cr1.R * a1 + (long)cr2.R * a2) / a);
                int g = (int)(((long)cr1.G * a1 + (long)cr2.G * a2) / a);
                int b = (int)(((long)cr1.B * a1 + (long)cr2.B * a2) / a);

                return new PNGPixel((ushort)Math.Min(ushort.MaxValue, r), (ushort)Math.Min(ushort.MaxValue, g), (ushort)Math.Min(ushort.MaxValue, b), (ushort)a);
            }
        }

        /// <summary>減法混色</summary>
        public static PNGPixel operator -(PNGPixel cr1, PNGPixel cr2) {
            uint a1 = cr1.A, a2 = cr2.A;

            if (a1 == 0) {
                return new PNGPixel(0, 0, 0, cr2.A);
            }
            if (a2 == 0) {
                return cr1;
            }

            unchecked {
                uint a = 0xFFFFu - (((0xFFFFu - a1) * (0xFFFFu - a2) + 0xFFFFu) >> 16);

                if (a <= 0) {
                    return new PNGPixel();
                }

                int r = (int)(((long)cr1.R * a1 - (long)cr2.R * a2) / a);
                int g = (int)(((long)cr1.G * a1 - (long)cr2.G * a2) / a);
                int b = (int)(((long)cr1.B * a1 - (long)cr2.B * a2) / a);

                return new PNGPixel((ushort)Math.Max(ushort.MinValue, r), (ushort)Math.Max(ushort.MinValue, g), (ushort)Math.Max(ushort.MinValue, b), (ushort)a);
            }
        }

        /// <summary>アルファブレンディング</summary>
        /// <param name="cr1">背景色</param>
        /// <param name="cr2">前景色</param>
        public static PNGPixel operator *(PNGPixel cr1, PNGPixel cr2) {
            uint a1 = cr1.A, a2 = cr2.A;

            if (a1 == 0 || a2 == 0xFFFF) {
                return cr2;
            }
            if (a2 == 0) {
                return cr1;
            }

            unchecked {
                uint a = 0xFFFFu - (((0xFFFFu - a1) * (0xFFFFu - a2) + 0xFFFFu) >> 16);

                if (a <= 0) {
                    return new PNGPixel();
                }

                ushort r = (ushort)((cr1.R * (a - a2) + cr2.R * a2) / a);
                ushort g = (ushort)((cr1.G * (a - a2) + cr2.G * a2) / a);
                ushort b = (ushort)((cr1.B * (a - a2) + cr2.B * a2) / a);

                return new PNGPixel(r, g, b, (ushort)a);
            }
        }

        /// <summary>GDI+カラーへ変換</summary>
        public static explicit operator Color(PNGPixel cr) {
            unchecked {
                return Color.FromArgb(cr.A >> 8, cr.R >> 8, cr.G >> 8, cr.B >> 8);
            }
        }

        /// <summary>GDI+カラーから変換</summary>
        public static implicit operator PNGPixel(Color cr) {
            unchecked {
                return new PNGPixel((ushort)(cr.R << 8 | cr.R), (ushort)(cr.G << 8 | cr.G), (ushort)(cr.B << 8 | cr.B), (ushort)(cr.A << 8 | cr.A));
            }
        }

        /// <summary>色指定(int32 [0, 65535])</summary>
        public static PNGPixel FromInt32(int r, int g, int b, int a) {
            return new PNGPixel((ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, r)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, g)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, b)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, a)));
        }

        /// <summary>色指定(float [0, 1])</summary>
        public static PNGPixel FromSingle(float r, float g, float b, float a) {
            return new PNGPixel((ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, r * 0xFFFF + epsf)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, g * 0xFFFF + epsf)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, b * 0xFFFF + epsf)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, a * 0xFFFF + epsf)));
        }

        /// <summary>色指定(double [0, 1])</summary>
        public static PNGPixel FromDouble(double r, double g, double b, double a) {
            return new PNGPixel((ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, r * 0xFFFF + epsd)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, g * 0xFFFF + epsd)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, b * 0xFFFF + epsd)),
                                (ushort)Math.Max(ushort.MinValue, Math.Min(ushort.MaxValue, a * 0xFFFF + epsd)));
        }

        /// <summary>色指定(int32 [0, 65535])</summary>
        public readonly (int r, int g, int b, int a) ToInt32() {
            return (R, G, B, A);
        }

        /// <summary>色取得(float [0, 1])</summary>
        public readonly (float r, float g, float b, float a) ToSingle() {
            return (R * inv_rangef, G * inv_rangef, B * inv_rangef, A * inv_rangef);
        }

        /// <summary>色取得(double [0, 1])</summary>
        public readonly (double r, double g, double b, double a) ToDouble() {
            return (R * inv_ranged, G * inv_ranged, B * inv_ranged, A * inv_ranged);
        }

        /// <summary>オブジェクト比較</summary>
        public override readonly bool Equals(object? obj) {
            return obj is not null && obj is PNGPixel pixel && this == pixel;
        }

        /// <summary>ハッシュ値</summary>
        public override readonly int GetHashCode() {
            return (R | (G << 16)) ^ (B | (A << 16));
        }

        /// <summary>文字列化</summary>
        /// <remarks>16進数表記 RRRR GGGG BBBB AAAA</remarks>
        public override readonly string ToString() {
            return $"{R:X4} {G:X4} {B:X4} {A:X4}";
        }

        /// <summary>文字列化</summary>
        /// <remarks>浮動小数点表記 .4f</remarks>
        public readonly string ToFloatString() {
            return $"{(R * inv_rangef):F4} {(G * inv_rangef):F4} {(B * inv_rangef):F4} {(A * inv_rangef):F4}";
        }
    }
}
