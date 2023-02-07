using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;
using System.Drawing;
using System.IO;

namespace PNGReadWriteTest {
    [TestClass]
    public class ReadWriteTest {
        [TestMethod]
        public void PNGSuiteTest() {
            const string dirpath = "../../../../PngSuite-2017jul19/";
            const string dirpath_result = "../../../../PngSuite-2017jul19_result/";

            var filepaths = Directory.EnumerateFiles(dirpath, "*.png");

            Assert.IsTrue(filepaths.Any(), "Please set PNGSuite path...");

            PNGPixelArray png = new();

            foreach (var filepath in filepaths) {
                string filename = filepath[dirpath.Length..];
                string filename_withoutext = filename[..^".png".Length];

                try {
                    png.Read(filepath);

                    png.Write($"{dirpath_result}{filename_withoutext}_rgb24.png", PNGFormat.RGB24);
                    png.Write($"{dirpath_result}{filename_withoutext}_rgb48.png", PNGFormat.RGB48);
                    png.Write($"{dirpath_result}{filename_withoutext}_rgba32.png", PNGFormat.RGBA32);
                    png.Write($"{dirpath_result}{filename_withoutext}_rgba64.png", PNGFormat.RGBA64);

                    Bitmap bitmap = (Bitmap)png;

                    bitmap.Save($"{dirpath_result}{filename_withoutext}_gdi.png");

                    Console.WriteLine($"Success {filename}");
                    Assert.AreNotEqual('x', filename[0]);
                }
                catch (Exception e) {
                    Console.WriteLine($"Fail    {filename} {e.Message}");
                    Assert.AreEqual('x', filename[0]);
                }
            }
        }

        [TestMethod]
        public void RGBA64Test() {
            const string dirpath = "../../../../testimg/";

            const int width = 1023, height = 1023;

            Random rg = new();

            ushort[] arr = (new ushort[width * height * 4]).Select((v, idx) => (ushort)rg.Next()).ToArray();

            PNGPixelArray png = new(arr, width, height);

            png.Write(dirpath + "rgba64.png", PNGFormat.RGBA64);

            PNGPixelArray png2 = new(dirpath + "rgba64.png");

            Assert.IsTrue(arr.SequenceEqual(png2.Pixels));
        }

        [TestMethod]
        public void RGB48Test() {
            const string dirpath = "../../../../testimg/";

            const int width = 1023, height = 1023;

            Random rg = new();

            ushort[] arr = (new ushort[width * height * 4]).Select((v, idx) => (idx % 4 != 3) ? (ushort)rg.Next() : (ushort)0xFFFF).ToArray();

            PNGPixelArray png = new(arr, width, height);

            png.Write(dirpath + "rgb48.png", PNGFormat.RGB48);

            PNGPixelArray png2 = new(dirpath + "rgb48.png");

            Assert.IsTrue(arr.SequenceEqual(png2.Pixels));
        }

        [TestMethod]
        public void IndexerTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            png.Write(dirpath + "rgba64_4.png", PNGFormat.RGBA64);

            PNGPixelArray png2 = new(dirpath + "rgba64_4.png");

            PNGPixel[,] arr2 = (PNGPixel[,])png2;

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    Assert.AreEqual(arr[x, y], arr2[x, y]);
                }
            }
        }

        [TestMethod]
        public void ChannelsTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 32, height = 16;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = PNGPixel.FromSingle(0.25f, 0.50f, 0.75f, 1.00f);
                }
            }

            PNGPixelArray png = new(arr);

            float[] r = png.RedArray;
            float[] g = png.GreenArray;
            float[] b = png.BlueArray;
            float[] a = png.AlphaArray;
            float[] rgb_chw = png.RGBChannelFirstArray;
            float[] rgb_hwc = png.RGBChannelLastArray;
            float[] rgba_chw = png.RGBAChannelFirstArray;
            float[] rgba_hwc = png.RGBAChannelLastArray;

            Assert.AreEqual(width * height, r.Length);
            Assert.AreEqual(0.25f, r[0], 1e-3f);
            Assert.AreEqual(0.25f, r[width * height - 1], 1e-3f);

            Assert.AreEqual(width * height, g.Length);
            Assert.AreEqual(0.50f, g[0], 1e-3f);
            Assert.AreEqual(0.50f, g[width * height - 1], 1e-3f);

            Assert.AreEqual(width * height, b.Length);
            Assert.AreEqual(0.75f, b[0], 1e-3f);
            Assert.AreEqual(0.75f, b[width * height - 1], 1e-3f);

            Assert.AreEqual(width * height, a.Length);
            Assert.AreEqual(1.00f, a[0], 1e-3f);
            Assert.AreEqual(1.00f, a[width * height - 1], 1e-3f);

            Assert.AreEqual(width * height * 3, rgb_chw.Length);
            Assert.AreEqual(0.25f, rgb_chw[0], 1e-3f);
            Assert.AreEqual(0.25f, rgb_chw[width * height - 1], 1e-3f);
            Assert.AreEqual(0.50f, rgb_chw[width * height], 1e-3f);
            Assert.AreEqual(0.50f, rgb_chw[width * height * 2 - 1], 1e-3f);
            Assert.AreEqual(0.75f, rgb_chw[width * height * 2], 1e-3f);
            Assert.AreEqual(0.75f, rgb_chw[width * height * 3 - 1], 1e-3f);

            Assert.AreEqual(width * height * 3, rgb_hwc.Length);
            Assert.AreEqual(0.25f, rgb_hwc[0], 1e-3f);
            Assert.AreEqual(0.50f, rgb_hwc[1], 1e-3f);
            Assert.AreEqual(0.75f, rgb_hwc[2], 1e-3f);
            Assert.AreEqual(0.25f, rgb_hwc[width * height * 3 - 3], 1e-3f);
            Assert.AreEqual(0.50f, rgb_hwc[width * height * 3 - 2], 1e-3f);
            Assert.AreEqual(0.75f, rgb_hwc[width * height * 3 - 1], 1e-3f);

            Assert.AreEqual(width * height * 4, rgba_chw.Length);
            Assert.AreEqual(0.25f, rgba_chw[0], 1e-3f);
            Assert.AreEqual(0.25f, rgba_chw[width * height - 1], 1e-3f);
            Assert.AreEqual(0.50f, rgba_chw[width * height], 1e-3f);
            Assert.AreEqual(0.50f, rgba_chw[width * height * 2 - 1], 1e-3f);
            Assert.AreEqual(0.75f, rgba_chw[width * height * 2], 1e-3f);
            Assert.AreEqual(0.75f, rgba_chw[width * height * 3 - 1], 1e-3f);
            Assert.AreEqual(1.00f, rgba_chw[width * height * 3], 1e-3f);
            Assert.AreEqual(1.00f, rgba_chw[width * height * 4 - 1], 1e-3f);

            Assert.AreEqual(width * height * 4, rgba_hwc.Length);
            Assert.AreEqual(0.25f, rgba_hwc[0], 1e-3f);
            Assert.AreEqual(0.50f, rgba_hwc[1], 1e-3f);
            Assert.AreEqual(0.75f, rgba_hwc[2], 1e-3f);
            Assert.AreEqual(1.00f, rgba_hwc[3], 1e-3f);
            Assert.AreEqual(0.25f, rgba_hwc[width * height * 4 - 4], 1e-3f);
            Assert.AreEqual(0.50f, rgba_hwc[width * height * 4 - 3], 1e-3f);
            Assert.AreEqual(0.75f, rgba_hwc[width * height * 4 - 2], 1e-3f);
            Assert.AreEqual(1.00f, rgba_hwc[width * height * 4 - 1], 1e-3f);

            PNGPixelArray png_rgb_chw = PNGPixelArray.FromRGBChannelFirstArray(rgb_chw, width, height);
            PNGPixelArray png_rgb_a_chw = PNGPixelArray.FromRGBChannelFirstArray(rgb_chw, a, width, height);
            PNGPixelArray png_rgba_chw = PNGPixelArray.FromRGBAChannelFirstArray(rgba_chw, width, height);

            PNGPixelArray png_rgb_hwc = PNGPixelArray.FromRGBChannelLastArray(rgb_hwc, width, height);
            PNGPixelArray png_rgb_a_hwc = PNGPixelArray.FromRGBChannelLastArray(rgb_hwc, a, width, height);
            PNGPixelArray png_rgba_hwc = PNGPixelArray.FromRGBAChannelLastArray(rgba_hwc, width, height);

            PNGPixelArray png_gray = PNGPixelArray.FromGrayscale(r, width, height);

            png_rgb_chw.Write(dirpath + nameof(png_rgb_chw) + ".png");
            png_rgba_chw.Write(dirpath + nameof(png_rgba_chw) + ".png");
            png_rgb_a_chw.Write(dirpath + nameof(png_rgb_a_chw) + ".png");
            png_rgb_hwc.Write(dirpath + nameof(png_rgb_hwc) + ".png");
            png_rgba_hwc.Write(dirpath + nameof(png_rgba_hwc) + ".png");
            png_rgb_a_hwc.Write(dirpath + nameof(png_rgb_a_hwc) + ".png");
            png_gray.Write(dirpath + nameof(png_gray) + ".png");
        }
    }
}
