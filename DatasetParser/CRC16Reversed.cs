using System;
using System.IO;

namespace DatasetParser
{
    public class CRC16
    {

        public static byte[] ComputeHash(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            ushort crc = 0x0000;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) > 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    }
                    else
                    {
                        crc = (ushort)(crc >> 1);
                    }
                }
            }

            byte[] crcBytes = BitConverter.GetBytes(crc);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(crcBytes);
            }

            return crcBytes;
        }
    }
}