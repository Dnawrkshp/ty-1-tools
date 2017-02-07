using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TyModManager.Extension;

namespace TyModManager.UI.Font
{
    /// <summary>
    /// http://font.ubuntu.com/
    /// </summary>
    public static class Ubuntu
    {
        private static PrivateFontCollection _privateFontCollection = new PrivateFontCollection();
        private static bool _setup = false;

        #region Fonts

        private static FontFamily _ubuntuMono = null;

        public static FontFamily UbuntuMono
        {
            get
            {
                if (!_setup)
                    Setup();
                return _ubuntuMono;
            }
        }

        #endregion

        private static void Setup()
        {
            _privateFontCollection.AddFont(Properties.Resources.UbuntuMono_R);

            _ubuntuMono = _privateFontCollection.GetFontFamily("Ubuntu Mono");

            _setup = true;
        }
    }
}
