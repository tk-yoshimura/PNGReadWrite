using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass]
    public class RegionTest {
        [TestMethod]
        public void RegionCopyDrawTest() {
            const string dirpath = "../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new PNGPixelArray(arr);

            PNGPixelArray png_copy = png.RegionCopy(10, 20, width - 20, height - 30);

            png_copy.Write(dirpath + "pngcopy.png");

            PNGPixelArray png_draw = new PNGPixelArray(width, height);

            png_draw.RegionDraw(png_copy, 10, 20);

            png_draw.Write(dirpath + "pngdraw.png");
        }
    }
}
