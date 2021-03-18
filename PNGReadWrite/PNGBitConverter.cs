using System;
using System.Text;

namespace PNGReadWrite {
    internal static class PNGBitConverter {
        internal static byte[] FromUInt32(UInt32 value) {
            unchecked {
                return new byte[4] { (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF) };
            }
        }

        internal static UInt32 ToUInt32(byte[] value, int start_index) {
            unchecked {
                return ((UInt32)value[start_index] << 24) | ((UInt32)value[start_index + 1] << 16) | ((UInt32)value[start_index + 2] << 8) | value[start_index + 3];
            }
        }

        internal static byte[] FromUInt16(UInt16 value) {
            unchecked {
                return new byte[2] { (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF) };
            }
        }

        internal static UInt16 ToUInt16(byte[] value, int start_index) {
            unchecked {
                return (UInt16)(((UInt32)value[start_index] << 8) | value[start_index + 1]);
            }
        }

        internal static byte[] FromString(string value) {
            return Encoding.ASCII.GetBytes(value);
        }

        internal static string ToString(byte[] value) {
            return Encoding.ASCII.GetString(value);
        }
    }
}
