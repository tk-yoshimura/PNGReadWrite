using Microsoft.VisualStudio.TestTools.UnitTesting;
using PNGReadWrite;

namespace PNGReadWriteTest {
    [TestClass()]
    public class PNGPixelArrayTests {
        [TestMethod()]
        public void GetEnumeratorTest() {
            PNGPixelArray pixels = new(4, 3);
            pixels[1, 1] = PNGPixel.Red;
            pixels[^1, ^1] = PNGPixel.Green;

            foreach (PNGPixel pixel in pixels) {
                Console.WriteLine(pixel);
            }
        }
    }
}