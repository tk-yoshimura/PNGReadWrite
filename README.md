# PNGReadWrite
 PNG file IO - Deep color compatible

 [PNG Suite](http://www.schaik.com/pngsuite/) passed

## Requirement
 .NET 6 - windows

## Install
[Download DLL](https://github.com/tk-yoshimura/PNGReadWrite/releases)  
[Download Nuget](https://www.nuget.org/packages/tyoshimura.PNGReadWrite/)  

- Nuget include: System.Drawing.Common
- Enable WPF (*.csproj)
```xml
<PropertyGroup>
    <UseWPF>true</UseWPF>
</PropertyGroup>
```

## Usage
### Create
```cs
const int width = 255, height = 128;

PNGPixel[,] arr = new PNGPixel[width, height];

for (int x, y = 0; y < height; y++) {
    for (x = 0; x < width; x++) {
        arr[x, y] = new PNGPixel((ushort)(x * 257), (ushort)(y * 257), 0);
    }
}

PNGPixelArray png = new(arr);

PNGPixelArray png_copy = png[10..^20, 20..^30];

png_copy.Write(dirpath + "pngcopy_regionclip.png");
```

### Open
``` cs
PNGPixelArray png = new();
png.Read(filepath);
```

``` cs
PNGPixelArray png = new(filepath);
```

### GDI+(Bitmap) &lt; - &gt; PNGPixelArray
```cs
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
```

### WPF(BitmapSource) &lt; - &gt; PNGPixelArray
```cs
WriteableBitmap bitmapsource = new(32, 32, 96d, 96d, PixelFormats.Rgba64, null);

PNGPixelArray png = bitmapsource;

png.Write(dirpath + "wic_test.png");

png.Metadata.Dpi = (32, 32);

bitmapsource = new WriteableBitmap((BitmapSource)png);

Assert.AreEqual(32, bitmapsource.DpiX);
```

See also: [Tests](https://github.com/tk-yoshimura/PNGReadWrite/tree/main/PNGReadWriteTest)

## Licence
[MIT](https://github.com/tk-yoshimura/PNGReadWrite/blob/main/LICENSE)

## Author

[tk-yoshimura](https://github.com/tk-yoshimura)
