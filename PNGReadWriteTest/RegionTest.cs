﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass]
    public class RegionTest {
        [TestMethod]
        public void RegionCopyOverwriteTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            PNGPixelArray png_copy = png.RegionCopy(10, 20, width - 20, height - 30);

            Assert.AreEqual(width - 20, png_copy.Width);
            Assert.AreEqual(height - 30, png_copy.Height);
            Assert.AreEqual(arr[10, 20], png_copy[0, 0]);

            png_copy.Write(dirpath + "pngcopy.png");

            PNGPixelArray png_draw = new(width, height);

            png_draw.RegionOverwrite(png_copy, 20, 30);

            png_draw.Write(dirpath + "pngoverwrite.png");

            Assert.AreEqual(arr[10, 20], png_draw[20, 30]);
        }

        [TestMethod]
        public void RegionCopyOverwriteIndexderTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            PNGPixelArray png_copy = png[10..^20, 20..^30];

            Assert.AreEqual(width - 30, png_copy.Width);
            Assert.AreEqual(height - 50, png_copy.Height);
            Assert.AreEqual(arr[10, 20], png_copy[0, 0]);

            png_copy.Write(dirpath + "pngcopy_indexer.png");

            PNGPixelArray png_draw = new(width, height);

            png_draw[20..(20 + png_copy.Width), 30..(30 + png_copy.Height)] = png_copy;

            png_draw.Write(dirpath + "pngoverwrite_indexer.png");

            Assert.AreEqual(arr[10, 20], png_draw[20, 30]);
        }
    }
}
