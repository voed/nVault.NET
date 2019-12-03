﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = System.Drawing.Size;

namespace nVault.NET
{
    public static class Utils
    {
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

        public static DateTime UnixToDateTime(int timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }

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

    }

}