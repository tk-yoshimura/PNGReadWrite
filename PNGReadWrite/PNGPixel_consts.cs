namespace PNGReadWrite {

    /// <summary>PNGピクセル</summary>
    public partial struct PNGPixel {
        public static PNGPixel Transparent { get; }
            = new PNGPixel(0, 0, 0, 0);

        public static PNGPixel Black { get; }
            = new PNGPixel(0x0000, 0x0000, 0x0000);
        public static PNGPixel Gray { get; }
            = new PNGPixel(0x8080, 0x8080, 0x8080);
        public static PNGPixel White { get; }
            = new PNGPixel(0xFFFF, 0xFFFF, 0xFFFF);

        public static PNGPixel Red { get; }
            = new PNGPixel(0xFFFF, 0x0000, 0x0000);
        public static PNGPixel Yellow { get; }
            = new PNGPixel(0xFFFF, 0xFFFF, 0x0000);
        public static PNGPixel Lime { get; }
            = new PNGPixel(0x0000, 0xFFFF, 0x0000);
        public static PNGPixel Cyan { get; }
            = new PNGPixel(0x0000, 0xFFFF, 0xFFFF);
        public static PNGPixel Blue { get; }
            = new PNGPixel(0x0000, 0x0000, 0xFFFF);
        public static PNGPixel Magenta { get; }
            = new PNGPixel(0xFFFF, 0x0000, 0xFFFF);

        public static PNGPixel Green { get; }
            = new PNGPixel(0x0000, 0x8080, 0x0000);
        public static PNGPixel Orange { get; }
            = new PNGPixel(0xFFFF, 0xA5A5, 0x0000);
        public static PNGPixel Pink { get; }
            = new PNGPixel(0xFFFF, 0x6969, 0xB4B4);
        public static PNGPixel Violet { get; }
            = new PNGPixel(0xEEEE, 0x8282, 0xEEEE);
    }
}
