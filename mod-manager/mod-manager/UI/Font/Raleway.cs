using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TyModManager.UI.Font
{
    /// <summary>
    /// https://fonts.google.com/specimen/Raleway
    /// </summary>
    public static class Raleway
    {
        private static PrivateFontCollection _privateFontCollection = new PrivateFontCollection();
        private static bool _setup = false;

        #region Fonts

        private static FontFamily _ralewayRegular = null;
        private static FontFamily _ralewayLight = null;

        public static FontFamily RalewayRegular
        {
            get
            {
                if (!_setup)
                    Setup();
                return _ralewayRegular;
            }
        }

        public static FontFamily RalewayLight
        {
            get
            {
                if (!_setup)
                    Setup();
                return _ralewayLight;
            }
        }

        #endregion

        private static void Setup()
        {
            _privateFontCollection.AddFont(Properties.Resources.Raleway_Regular);
            _privateFontCollection.AddFont(Properties.Resources.Raleway_Light);
            
            _ralewayRegular = _privateFontCollection.GetFontFamily("Raleway");
            _ralewayLight = _privateFontCollection.GetFontFamily("Raleway Light");

            _setup = true;
        }
    }
}
