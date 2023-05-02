using System;

namespace DatasetParser
{
    public static class CRC32Reversed
    {
        public static uint Compute(byte[] data)
        {
            uint crc = 0xFFFFFFFF;

            for (int i = data.Length - 1; i >= 0; i--)
            {
                byte b = data[i];

                for (int j = 0; j < 8; j++)
                {
                    bool bit = ((crc & 0x80000000) != 0) ^ ((b & 0x80) != 0);
                    crc <<= 1;
                    if (bit)
                    {
                        crc ^= 0x04C11DB7;
                    }
                    b <<= 1;
                }
            }

            return crc ^ 0xFFFFFFFF;
        }
    }
}
