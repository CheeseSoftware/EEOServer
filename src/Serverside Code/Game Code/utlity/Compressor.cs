using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MushroomsUnity3DExample.utlity
{
    public class Compressor
    {
        /// <summary>
        /// Only posetive values are allowed!
        /// Data stored as:
        /// [quad] main;
        /// [quad(complex)] = [(byte) isSimple-booleans], [quad], [quad], [quad], [quad];
        /// [quad(simple)] = [blockoutput];
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void CompressQuadTree(short[,] input, Stack<byte> output, short x, short y, short width, short height)
        {
            if (width <= 2 && height <= 2)
            {
                if (width == 1 && height == 1)
                {
                    AddShortToByteStack(output, input[x, y]);
                }
                else if (width == 2 && height == 1)
                {
                    AddShortToByteStack(output, input[x, y]);
                    AddShortToByteStack(output, input[x + 1, y]);
                }
                else if (width == 1 && height == 2)
                {
                    AddShortToByteStack(output, input[x, y]);
                    AddShortToByteStack(output, input[x, y + 1]);
                }
                else if (width == 2 && height == 2)
                {
                    AddShortToByteStack(output, input[x, y]);
                    AddShortToByteStack(output, input[x + 1, y]);
                    AddShortToByteStack(output, input[x, y + 1]);
                    AddShortToByteStack(output, input[x + 1, y + 1]);
                }
                return;
            }

            short x1;
            short y1;
            short x2;
            short y2;
            short Id;

            // This is also 4 booleans(rather 8, to be precise).
            // They tell if the area is filled with 1 simple block or more complex. (True: simple, False: complex)
            byte finalQuadDataByte = 0;

            Action<byte> lambda = null;
            lambda = new Action<byte>(b=>
                {
                    switch (b)
                    {
                        // Strong values take half or more than half of the size,
                        // weak values are take half or more than half of the size.
                        // Example: weak: 3/2 = 1, strong: 3-3/2 = 2 
                        case 0:
                            //weak x, weak y
                            if (width == 1 || height == 1)
                                return;

                            x1 = x;
                            y1 = y;
                            x2 = (short)(x + width / 2);
                            y2 = (short)(y + height / 2);
                            break;

                        case 1:
                            //strong x, weak y
                            if (height == 1)
                                return;

                            x1 = (short)(x + width / 2);
                            y1 = (short)(y);
                            x2 = (short)(x + width);
                            y2 = (short)(y + height / 2);
                            break;

                        case 2:
                            //weak x, strong y
                            if (width == 1)
                                return;

                            x1 = (short)(x);
                            y1 = (short)(y + height / 2);
                            x2 = (short)(x + width / 2);
                            y2 = (short)(y + height);
                            break;

                        case 3:
                            //strong x, strong y
                            x1 = (short)(x + width / 2);
                            y1 = (short)(y + height / 2);
                            x2 = (short)(x + width);
                            y2 = (short)(y + height);
                            break;

                        default:
                            throw new Exception(b.ToString() + " is an invalid quadtree child index. It can only be 0,1,2 or 3.");
                    }

                    Id = getType(input, x1, y1, x2, y2);

                    if (Id == -1)
                        throw new Exception("Negative value(-1) found in data! Only possetive values should be used. Negative values may be allowed, but -1 is forbidden!");

                    bool isSimple = (Id != -1);

                    if (isSimple)
                        finalQuadDataByte |= (byte)(1<< b);

                    if (b > 0) //quadtree does not have more than 4 children!
                    {
                        // Goes to back to the beginning...
                        lambda((byte)(b - 1));
                    }
                    else
                    {
                        output.Push(finalQuadDataByte);
                    }

                    if (isSimple)
                    {
                        AddShortToByteStack(output, Id);
                    }
                    else
                    {
                        // declares another quad inside the square
                        CompressQuadTree(input, output, x1, y1, (short)(x2 - x1), (short)(y2 - y1));
                    }

                    // Goes back to the end...
                    return;

                });

            // starts from the end and ends with the end.
            // 3-> 2-> 1-> 0-> 1-> 2-> 3
            lambda(3);
        }

        public static void DecompressQuadTree(short[,] output, Stack<byte> input, short x, short y, short width, short height)
        {
            if (width == 0 || height == 0)
                return;
            
            if (width == 1 && height == 1)
            {
                output[x, y] = input.Pop();
            }

            DecompressQuadTree(output, input, 
                x,
                y,
                (short)(width / 2),
                (short)(height / 2));

            DecompressQuadTree(output, input,
                (short)(x + width / 2),
                y,
                (short)(width - width / 2),
                (short)(height / 2));

            DecompressQuadTree(output, input,
                x,
                (short)(y + height),
                (short)(width / 2),
                (short)(height - height / 2));

            DecompressQuadTree(output, input, 
                x,
                y, 
                (short)(width / 2),
                (short)(height / 2));
        }

        public static void CompressBinaryTree(short[] input, Stack<byte> output, short start, short end)
        {
            int length = end - start;
            if (length <= 2)
            {
                if (length == 1)
                {
                    AddShortToByteStack(output, input[start]);
                }
                else if (length == 2)
                {
                    AddShortToByteStack(output, input[start]);
                }
                return;
            }

            short l_start;
            short l_end;
            short l_id;

            // This is also 2 booleans(rather 8, to be precise).
            // They tell if the array is filled with 1 simple block or more complex. (True: simple, False: complex)
            byte finalQuadDataByte = 0;

            Action<byte> lambda = null;
            lambda = new Action<byte>(b=>
                {
                    if (b == 0)
                    {
                        l_start = start;
                        l_end = (short)(start + length / 2);
                    }
                    else
                    {
                        l_start = (short)(start + length / 2);
                        l_end = end;
                    }

                    l_id = getType(input, l_start, l_end);

                    if (l_id == -1)
                        throw new Exception("Negative value(-1) found in data! Only possetive values should be used. Negative values may be allowed, but -1 is forbidden!");

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
                        output.Push(finalQuadDataByte);
                    }

                    if (isSimple)
                    {
                        AddShortToByteStack(output, l_id);
                    }
                    else
                    {
                        // declares another quad inside the square
                        CompressBinaryTree(input, output, l_start, l_end);
                    }

                    // Goes back to the end...
                    return;
                });

            lambda(1);
        }

        public static void DecompressBinaryTree(short[] input, Stack<byte> output, short start, short end)
        {
            
        }

        private static short getType(short[] input, short start, short end)
        {
            short blockId = input[start++];

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

            for (; y1 < y2; y1++)
            {
                for (; x1 < x2; x1++)
                {
                    if (input[x1, y1] != blockId)
                        return -1;
                }
            }

            return blockId;
        }

        private static void AddShortToByteStack(Stack<byte> output, short value)
        {
            output.Push((byte)(value>>8));
            output.Push((byte)(value & 0xFF));
        }
    }
}
