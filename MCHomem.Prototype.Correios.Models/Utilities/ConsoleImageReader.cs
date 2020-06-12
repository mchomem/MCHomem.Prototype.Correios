using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace MCHomem.Prototype.Correios.Models.Utilities
{
    public static class ConsoleImageReader
    {
        #region External DLL's

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            int dwShareMode,
            IntPtr lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            [Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleFontInfo lpConsoleCurrentFont);

        #endregion

        #region Fields

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = 0x40000000;
        private const int FILE_SHARE_READ = 1;
        private const int FILE_SHARE_WRITE = 2;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int OPEN_EXISTING = 3;

        #endregion

        #region Internal Class

        [StructLayout(LayoutKind.Sequential)]
        internal class ConsoleFontInfo
        {
            internal int nFont;
            internal Coord dwFontSize;
        }

        #endregion

        #region Struct

        [StructLayout(LayoutKind.Explicit)]
        internal struct Coord
        {
            [FieldOffset(0)]
            internal short X;
            [FieldOffset(2)]
            internal short Y;
        }

        #endregion

        #region Methods

        public static void DrawImage(String path)
        {
            Point location = new Point(1, 1);
            // TODO ajust aspect ratio for image.
            // TODO send imagen by parameter in this method to get your properites (height & width).
            // TODO transform imagem dimension to character size.

            Size imageSize = new Size(50, 7); // desired image size in characters

#if DEBUG
            // draw some placeholders to show the bounds of image.
            Console.SetCursorPosition(location.X - 1, location.Y);
            Console.Write(">");
            Console.SetCursorPosition(location.X + imageSize.Width, location.Y);
            Console.Write("<");
            Console.SetCursorPosition(location.X - 1, location.Y + imageSize.Height - 1);
            Console.Write(">");
            Console.SetCursorPosition(location.X + imageSize.Width, location.Y + imageSize.Height - 1);
            Console.WriteLine("<");
#endif

            using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
            {
                using (Image image = Image.FromFile(path))
                {
                    Size fontSize = GetConsoleFontSize();

                    // translating the character positions to pixels
                    Rectangle imageRect =
                        new Rectangle
                        (
                            location.X * fontSize.Width
                            , location.Y * fontSize.Height
                            , imageSize.Width * fontSize.Width
                            , imageSize.Height * fontSize.Height
                        );

                    g.DrawImage(image, imageRect);
                }
            }
        }

        private static Size GetConsoleFontSize()
        {
            // getting the console out buffer handle
            IntPtr outHandle =
                CreateFile
                (
                    "CONOUT$"
                    , GENERIC_READ | GENERIC_WRITE
                    , FILE_SHARE_READ | FILE_SHARE_WRITE
                    , IntPtr.Zero
                    , OPEN_EXISTING
                    , 0
                    , IntPtr.Zero
                );

            int errorCode = Marshal.GetLastWin32Error();

            if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", errorCode);
            }

            ConsoleFontInfo cfi = new ConsoleFontInfo();

            if (!GetCurrentConsoleFont(outHandle, false, cfi))
            {
                throw new InvalidOperationException("Unable to get font information.");
            }

            return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
        }

        #endregion
    }
}
