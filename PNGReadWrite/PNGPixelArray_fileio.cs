using System;
using System.IO;

namespace PNGReadWrite {
    public partial class PNGPixelArray {

        /// <summary>ファイル読み込み</summary>
        /// <param name="filepath">ファイルパス</param>
        /// <param name="crc_check">CRCを確認するか</param>
        /// <exception cref="FileNotFoundException">ファイルが存在しないとき</exception>
        /// <exception cref="FileFormatException">ファイル形式が不正であるとき</exception>
        /// <exception cref="NotSupportedException">コーデックが対応していないとき</exception>
        /// <exception cref="InvalidDataException">CRCまたはチャンクが不正であるとき</exception>
        /// <exception cref="OverflowException">データサイズが1GB以上、その他オーバーフローが発生したとき</exception>
        /// <remarks>フォーマットがRGB/RGBAかつ色深度が8/16であるときWICで読み込み、そうでないときはGDI+で読み込む</remarks>
        public void Read(string filepath, bool crc_check = true) {
            if (!File.Exists(filepath)) {
                throw new FileNotFoundException(filepath);
            }

            using (FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                Read(stream, crc_check);
            }
        }

        /// <summary>ファイル書き込み</summary>
        public void Write(string filepath, PNGFormat format = PNGFormat.RGBA32) {
            using (Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write)) {
                Write(stream, format);
            }
        }
    }
}
