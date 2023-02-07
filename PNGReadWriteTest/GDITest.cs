using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;
using System.Drawing;

namespace PNGReadWriteTest {
    [TestClass]
    public class GDITest {
        [TestMethod]
        public void CastTest() {
            const string dirpath = "../../../../testimg/";

            Bitmap bitmap = new(32, 32);
            using (Graphics g = Graphics.FromImage(bitmap)) {
                using Pen pen = new(new SolidBrush(Color.Black));

                g.Clear(Color.Gray);
                g.DrawLine(pen, new Point(0, 0), new Point(31, 31));
            }

            PNGPixelArray png_black = bitmap;
            png_black.Write(dirpath + "gdi_test.png");

            PNGPixelArray png_read = new(dirpath + "gdi_test.png");

            bitmap = (Bitmap)png_read;
            using (Graphics g = Graphics.FromImage(bitmap)) {
                using Pen pen = new(new SolidBrush(Color.White));

                g.DrawLine(pen, new Point(0, 31), new Point(31, 0));
            }

            PNGPixelArray png_cross = bitmap;

            png_cross.Write(dirpath + "gdi_test2.png");

            Assert.AreEqual(PNGPixel.Black, png_cross[0, 0]);
            Assert.AreEqual(PNGPixel.Black, png_cross[^1, ^1]);
            Assert.AreEqual(PNGPixel.White, png_cross[0, ^1]);
            Assert.AreEqual(PNGPixel.White, png_cross[^1, 0]);
        }
    }
}
