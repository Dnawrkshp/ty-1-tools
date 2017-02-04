using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TyModManager.UI.Font
{
    public static class FontExtensions
    {

        public static void AddFont(this PrivateFontCollection fontCollection, byte[] bytes)
        {
            // allocate memory and copy byte[] to the location
            IntPtr data = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, data, bytes.Length);

            // pass the font to the font collection
            fontCollection.AddMemoryFont(data, bytes.Length);

            // Free the unsafe memory
            Marshal.FreeCoTaskMem(data);
        }

        public static FontFamily GetFontFamily(this PrivateFontCollection fontCollection, string name)
        {
            return fontCollection.Families.FirstOrDefault(x => x.Name == name);
        }

    }
}
