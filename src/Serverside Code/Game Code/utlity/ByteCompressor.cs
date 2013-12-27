using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace MushroomsUnity3DExample.utlity
{
    public class ByteCompressor
    {
        public static byte[] Compress(byte[] input)
        {
            byte[] compressesData = null;
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(input, 0, input.Length);
                }
                //Dont get the MemoryStream data before the GZipStream is closed 
                //since it doesn’t yet contain complete compressed data.
                //GZipStream writes additional data including footer information when its been disposed
                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }

        public static byte[] Deflate(byte[] input)
        {
            byte[] compressesData = null;
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (DeflateStream zip = new DeflateStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(input, 0, input.Length);
                }
                //Dont get the MemoryStream data before the GZipStream is closed 
                //since it doesn’t yet contain complete compressed data.
                //GZipStream writes additional data including footer information when its been disposed
                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }

    }
}
