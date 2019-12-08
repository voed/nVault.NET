using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = System.Drawing.Size;

namespace nVault.NET.Helper
{
    public static class Utils
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        public static readonly List<string> SearchPhrases = new List<string>
        {
            "All",
            "Key",
            "Value",
            "Timestamp",
            "Date"
        };
        public enum SearchType
        {
            SearchAll,
            SearchKey,
            SearchValue,
            SearchTimestamp,
            SearchDate
        }

        public static readonly DateTime MinDate = UnixToDateTime(0);//01.01.1970
        public static readonly DateTime MaxDate = UnixToDateTime(int.MaxValue - 1);//2038 problem
        public static readonly char[] MagickChars = "nVLT".Reverse().ToArray(); //header of nvault file

        public static readonly FileVersionInfo CurrentAssembly = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        public static readonly string ProgramName = CurrentAssembly.ProductName;
        public static readonly string ProgramVersion = CurrentAssembly.ProductVersion;
        public static readonly string ProgramAuthor = CurrentAssembly.CompanyName;
        public static readonly string ProgramCopyright = CurrentAssembly.LegalCopyright;
        public static readonly ImageSource ProgramImage = Imaging.CreateBitmapSourceFromHBitmap(
            ResizeImage(Properties.Resources.icon, 192,192)
                .GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        public static readonly string ProgramDescription = 
            Assembly.GetExecutingAssembly()
            .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
            .OfType<AssemblyDescriptionAttribute>()
            .FirstOrDefault()?.Description;

        public static readonly string ProgramUri = Properties.Resources.ProgramUri;

        public static int CompareNatural(string str1, string str2) => StrCmpLogicalW(str1, str2);

        public static DateTime UnixToDateTime(int timestamp) => DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

        public static int DateTimeToUnix(DateTime time)
        {
            try
            {
                return Convert.ToInt32(new DateTimeOffset(new DateTimeOffset(time).UtcDateTime).ToUnixTimeSeconds());
            }
            catch (FormatException)
            {
                return 0;
            }
            
        }


        public static Bitmap ResizeImage(Bitmap imgToResize, int width, int height)
        {
            try
            {
                Bitmap b = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                    g.DrawImage(imgToResize, 0, 0, width, height);
                }
                return b;
            }
            catch 
            {
                return imgToResize; 
            }
        }

        public static int CompareResult(int compare, ListSortDirection sortDirection)
        {
            if (compare > 0)
                return sortDirection == ListSortDirection.Ascending ? 1 : -1;
            if (compare == -1)
                return sortDirection == ListSortDirection.Ascending ? -1 : 1;

            return 0;
        }
        public static int Clamp(this int val, int min, int max)
        {
            if (val.CompareTo(min) < 0) return min;
            if(val.CompareTo(max) > 0) return max;
            return val;
        }
    }
}