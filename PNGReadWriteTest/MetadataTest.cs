using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass]
    public class MetadataTest {
        [TestMethod]
        public void DefaultTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            Console.WriteLine(png.Metadata.Gamma);
            Console.WriteLine(png.Metadata.ChromaticityPoints);
            Console.WriteLine(png.Metadata.RenderingIntent);
            Console.WriteLine(png.Metadata.RecordTime);
            Console.WriteLine(png.Metadata.Dpi);

            png.Write(dirpath + "pngmetadata_defalut.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_defalut.png");

            Assert.AreEqual(png.Metadata.Gamma, png_read.Metadata.Gamma);
            Assert.AreEqual(png.Metadata.ChromaticityPoints.ToString(), png_read.Metadata.ChromaticityPoints.ToString());
            Assert.AreEqual(png.Metadata.RenderingIntent.ToString(), png_read.Metadata.RenderingIntent.ToString());
            Assert.AreEqual(png.Metadata.RecordTime.ToString(), png_read.Metadata.RecordTime.ToString());
            Assert.AreEqual(png.Metadata.Dpi.ToString(), png_read.Metadata.Dpi.ToString());
        }

        [TestMethod]
        public void GammaTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            png.Metadata.Gamma = 0.5;
            Assert.AreEqual(0.5, png.Metadata.Gamma);

            png.Write(dirpath + "pngmetadata_gamma.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_gamma.png");

            Assert.AreEqual(png.Metadata.Gamma, png_read.Metadata.Gamma);
            Assert.AreEqual(png.Metadata.ChromaticityPoints.ToString(), png_read.Metadata.ChromaticityPoints.ToString());
            Assert.AreEqual(png.Metadata.RenderingIntent.ToString(), png_read.Metadata.RenderingIntent.ToString());
            Assert.AreEqual(png.Metadata.RecordTime.ToString(), png_read.Metadata.RecordTime.ToString());
            Assert.AreEqual(png.Metadata.Dpi.ToString(), png_read.Metadata.Dpi.ToString());
        }

        [TestMethod]
        public void ChromaTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            png.Metadata.ChromaticityPoints = new() {
                WhiteX = 0.313,
                WhiteY = 0.330,
                RedX = 0.65,
                RedY = 0.34,
                GreenX = 0.31,
                GreenY = 0.61,
                BlueX = 0.16,
                BlueY = 0.07,
            };
            Assert.AreEqual(0.313, (double)png.Metadata.ChromaticityPoints.WhiteX);

            png.Write(dirpath + "pngmetadata_chroma.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_chroma.png");

            Assert.AreEqual(png.Metadata.Gamma, png_read.Metadata.Gamma);
            Assert.AreEqual(png.Metadata.ChromaticityPoints.ToString(), png_read.Metadata.ChromaticityPoints.ToString());
            Assert.AreEqual(png.Metadata.RenderingIntent.ToString(), png_read.Metadata.RenderingIntent.ToString());
            Assert.AreEqual(png.Metadata.RecordTime.ToString(), png_read.Metadata.RecordTime.ToString());
            Assert.AreEqual(png.Metadata.Dpi.ToString(), png_read.Metadata.Dpi.ToString());
        }

        [TestMethod]
        public void RenderingIntentTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            png.Metadata.RenderingIntent = PNGRenderingIntents.Saturation;
            Assert.AreEqual(PNGRenderingIntents.Saturation, png.Metadata.RenderingIntent);
 
            png.Write(dirpath + "pngmetadata_intent.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_intent.png");

            Assert.AreEqual(png.Metadata.Gamma, png_read.Metadata.Gamma);
            Assert.AreEqual(png.Metadata.ChromaticityPoints.ToString(), png_read.Metadata.ChromaticityPoints.ToString());
            Assert.AreEqual(png.Metadata.RenderingIntent.ToString(), png_read.Metadata.RenderingIntent.ToString());
            Assert.AreEqual(png.Metadata.RecordTime.ToString(), png_read.Metadata.RecordTime.ToString());
            Assert.AreEqual(png.Metadata.Dpi.ToString(), png_read.Metadata.Dpi.ToString());
        }

        [TestMethod]
        public void RecordTimeTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            DateTime time = DateTime.Now + TimeSpan.FromHours(2);

            png.Metadata.RecordTime = time;
            Assert.AreEqual(time, png.Metadata.RecordTime);
 
            png.Write(dirpath + "pngmetadata_recordtime.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_recordtime.png");

            Assert.AreEqual(png.Metadata.Gamma, png_read.Metadata.Gamma);
            Assert.AreEqual(png.Metadata.ChromaticityPoints.ToString(), png_read.Metadata.ChromaticityPoints.ToString());
            Assert.AreEqual(png.Metadata.RenderingIntent.ToString(), png_read.Metadata.RenderingIntent.ToString());
            Assert.AreEqual(png.Metadata.RecordTime.ToString(), png_read.Metadata.RecordTime.ToString());
            Assert.AreEqual(png.Metadata.Dpi.ToString(), png_read.Metadata.Dpi.ToString());
        }

        [TestMethod]
        public void DpiTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);

            png.Metadata.Dpi = (192, 192);
            Assert.AreEqual((192, 192), png.Metadata.Dpi);
 
            png.Write(dirpath + "pngmetadata_dpi.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_dpi.png");

            Assert.AreEqual(png.Metadata.Gamma, png_read.Metadata.Gamma);
            Assert.AreEqual(png.Metadata.ChromaticityPoints.ToString(), png_read.Metadata.ChromaticityPoints.ToString());
            Assert.AreEqual(png.Metadata.RenderingIntent.ToString(), png_read.Metadata.RenderingIntent.ToString());
            Assert.AreEqual(png.Metadata.RecordTime.ToString(), png_read.Metadata.RecordTime.ToString());
            Assert.AreEqual(png.Metadata.Dpi.ToString(), png_read.Metadata.Dpi.ToString());
        }

        [TestMethod]
        public void NoneTest() {
            const string dirpath = "../../../../testimg/";

            const int width = 255, height = 128;

            PNGPixel[,] arr = new PNGPixel[width, height];

            for (int x, y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
                }
            }

            PNGPixelArray png = new(arr);
            png.Metadata.Gamma = null;
            png.Metadata.ChromaticityPoints = null;
            png.Metadata.RenderingIntent = null;
            png.Metadata.RecordTime = null;

            png.Metadata.Dpi = (192, 192);
            Assert.IsNull(png.Metadata.Gamma);
            Assert.IsNull(png.Metadata.ChromaticityPoints);
            Assert.IsNull(png.Metadata.RenderingIntent);
            Assert.IsNull(png.Metadata.RecordTime);
 
            png.Write(dirpath + "pngmetadata_none.png");

            PNGPixelArray png_read = new(dirpath + "pngmetadata_none.png");

            Assert.IsNull(png_read.Metadata.Gamma);
            Assert.IsNull(png_read.Metadata.ChromaticityPoints);
            Assert.IsNull(png_read.Metadata.RenderingIntent);
            Assert.IsNull(png_read.Metadata.RecordTime);
        }
    }
}
