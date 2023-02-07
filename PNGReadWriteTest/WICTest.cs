using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PNGReadWriteTest {
    [TestClass]
    public class WICTest {
        [TestMethod]
        public void CastTest() {
            const string dirpath = "../../../../testimg/";

            WriteableBitmap bitmapsource = new(32, 32, 96d, 96d, PixelFormats.Rgba64, null);

            PNGPixelArray png = bitmapsource;

            png.Write(dirpath + "wic_test.png");

            png.Metadata.Dpi = (32, 32);

            bitmapsource = new WriteableBitmap((BitmapSource)png);

            Assert.AreEqual(32, bitmapsource.DpiX);
        }
    }
}
