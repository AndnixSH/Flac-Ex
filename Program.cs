using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace flac_ex
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream file = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            // create array
            byte[] input = new byte[file.Length];
            // fill array
            file.Read(input, 0, input.Length);

            int size = 0, offset = 0 , header_size = 0;
            //data size array
            byte[] fsiz = new byte[4];
            //pattern to get offset of start data
            byte [] dstart = {0xff, 0xf8};

            for (int i = 0; i < input.Length; i++) 
            {
                //search for - fLaC 66 4c 61 43
                if (input[i] == 0x66 && input[i + 1] == 0x4c && input[i + 2] == 0x61 && input[i + 3] == 0x43) 
                {
                    //save offset
                    offset = i;
                    //fill array file size
                    fsiz[0] = input[i + 97];
                    fsiz[1] = input[i + 96];
                    fsiz[2] = input[i + 95];
                    fsiz[3] = input[i + 94];
                    //get data size
                    size = HexaByte(fsiz, 0);

                    //get header size
                    header_size = FindIndex(input, dstart, i + 98);
                    header_size -= i;
                    //total size = data size + header size
                    size += header_size;

                    Stream output = File.Create(Path.GetFullPath(args[0]) + "_" + offset.ToString("X8"));
                    output.Write(input, offset, size);
                    output.Close();

                    i = offset + size - 1;

                    Console.Out.WriteLine("Offset : " + offset + "\nSize : " + size);
                }
            }


        }

        public static int FindIndex(byte[] input, byte[] pattern, int start)
        {
            int index = 0;

            for (int i = start; i < input.Length; i++)
            {
                if (input[i] == pattern[0] && input[i + 1] == pattern[1])
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static int HexaByte(byte[] input, int index)
        {
            int data = 0x00000000;
            int y = 24;

            for (int i = index; i < index + 4; i++)
            {
                data |= (input[i] << y);
                y -= 8;
            }

            return data;
        }
    }
}
