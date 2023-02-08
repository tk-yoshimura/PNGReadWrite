using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass]
    public class CreateTest {
        [TestMethod]
        public void From2DArrayTransposedTest() {
            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            Assert.AreEqual(width, png.Width);
            Assert.AreEqual(height, png.Height);
            Assert.AreEqual(new PNGPixel((ushort)((width - 1) * 257), (ushort)((height - 2) * 257), 0), png[^1, ^2]);
        }

        [TestMethod]
        public void From2DArrayWithoutTransposeTest() {
            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[height, width];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[y, x] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr, transposed: false);

            Assert.AreEqual(width, png.Width);
            Assert.AreEqual(height, png.Height);
            Assert.AreEqual(new PNGPixel((ushort)((width - 1) * 257), (ushort)((height - 2) * 257), 0), png[^1, ^2]);
        }

        [TestMethod]
        public void From1DArrayTest() {
            const int width = 255, height = 128;

            PNGPixel[] arr = new PNGPixel[width * height];

            for (int x, y = 0, i = 0; y < height; y++) {
                for (x = 0; x < width; x++, i++) {
                    arr[i] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr, width, height);

            Assert.AreEqual(width, png.Width);
            Assert.AreEqual(height, png.Height);
            Assert.AreEqual(new PNGPixel((ushort)((width - 1) * 257), (ushort)((height - 2) * 257), 0), png[^1, ^2]);
        }
    }
}
