using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass]
    public class PaddingTest {
        [TestMethod]
        public void EdgePaddingTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            PNGPixelArray png_pad = png.EdgePadding(2, 3, 5, 7);

            png_pad.Write(dirpath + "pngedgepad.png");

            Assert.AreEqual(png_pad[0, 0], png[0, 0]);
            Assert.AreEqual(png_pad[1, 4], png[0, 0]);
            Assert.AreEqual(png_pad[2, 5], png[0, 0]);
            Assert.AreEqual(png_pad[^1, ^1], png[^1, ^1]);
            Assert.AreEqual(png_pad[^4, ^8], png[^1, ^1]);
            Assert.AreEqual(png_pad[^3, ^7], png[^1, ^1]);

            Assert.AreEqual(png_pad[0, ^1], png[0, ^1]);
            Assert.AreEqual(png_pad[1, ^8], png[0, ^1]);
            Assert.AreEqual(png_pad[2, ^7], png[0, ^1]);
            Assert.AreEqual(png_pad[^1, 0], png[^1, 0]);
            Assert.AreEqual(png_pad[^4, 4], png[^1, 0]);
            Assert.AreEqual(png_pad[^3, 5], png[^1, 0]);
        }

        [TestMethod]
        public void ZeroPaddingTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            PNGPixelArray png_pad = png.ZeroPadding(2, 3, 5, 7);

            png_pad.Write(dirpath + "pngzeropad.png");

            Assert.AreEqual(png_pad[0, 0], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[1, 4], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[2, 5], png[0, 0]);
            Assert.AreEqual(png_pad[^1, ^1], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[^3, ^7], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[^4, ^8], png[^1, ^1]);

            Assert.AreEqual(png_pad[0, ^1], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[1, ^7], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[2, ^8], png[0, ^1]);
            Assert.AreEqual(png_pad[^1, 0], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[^3, 4], PNGPixel.Transparent);
            Assert.AreEqual(png_pad[^4, 5], png[^1, 0]);
        }
    }
}
