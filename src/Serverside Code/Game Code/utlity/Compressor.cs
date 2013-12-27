using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace MushroomsUnity3DExample.utlity
{
    public class Compressor
    {
        public static Pair<bool, byte[]> CompressWorld(short[,] world, short width, short height)
        {
            Queue<byte> worldData = new Queue<byte>();
            short[] cornerBlocks = new short[796];// 200+200+198+198

            short i = 0;
            short j = width;
            short k = 0;

            for (; i < j; i++) // top
            {
                cornerBlocks[i] = world[i, 0];
            }

            k = j;
            j += width;

            for (; i < j; i++) // bottom
            {
                cornerBlocks[i] = world[i - k, 199];
            }

            k = (short)(j - 1);
            j += (short)(height - 2);

            for (; i < j; i++) // left
            {
                cornerBlocks[i] = world[0, i - k];
            }

            k = (short)(j - 1);
            j += (short)(height - 2);

            for (; i < j; i++) // right
            {
                cornerBlocks[i] = world[199, i - k];
            }

            worldData.Enqueue((byte)(width >> 8));
            worldData.Enqueue((byte)(width & 0xFF));

            worldData.Enqueue((byte)(height >> 8));
            worldData.Enqueue((byte)(height & 0xFF));

            CompressQuadTree(world, worldData, 1, 1, (short)(width-1), (short)(height-1));
            CompressBinaryTree(cornerBlocks, worldData, 0, j);

            byte[] raw = worldData.ToArray();
            byte[] deflated = ByteCompressor.Deflate(raw);

            if (deflated.Length < raw.Length)
                return new Pair<bool, byte[]>(true, deflated);//ZipCompressor.Compress(worldData.ToArray());
            else
                return new Pair<bool, byte[]>(false, raw);
        }

        public static short[,] DecompressWorld(bool iDeflated, byte[] data)
        {
            QueueReader<byte> input;
            short[,] world;
            short[] cornerBlocks;
            short width;
            short height;

            input = new QueueReader<byte>(data);

            width = DequeueShortAsBytes(input);
            height = DequeueShortAsBytes(input);

            world = new short[width, height];
            cornerBlocks = new short[2*width + 2*height - 4];

            DecompressQuadTree(world, input, 1, 1, (short)(width - 1), (short)(height - 1));
            DecompressBinaryTree(cornerBlocks, input, 0, (short)cornerBlocks.Length);

            // Put corner blocks to the world.
            short i = 0;
            short j = width;
            short k = 0;

            for (; i < j; i++) // top
            {
                world[i, 0] = cornerBlocks[i];
            }

            k = j;
            j += width;

            for (; i < j; i++) // bottom
            {
                world[i - k, 199] = cornerBlocks[i];
            }

            k = (short)(j - 1);
            j += (short)(height - 2);

            for (; i < j; i++) // left
            {
                world[0, i - k] = cornerBlocks[i];
            }

            k = (short)(j - 1);
            j += (short)(height - 2);

            for (; i < j; i++) // right
            {
                world[199, i - k] = cornerBlocks[i];
            }

            return world;
        }

        /// <summary>
        /// Only posetive values are allowed!
        /// Data stored as:
        /// [quad] main;
        /// [quad(complex)] = [(byte) isSimple-booleans], [quad], [quad], [quad], [quad];
        /// [quad(simple)] = [(short) blockoutput];
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void CompressQuadTree(short[,] input, Queue<byte> output, short x1, short y1, short x2, short y2)
        {
            short width = (short)(x2 - x1);
            short height = (short)(y2 - y1);

            short middleX;
            short middleY;

            byte finalQuadDataByte;

            if (width == 0 || height == 0)
                return;

            if (width <= 2 && height <= 2)
            {
                if (width == 1 && height == 1)
                {
                    EnqueueShortAsBytes(output, input[x1, y1]);
                }
                else if (width == 2 && height == 1)
                {
                    EnqueueShortAsBytes(output, input[x1, y1]);
                    EnqueueShortAsBytes(output, input[x1+1, y1]);
                }
                else if (width == 1 && height == 2)
                {
                    EnqueueShortAsBytes(output, input[x1, y1]);
                    EnqueueShortAsBytes(output, input[x1, y1+1]);
                }
                else if (width == 2 && height == 2)
                {
                    EnqueueShortAsBytes(output, input[x1, y1]);
                    EnqueueShortAsBytes(output, input[x1+1, y1]);
                    EnqueueShortAsBytes(output, input[x1, y1+1]);
                    EnqueueShortAsBytes(output, input[x1+1, y1+1]);
                }
                return;
            }

            middleX = (short)(x1 + (width >> 1));
            middleY = (short)(y1 + (height >> 1));

            
            // This is also 4 booleans(rather 8, to be precise).
            // They tell if the area is filled with 1 simple block or more complex. (True: simple, False: complex)
            finalQuadDataByte = 0;

            Action<byte> lambda = null;
            lambda = new Action<byte>(b =>
                {
                    short l_x1;
                    short l_y1;
                    short l_x2;
                    short l_y2;
                    short l_id;

                    switch (b)
                    {
                        // Strong values take half or more than half of the size,
                        // weak values are take half or more than half of the size.
                        // Example: weak: 3/2 = 1, strong: 3-3/2 = 2 
                        case 0:
                            //weak x, weak y
                            if (width == 1 || height == 1)
                            {
                                output.Enqueue(finalQuadDataByte);
                                return;
                            }

                            l_x1 = x1;
                            l_y1 = y1;
                            l_x2 = middleX;
                            l_y2 = middleY;
                            break;

                        case 1:
                            //strong x, weak y
                            if (height == 1)
                            {
                                // Goes to back to the beginning...
                                lambda(0);
                                return;
                            }

                            l_x1 = middleX;
                            l_y1 = y1;
                            l_x2 = x2;
                            l_y2 = middleY;
                            break;

                        case 2:
                            //weak x, strong y
                            if (width == 1)
                            {
                                // Goes to back to the beginning...
                                lambda(1);
                                return;
                            }

                            l_x1 = x1;
                            l_y1 = middleY;
                            l_x2 = middleX;
                            l_y2 = y2;
                            break;

                        case 3:
                            //strong x, strong y
                            l_x1 = middleX;
                            l_y1 = middleY;
                            l_x2 = x2;
                            l_y2 = y2;
                            break;

                        default:
                            throw new Exception(b.ToString() + " is an invalid quadtree child index. It can only be 0,1,2 or 3.");
                    }

                    l_id = getType(input, l_x1, l_y1, l_x2, l_y2);

                    bool isSimple = (l_id != -1);

                    if (isSimple)
                        finalQuadDataByte |= (byte)(1 << b);

                    if (b > 0) //quadtree does not have more than 4 children!
                    {
                        // Goes to back to the beginning...
                        lambda((byte)(b - 1));
                    }
                    else
                    {
                        output.Enqueue(finalQuadDataByte);
                    }

                    if (isSimple)
                    {
                        EnqueueShortAsBytes(output, l_id);
                    }
                    else
                    {
                        // declares another quad inside the square
                        CompressQuadTree(input, output, l_x1, l_y1, l_x2, l_y2);
                    }

                    // Goes back to the end...
                    return;

                });

            // starts from the end and ends with the end.
            // 3-> 2-> 1-> 0-> 1-> 2-> 3
            lambda(3);
        }

        public static void DecompressQuadTree(short[,] output, QueueReader<byte> input, short x1, short y1, short x2, short y2) //public static void DecompressQuadTree(short[,] output, QueueReader<byte> input, short x, short y, short width, short height)
        {
            short width = (short)(x2 - x1);
            short height = (short)(y2 - y1);

            short middleX;
            short middleY;

            int binaryDataByte;
            //byte id;

            if (width == 0 || height == 0)
                return;

            if (width <= 2 && height <= 2)
            {
                for (int yy = y1; yy < y2; yy++) //output[x, y] = input.Dequeue();
                {
                    for (int xx = x1; xx < x2; xx++)
                        output[xx, yy] = (short)((input.Dequeue()<<8) | input.Dequeue());
                }
                return;
            }

            middleX = (short)(x1 + (width >> 1));//(x1) - (x1+x2 >> 1);
            middleY = (short)(y1 + (height >> 1));//(y1) - (y1+y2 >> 1);

            binaryDataByte = input.Dequeue();

            Action<byte> lambda = new Action<byte>(b =>
                {
                    short l_x1;
                    short l_y1;
                    short l_x2;
                    short l_y2;
                    short l_id;

                    switch (b)
                    {
                        case 0:
                            l_x1 = x1;
                            l_y1 = y1;
                            l_x2 = middleX;
                            l_y2 = middleY;
                            break;
                        case 1:
                            l_x1 = middleX;
                            l_y1 = y1;
                            l_x2 = x2;
                            l_y2 = middleY;
                            break;
                        case 2:
                            l_x1 = x1;
                            l_y1 = middleY;
                            l_x2 = middleX;
                            l_y2 = y2;
                            break;
                        case 3:
                            //strong x, strong y
                            l_x1 = middleX;
                            l_y1 = middleY;
                            l_x2 = x2;
                            l_y2 = y2;
                            break;
                        default:
                            throw new Exception(b.ToString() + " is an invalid quadtree child index. It can only be 0,1,2 or 3.");
                    }

                    if ((binaryDataByte & (1 << b)) == (1 << b))
                    {
                        l_id = DequeueShortAsBytes(input);
                        for (int yy = l_y1; yy < l_y2; yy++)
                        {
                            for (int xx = l_x1; xx < l_x2; xx++)
                                output[xx, yy] = l_id;
                        }
                    }
                    else
                    {
                        DecompressQuadTree(output, input, l_x1, l_y1, l_x2, l_y2);
                    }
                });

                lambda(0);
                lambda(1);
                lambda(2);
                lambda(3);
        }

        public static void CompressBinaryTree(short[] input, Queue<byte> output, short start, short end)
        {
            short length = (short)(end - start);
            short middle;

            byte finalBinaryDataByte;

            if (length <= 2)
            {
                if (length == 1)
                {
                    EnqueueShortAsBytes(output, input[start]);
                }
                else if (length == 2)
                {
                    EnqueueShortAsBytes(output, input[start]);
                    EnqueueShortAsBytes(output, input[start+1]);
                }
                return;
            }


            middle = (short)(start + length / 2);

            // This is also 2 booleans(rather 8, to be precise).
            // They tell if the array is filled with 1 simple block or more complex. (True: simple, False: complex)
            finalBinaryDataByte = 0;

            Action<byte> lambda = null;
            lambda = new Action<byte>(b =>
                {
                    short l_start;
                    short l_end;
                    short l_id;

                    if (b == 0)
                    {
                        if (length == 1)
                        {
                            output.Enqueue(finalBinaryDataByte);
                            return;
                        }
                        l_start = start;
                        l_end = middle;
                    }
                    else
                    {
                        l_start = middle;
                        l_end = end;
                    }

                    l_id = getType(input, l_start, l_end);

                    //if (l_id == -1)
                    //    throw new Exception("Negative value(-1) found in data! Only possetive values should be used. Negative values may be allowed, but -1 is forbidden!");

                    bool isSimple = (l_id != -1);

                    if (isSimple)
                        finalBinaryDataByte |= (byte)(1 << b);

                    if (b > 0) //quadtree does not have more than 4 children!
                    {
                        // Goes to back to the beginning...
                        lambda((byte)(b - 1));
                    }
                    else
                    {
                        output.Enqueue(finalBinaryDataByte);
                    }

                    if (isSimple)
                    {
                        EnqueueShortAsBytes(output, l_id);
                    }
                    else
                    {
                        // declares another quad inside the square
                        CompressBinaryTree(input, output, l_start, l_end);
                    }

                    // Goes back to the end...
                    return;
                });

            // 1a -> 0ab-> 1b
            lambda(1);
        }

        public static void DecompressBinaryTree(short[] output, QueueReader<byte> input, short start, short end)
        {
            short length = (short)(start - end);
            short middle;

            //tells if the children are simple(true) or complex(false).
            byte binaryDataByte;
            short id;

            if (length <= 2)
            {
                if (length == 0)
                    return;

                if (length == 1)
                {
                    output[start] = DequeueShortAsBytes(input);
                }
                else // length must be 2
                {
                    output[start] = DequeueShortAsBytes(input);
                    output[start + 1] = DequeueShortAsBytes(input);
                }

                return;
            }

            middle = (short)(start + length / 2);


            binaryDataByte = input.Dequeue();

            if ((binaryDataByte & 1) == 1)
            {
                id = DequeueShortAsBytes(input);
                for (int i = start; i < middle; i++)
                    output[i] = id;
            }
            else
            {
                DecompressBinaryTree(output, input, start, middle);
            }


            if ((binaryDataByte & 2) == 2)
            {
                id = DequeueShortAsBytes(input);
                for (int i = middle; i < end; i++)
                    output[i] = id;
            }
            else
            {
                DecompressBinaryTree(output, input, middle, end);
            }
        }

        private static short getType(short[] input, short start, short end)
        {
            short blockId = input[start++];

            if (blockId == -1)
                throw new Exception("Negative value(-1) found in data! Only positive values should be used. Negative values may be allowed, but -1 is forbidden!");

            for (; start < end; start++)
            {
                if (input[start] != blockId)
                    return -1;
            }

            return blockId;
        }

        private static short getType(short[,] input, short x1, short y1, short x2, short y2)
        {
            short blockId = input[x1, y1];

            if (blockId == -1)
                throw new Exception("Negative value(-1) found in data! Only positive values should be used. Negative values may be allowed, but -1 is forbidden!");

            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    if (input[x, y] != blockId)
                        return -1;
                }
            }

            return blockId;
        }

        private static void EnqueueShortAsBytes(Queue<byte> output, short value)
        {
            output.Enqueue((byte)(value >> 8));
            output.Enqueue((byte)(value & 0xFF));
        }

        private static short DequeueShortAsBytes(QueueReader<byte> input)
        {
            return (short)((input.Dequeue() << 8) | input.Dequeue());
        }
    }
}
