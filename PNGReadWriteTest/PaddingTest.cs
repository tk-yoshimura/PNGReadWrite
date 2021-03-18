using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass]
    public class PaddingTest {
        [TestMethod]
        public void EdgePaddingTest() {
            const string dirpath = "../../../testimg/";

            const int width = 255, height = 128;
            
            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new PNGPixelArray(arr);

            PNGPixelArray png_pad = png.EdgePadding(2, 3, 5, 7);

            png_pad.Write(dirpath + "pngedgepad.png");
        }

        [TestMethod]
        public void ZeroPaddingTest() {
            const string dirpath = "../../../testimg/";

            const int width = 255, height = 128;
            
            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new PNGPixelArray(arr);

            PNGPixelArray png_pad = png.ZeroPadding(2, 3, 5, 7);

            png_pad.Write(dirpath + "pngzeropad.png");
        }
    }
}
