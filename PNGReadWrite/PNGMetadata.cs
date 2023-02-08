using System.Diagnostics;
using System.Text;

namespace PNGReadWrite {

    /// <summary>メタデータ</summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public class PNGMetadata : ICloneable {

        /// <summary>ガンマ値</summary>
        /// <remarks>逆数表記 1/γ WICは常に既定値を書き込む</remarks>
        public PNGFixed? Gamma { get; set; } = null;

        /// <summary>CIExy色度図代表点</summary>
        public PNGChromaticityPoints? ChromaticityPoints { get; set; } = null;

        /// <summary>色域変換時に優先するレンダリングオプション</summary>
        public PNGRenderingIntents? RenderingIntent { get; set; } = null;

        /// <summary>作成時間</summary>
        public DateTime? RecordTime { get; set; } = null;

        /// <summary>dpi</summary>
        /// <remarks>既定値 : 96</remarks>
        public (double x, double y) Dpi { get; set; } = DefaultDpi;

        public static double DefaultGamma { get; } = 1 / 2.2;
        public static (double x, double y) DefaultDpi { get; } = (96, 96);

        /// <summary>コンストラクタ メタデータ無し</summary>
        public PNGMetadata() { }

        /// <summary>既定値 ガンマ値2.2 D65 sRGB </summary>
        public static PNGMetadata Default {
            get {
                PNGMetadata metadata = new() {
                    Gamma = DefaultGamma,
                    ChromaticityPoints = PNGChromaticityPoints.Default,
                    RenderingIntent = PNGRenderingIntents.Perceptual,
                    RecordTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                    Dpi = DefaultDpi
                };

                return metadata;
            }
        }

        /// <summary>読み込み</summary>
        internal void Read(List<PNGChunk> chunks) {
            Gamma = null;
            ChromaticityPoints = null;
            RenderingIntent = null;
            RecordTime = null;
            Dpi = DefaultDpi;

            foreach (PNGChunk chunk in chunks) {
                if (chunk.Type == PNGgAMAChunk.Type) {
                    Gamma = new PNGgAMAChunk(chunk).Gamma;
                }
                else if (chunk.Type == PNGcHRMChunk.Type) {
                    ChromaticityPoints = new PNGcHRMChunk(chunk).ChromaticityPoints;
                }
                else if (chunk.Type == PNGsRGBChunk.Type) {
                    RenderingIntent = new PNGsRGBChunk(chunk).RenderingIntent;
                }
                else if (chunk.Type == PNGtIMEChunk.Type) {
                    RecordTime = new PNGtIMEChunk(chunk).RecordTime;
                }
                else if (chunk.Type == PNGpHYsChunk.Type) {
                    Dpi = new PNGpHYsChunk(chunk).Dpi;
                }
            }
        }

        /// <summary>書き込み</summary>
        internal void Write(List<PNGChunk> chunks) {
            chunks.RemoveAll((chunk) => chunk.Type == PNGgAMAChunk.Type);
            chunks.RemoveAll((chunk) => chunk.Type == PNGcHRMChunk.Type);
            chunks.RemoveAll((chunk) => chunk.Type == PNGsRGBChunk.Type);
            chunks.RemoveAll((chunk) => chunk.Type == PNGtIMEChunk.Type);
            chunks.RemoveAll((chunk) => chunk.Type == PNGpHYsChunk.Type);

            List<PNGChunk> chunks_insert = new();

            if (Gamma != null) {
                chunks_insert.Add(PNGgAMAChunk.Create(Gamma.Value));
            }
            if (ChromaticityPoints != null) {
                chunks_insert.Add(PNGcHRMChunk.Create(ChromaticityPoints));
            }
            if (RenderingIntent != null) {
                chunks_insert.Add(PNGsRGBChunk.Create(RenderingIntent.Value));
            }
            if (RecordTime != null) {
                chunks_insert.Add(PNGtIMEChunk.Create(RecordTime.Value));
            }

            chunks_insert.Add(PNGpHYsChunk.Create(Dpi));

            chunks.InsertRange(1, chunks_insert);
        }

        /// <summary>クローン</summary>
        public object Clone() {
            PNGMetadata metadata = new() {
                Gamma = this.Gamma,
                ChromaticityPoints = this.ChromaticityPoints != null ? (PNGChromaticityPoints)this.ChromaticityPoints.Clone() : null,
                RenderingIntent = this.RenderingIntent,
                RecordTime = this.RecordTime,
                Dpi = this.Dpi
            };

            return metadata;
        }

        public override string ToString() {
            StringBuilder str = new();
            
            str.Append($"{nameof(Dpi)}: {Dpi}");
            
            if (Gamma is not null) {
                str.Append($" {nameof(Gamma)}: {Gamma}");
            }

            if (ChromaticityPoints is not null) {
                str.Append($" {nameof(ChromaticityPoints)}: {ChromaticityPoints}");
            }

            if (RenderingIntent is not null) {
                str.Append($" {nameof(RenderingIntent)}: {RenderingIntent}");
            }

            if (RecordTime is not null) {
                str.Append($" {nameof(RecordTime)}: {RecordTime}");
            }

            return str.ToString();
        }
    }

    /// <summary>色域変換時に優先するレンダリングオプション</summary>
    /// <remarks>sRGBチャネル</remarks>
    public enum PNGRenderingIntents {
        /// <summary>発色ができるだけ自然に見えるように変換する (既定値)</summary>
        Perceptual = 0,

        /// <summary>白色点を固定して変換する</summary>
        RelativeColorimetric = 1,

        /// <summary>できるだけ彩度が変わらないように変換する</summary>
        Saturation = 2,

        /// <summary>印刷時にできるだけ発色を維持する</summary>
        AbsoluteColorimetric = 3
    }

    /// <summary>CIExy色度図代表点</summary>
    /// <remarks>cHRMチャネル 既定値 : D65,sRGB</remarks>
    [DebuggerDisplay("{ToString(),nq}")]
    public class PNGChromaticityPoints : ICloneable {

        /// <summary>白色点 x</summary>
        public PNGFixed WhiteX { get; set; } = 0.3127;
        /// <summary>白色点 y</summary>
        public PNGFixed WhiteY { get; set; } = 0.3290;

        /// <summary>赤色点 x</summary>
        public PNGFixed RedX { get; set; } = 0.64;
        /// <summary>赤色点 y</summary>
        public PNGFixed RedY { get; set; } = 0.33;

        /// <summary>緑色点 x</summary>
        public PNGFixed GreenX { get; set; } = 0.30;
        /// <summary>緑色点 y</summary>
        public PNGFixed GreenY { get; set; } = 0.60;

        /// <summary>青色点 x</summary>
        public PNGFixed BlueX { get; set; } = 0.15;

        /// <summary>青色点 y</summary>
        public PNGFixed BlueY { get; set; } = 0.06;

        /// <summary>既定値 : D65,sRGB</summary>
        /// <remarks>デフォルトコンストラクタに同じ</remarks>
        public static PNGChromaticityPoints Default { get; } = new();

        /// <summary>文字列化</summary>
        public override string ToString() {
            return $"W:{WhiteX},{WhiteY} R:{RedX},{RedY} G:{GreenX},{GreenY} B:{BlueX},{BlueY}";
        }

        /// <summary>クローン</summary>
        public object Clone() {
            PNGChromaticityPoints points = new() {
                WhiteX = this.WhiteX,
                WhiteY = this.WhiteY,
                RedX = this.RedX,
                RedY = this.RedY,
                GreenX = this.GreenX,
                GreenY = this.GreenY,
                BlueX = this.BlueX,
                BlueY = this.BlueY,
            };

            return points;
        }
    }

    /// <summary>PNGファイル形式固定小数点</summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public struct PNGFixed {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private UInt32 val;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const UInt32 val_times = 100000;

        /// <summary>doubleへ変換</summary>
        public static explicit operator double(PNGFixed fixedval) {
            return (double)fixedval.val / val_times;
        }

        /// <summary>fixedvalへ変換</summary>
        public static implicit operator PNGFixed(double val) {
            PNGFixed fixedval = new() {
                val = (val > 0) ? ((val * val_times <= UInt32.MaxValue) ? (UInt32)(val * val_times) : UInt32.MaxValue) : 0
            };

            return fixedval;
        }

        /// <summary>uint32へ変換</summary>
        public static explicit operator UInt32(PNGFixed fixedval) {
            return fixedval.val;
        }

        /// <summary>fixedvalへ変換</summary>
        public static implicit operator PNGFixed(UInt32 val) {
            PNGFixed fixedval = new() {
                val = val
            };

            return fixedval;
        }

        /// <summary>文字列化</summary>
        public override string ToString() {
            return $"{(double)val / val_times}";
        }
    }
}
