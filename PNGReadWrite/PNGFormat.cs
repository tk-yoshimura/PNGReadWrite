using System;
using System.Windows.Media;

namespace PNGReadWrite {

    /// <summary>PNGフォーマット</summary>
    public enum PNGFormat {
        /// <summary>既定値</summary>
        Undefined,

        /// <summary>アルファチャネルなし 色深度8</summary>
        RGB24,

        /// <summary>アルファチャネルあり 色深度8</summary>
        RGBA32,

        /// <summary>アルファチャネルなし 色深度16</summary>
        RGB48,

        /// <summary>アルファチャネルあり 色深度16</summary>
        RGBA64
    }

    /// <summary>PNGフォーマット拡張</summary>
    public static class PNGFormatExtend {

        /// <summary>アルファチャネルを持つか</summary>
        public static bool HasAlphaChannel(this PNGFormat format) {
            return format == PNGFormat.RGBA32 || format == PNGFormat.RGBA64;
        }

        /// <summary>チャネル数</summary>
        public static int Channels(this PNGFormat format) {
            if (!Enum.IsDefined(typeof(PNGFormat), format) || format == PNGFormat.Undefined) {
                return 0;
            }

            return format.HasAlphaChannel() ? 4 : 3;
        }

        /// <summary>色深度</summary>
        public static int Depth(this PNGFormat format) {
            if (format == PNGFormat.RGB24 || format == PNGFormat.RGBA32) {
                return 8;
            }

            if (format == PNGFormat.RGB48 || format == PNGFormat.RGBA64) {
                return 16;
            }

            return 0;
        }

        /// <summary>WICピクセルフォーマットからPNGフォーマットへ変換</summary>
        internal static PNGFormat ToPNGFormat(this PixelFormat format) {
            if (format == PixelFormats.Bgr24) {
                return PNGFormat.RGB24;
            }
            else if (format == PixelFormats.Rgb48) {
                return PNGFormat.RGB48;
            }
            else if (format == PixelFormats.Bgra32 || format == PixelFormats.Bgr32 || format == PixelFormats.Pbgra32) {
                return PNGFormat.RGBA32;
            }
            else if (format == PixelFormats.Rgba64) {
                return PNGFormat.RGBA64;
            }
            else {
                return PNGFormat.Undefined;
            }
        }

        /// <summary>PNGフォーマットへ変換からWICピクセルフォーマットへ変換</summary>
        internal static PixelFormat ToPixelFormat(this PNGFormat format) {
            switch (format) {
                case PNGFormat.RGB24:
                    return PixelFormats.Bgr24;
                case PNGFormat.RGB48:
                    return PixelFormats.Rgb48;
                case PNGFormat.RGBA32:
                    return PixelFormats.Bgra32;
                case PNGFormat.RGBA64:
                    return PixelFormats.Rgba64;
                default:
                    throw new NotSupportedException(nameof(format));
            }
        }
    }
}