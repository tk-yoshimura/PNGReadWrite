using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>データストリーム読み込み</summary>
        /// <param name="stream">データストリーム</param>
        /// <param name="crc_check">CRCを確認するか</param>
        /// <exception cref="FileFormatException">ファイル形式が不正であるとき</exception>
        /// <exception cref="NotSupportedException">コーデックが対応していないとき</exception>
        /// <exception cref="InvalidDataException">CRCまたはチャンクが不正であるとき</exception>
        /// <exception cref="OverflowException">データサイズが1GB以上、その他オーバーフローが発生したとき</exception>
        /// <remarks>フォーマットがRGB/RGBAかつ色深度が8/16であるときWICで読み込み、そうでないときはGDI+で読み込む</remarks>
        [SuppressMessage("Interoperability", "CA1416")]
        public void Read(Stream stream, bool crc_check = true) {
            Clear();

            BitmapSource? wic_bitmap = null;

            try {
                using MemoryStream stream_memory = new();
                stream.CopyTo(stream_memory);

                byte[] bytes = bytes = stream_memory.ToArray();

                var decoder = new PngBitmapDecoder(stream,
                    BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile | BitmapCreateOptions.IgnoreImageCache,
                    BitmapCacheOption.OnLoad);

                if (decoder.Frames.Count < 1 || decoder.Frames[0] == null) {
                    throw new FileFormatException("Chunk does not exist.");
                }

                wic_bitmap = decoder.Frames[0].Clone();

                List<PNGChunk> chunks = PNGChunk.EnumerateChunk(bytes, crc_check);

                Metadata.Read(chunks);
            }
            catch (System.Runtime.InteropServices.COMException e) {
                throw new FileFormatException(e.Message);
            }

            if (wic_bitmap.Format.ToPNGFormat() != PNGFormat.Undefined) {
                FromWICBitmap(wic_bitmap);
            }
            else {
                Bitmap? gdi_bitmap;
                try {
                    gdi_bitmap = (Bitmap)Image.FromStream(stream);
                }
                catch (System.Runtime.InteropServices.COMException e) {
                    throw new FileFormatException(e.Message);
                }

                FromGDIBitmap(gdi_bitmap);
            }
        }

        /// <summary>データストリーム書き込み</summary>
        public void Write(Stream stream, PNGFormat format = PNGFormat.RGBA32) {
            BitmapSource bitmap = ToWICBitmap(format);

            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);

            encoder.Frames.Add(frame);

            encoder.Interlace = PngInterlaceOption.Off;

            using MemoryStream stream_memory = new();
            encoder.Save(stream_memory);

            byte[] bytes = stream_memory.ToArray();

            List<PNGChunk> chunks = PNGChunk.EnumerateChunk(bytes, crc_check: false);
            Metadata.Write(chunks);

            bytes = PNGChunk.ChunksToBytes(chunks).ToArray();

            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
