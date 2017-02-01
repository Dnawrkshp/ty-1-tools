using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ty_mod_manager
{
    public class TyRKV
    {
        private string _path = null;

        public struct FileEntry
        {
            public string FileName;
            public string FullPath;
            public long FileSize;
            public long FileOffset;
        }

        public FileEntry[] FileEntries { get; set; } = null;
        public string[] Directories { get; set; } = null;

        public TyRKV(string rkvPath)
        {
            _path = rkvPath;

            // Populate file entries
            FileStream fs = File.OpenRead(rkvPath);

            Directories = new string[FileStreamReader.ParseUInt(fs, fs.Length - 4, true)];
            FileEntries = new FileEntry[FileStreamReader.ParseUInt(fs, fs.Length - 8, true)];

            for (int i = 0; i < Directories.Length; i++)
                Directories[i] = FileStreamReader.ParseString(fs, fs.Length - 8 - (0x100 * (i+1)));
            
            for (int i = 0; i < FileEntries.Length; i++)
            {
                long offset = fs.Length - 8 - (0x100 * Directories.Length) - (0x40 * (i + 1));
                FileEntries[i] = new FileEntry() {
                    FileName = FileStreamReader.ParseString(fs, offset),
                    FileSize = FileStreamReader.ParseUInt(fs, offset + 0x24, true),
                    FileOffset = FileStreamReader.ParseUInt(fs, offset + 0x2C, true)
                };
                FileEntries[i].FullPath = Path.Combine(Directories[Directories.Length - 1 - FileStreamReader.ParseUInt(fs, offset + 0x20, true)], FileEntries[i].FileName);
            }

            fs.Close();
        }

        public string ExtractText(FileEntry file)
        {
            try
            {
                using (StreamReader sr = new StreamReader(_path, Encoding.GetEncoding("iso-8859-1")))
                {
                    char[] buffer = new char[file.FileSize];

                    if (sr.BaseStream.Position != file.FileOffset)
                        sr.BaseStream.Seek(file.FileOffset - sr.BaseStream.Position, SeekOrigin.Current);
                    sr.Read(buffer, 0, buffer.Length);

                    return new string(buffer);
                }
            }
            catch (Exception e)
            {
                Program.Log("Failed to open RKV (ExtractText)", e);
                return null;
            }
        }
    }

    public class FileStreamReader
    {

        public static float ParseFloat(byte[] ba, long offset, bool flip = false)
        {
            byte[] bytes = new byte[4];
            Array.Copy(ba, (int)offset, bytes, 0, 4);

            if ((flip ? !BitConverter.IsLittleEndian : BitConverter.IsLittleEndian))
                Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static uint ParseUInt(byte[] ba, long offset, bool flip = false)
        {
            byte[] bytes = new byte[4];
            Array.Copy(ba, (int)offset, bytes, 0, 4);

            if ((flip ? !BitConverter.IsLittleEndian : BitConverter.IsLittleEndian))
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static uint ParseUInt(FileStream fs, long offset, bool flip = false)
        {
            if (fs.Position != offset)
                fs.Seek(offset - fs.Position, SeekOrigin.Current);

            byte[] bytes = new byte[4];
            fs.Read(bytes, 0, 4);

            return (uint)(bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24);
        }

        public static string ParseString(FileStream fs, long offset)
        {
            string ret = "";

            loop: if (fs.Position != offset)
                fs.Seek(offset - fs.Position, SeekOrigin.Current);

            byte[] bytes = new byte[2048];
            fs.Read(bytes, 0, 2048);

            int x = 0;
            for (x = 0; x < bytes.Length; x++)
            {
                if (bytes[x] == 0)
                    break;
                ret += ((char)bytes[x]).ToString();
            }
            
            // More string to parse
            if (x == bytes.Length)
            {
                offset += bytes.Length;
                goto loop;
            }

            return ret;
        }

        public static float ParseFloat(FileStream fs, long offset, bool flip = false)
        {
            if (fs.Position != offset)
                fs.Seek(offset - fs.Position, SeekOrigin.Current);

            byte[] bytes = new byte[4];
            fs.Read(bytes, 0, 4);
            if ((flip ? !BitConverter.IsLittleEndian : BitConverter.IsLittleEndian))
                Array.Reverse(bytes);

            return BitConverter.ToSingle(bytes, 0);
        }


    }
}
