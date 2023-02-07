using System.IO;

namespace PNGReadWrite {
    internal class PNGChunk {
        private static readonly UInt32[] crc_table;
        internal static byte[] Signature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

#pragma warning disable CS8625
        protected byte[] type = null, data = null;
#pragma warning restore CS8625

        internal int Length => type.Length + data.Length + 8;
        internal byte[] Data => data;
        internal string Type => PNGBitConverter.ToString(type);

        protected internal static PNGChunk Create(byte[] type, byte[] data) {
            if (type == null || data == null) {
                throw new ArgumentNullException($"{nameof(type)},{nameof(data)}");
            }
            if (type.Length != 4) {
                throw new ArgumentException("Invalid length.", nameof(type));
            }
            if (data.LongLength > int.MaxValue / 2) {
                throw new OverflowException("Too long data length.");
            }

            PNGChunk chunk = new() {
                type = (byte[])type.Clone(),
                data = (byte[])data.Clone()
            };

            return chunk;
        }

        internal static PNGChunk FromBytes(byte[] bin, int start_index, bool crc_check = true) {
            if (bin == null) {
                throw new ArgumentNullException(nameof(bin));
            }
            if (start_index > int.MaxValue / 2) {
                throw new OverflowException("Too long data length.");
            }
            if (bin.LongLength - start_index < 12) {
                throw new InvalidDataException("Invalid chunk exists.");
            }

            UInt32 length = PNGBitConverter.ToUInt32(bin, start_index);
            if (length > int.MaxValue / 2) {
                throw new OverflowException("Too long data length.");
            }
            if (bin.LongLength - start_index < length + 12) {
                throw new InvalidDataException("Invalid chunk exists.");
            }

            byte[] type = new byte[4] { bin[start_index + 4], bin[start_index + 5], bin[start_index + 6], bin[start_index + 7] };

            if (crc_check) {
                UInt32 crc_expect = CRC32(bin.Skip(start_index + 4).Take((int)length + 4));

                UInt32 crc_real = PNGBitConverter.ToUInt32(bin, start_index + (int)length + 8);

                if (crc_expect != crc_real) {
                    throw new InvalidDataException("CRC does not match.");
                }
            }

            byte[] data = bin.Skip(start_index + 8).Take((int)length).ToArray();

            PNGChunk chunk = new() {
                type = type,
                data = data
            };

            return chunk;
        }

        internal static byte[] ToBytes(PNGChunk chunk) {
            byte[] length = PNGBitConverter.FromUInt32((UInt32)chunk.data.Length);
            byte[] crc = PNGBitConverter.FromUInt32(CRC32(chunk.type.Concat(chunk.data)));

            return length.Concat(chunk.type).Concat(chunk.data).Concat(crc).ToArray();
        }

        static PNGChunk() {
            crc_table = new UInt32[256];

            unchecked {
                for (int n = 0, k; n < crc_table.Length; n++) {
                    UInt32 c = (UInt32)n;

                    for (k = 0; k < 8; k++) {
                        c = ((c & 1) == 1) ? (0xEDB88320u ^ (c >> 1)) : (c >> 1);
                    }
                    crc_table[n] = c;
                }
            }
        }

        internal static UInt32 CRC32(IEnumerable<byte> data) {
            UInt32 c = 0xFFFFFFFFu;

            unchecked {
                foreach (byte b in data) {
                    c = crc_table[(c ^ b) & 0xFF] ^ (c >> 8);
                }
            }

            c ^= 0xFFFFFFFFu;

            return c;
        }

        internal static List<PNGChunk> EnumerateChunk(byte[] bin, bool crc_check = false) {
            if (bin.LongLength > int.MaxValue / 2) {
                throw new ArgumentException("Too long data length.");
            }

            List<PNGChunk> chunks = new();

            int start_index = Signature.Length;

            while (start_index < bin.Length) {
                PNGChunk chunk = FromBytes(bin, start_index, crc_check);
                start_index += chunk.Length;

                chunks.Add(chunk);

                if (chunk.Type == "IEND") {
                    break;
                }
            }

            return chunks;
        }

        internal static List<byte> ChunksToBytes(IEnumerable<PNGChunk> chunks) {
            List<byte> bytes = Signature.ToList();

            foreach (PNGChunk chunk in chunks) {
                bytes.AddRange(ToBytes(chunk));
            }

            return bytes;
        }
    }

    internal class PNGgAMAChunk : PNGChunk {
        internal static new string Type => "gAMA";

        internal PNGFixed Gamma => PNGBitConverter.ToUInt32(Data, 0);

        internal PNGgAMAChunk(PNGChunk chunk) {
            if (chunk.Type != Type) {
                throw new ArgumentException("Mismatch chunk type.", nameof(chunk));
            }

            this.type = PNGBitConverter.FromString(chunk.Type);
            this.data = chunk.Data;
        }

        internal static PNGChunk Create(PNGFixed gamma) {
            byte[] type = PNGBitConverter.FromString(Type);
            byte[] data = PNGBitConverter.FromUInt32((UInt32)gamma);

            return Create(type, data);
        }
    }

    internal class PNGsRGBChunk : PNGChunk {
        internal static new string Type => "sRGB";

        internal PNGRenderingIntents RenderingIntent => (PNGRenderingIntents)Data[0];

        internal PNGsRGBChunk(PNGChunk chunk) {
            if (chunk.Type != Type) {
                throw new ArgumentException("Mismatch chunk type.", nameof(chunk));
            }

            this.type = PNGBitConverter.FromString(chunk.Type);
            this.data = chunk.Data;
        }

        internal static PNGChunk Create(PNGRenderingIntents rendering_intent) {
            if (!Enum.IsDefined(typeof(PNGRenderingIntents), rendering_intent)) {
                throw new ArgumentException("Unsupported rendering intents.", nameof(rendering_intent));
            }

            byte[] type = PNGBitConverter.FromString(Type);
            byte[] data = { (byte)rendering_intent };

            return Create(type, data);
        }
    }

    internal class PNGcHRMChunk : PNGChunk {
        internal static new string Type => "cHRM";

        internal PNGcHRMChunk(PNGChunk chunk) {
            if (chunk.Type != Type) {
                throw new ArgumentException("Mismatch chunk type.", nameof(chunk));
            }

            this.type = PNGBitConverter.FromString(chunk.Type);
            this.data = chunk.Data;
        }

        internal PNGChromaticityPoints ChromaticityPoints {
            get {
                PNGChromaticityPoints points = new() {
                    WhiteX = PNGBitConverter.ToUInt32(Data, 0),
                    WhiteY = PNGBitConverter.ToUInt32(Data, 4),
                    RedX = PNGBitConverter.ToUInt32(Data, 8),
                    RedY = PNGBitConverter.ToUInt32(Data, 12),
                    GreenX = PNGBitConverter.ToUInt32(Data, 16),
                    GreenY = PNGBitConverter.ToUInt32(Data, 20),
                    BlueX = PNGBitConverter.ToUInt32(Data, 24),
                    BlueY = PNGBitConverter.ToUInt32(Data, 28),
                };

                return points;
            }
        }
        internal static PNGChunk Create(PNGChromaticityPoints points) {
            byte[] type = PNGBitConverter.FromString(Type);

            List<byte> data = new();

            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.WhiteX));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.WhiteY));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.RedX));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.RedY));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.GreenX));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.GreenY));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.BlueX));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)points.BlueY));

            return Create(type, data.ToArray());
        }
    }

    internal class PNGtIMEChunk : PNGChunk {
        internal static new string Type => "tIME";

        internal PNGtIMEChunk(PNGChunk chunk) {
            if (chunk.Type != Type) {
                throw new ArgumentException("Mismatch chunk type.", nameof(chunk));
            }

            this.type = PNGBitConverter.FromString(chunk.Type);
            this.data = chunk.Data;
        }

        internal DateTime RecordTime {
            get {
                UInt16 year = PNGBitConverter.ToUInt16(Data, 0);
                byte month = Data[2], day = Data[3], hour = Data[4], minute = Data[5], second = Data[6];

                DateTime time = new(year, month, day, hour, minute, second);

                return time;
            }
        }

        internal static PNGChunk Create(DateTime time) {
            byte[] type = PNGBitConverter.FromString(Type);

            List<byte> data = new();

            data.AddRange(PNGBitConverter.FromUInt16((UInt16)time.Year));
            data.AddRange(new byte[] { (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second });

            return Create(type, data.ToArray());
        }
    }

    internal class PNGpHYsChunk : PNGChunk {
        internal static new string Type => "pHYs";

        const double inch_per_meter = 100d / 2.54d;

        internal PNGpHYsChunk(PNGChunk chunk) {
            if (chunk.Type != Type) {
                throw new ArgumentException("Mismatch chunk type.", nameof(chunk));
            }

            this.type = PNGBitConverter.FromString(chunk.Type);
            this.data = chunk.Data;
        }

        internal (double x, double y) Dpi {
            get {
                double dpix = Math.Round(PNGBitConverter.ToUInt32(Data, 0) / inch_per_meter * 32) / 32;
                double dpiy = Math.Round(PNGBitConverter.ToUInt32(Data, 4) / inch_per_meter * 32) / 32;
                
                return (dpix, dpiy);
            }
        }

        internal static PNGChunk Create((double x, double y) dpi) {
            byte[] type = PNGBitConverter.FromString(Type);

            List<byte> data = new();

            data.AddRange(PNGBitConverter.FromUInt32((UInt32)(dpi.x * inch_per_meter)));
            data.AddRange(PNGBitConverter.FromUInt32((UInt32)(dpi.y * inch_per_meter)));
            data.Add(0x01); // meter

            return Create(type, data.ToArray());
        }
    }
}
