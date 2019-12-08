using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using nVault.NET.Model;
using Syncfusion.Data;

namespace nVault.NET.Helper
{
    public class KeyComparer : IComparer<object>, ISortDirection
    {
        public ListSortDirection SortDirection { get; set; }


        public int Compare(object x, object y)
        {
            string entryX = ((EntryModel)x)?.EntryKey;
            string entryY = ((EntryModel)y)?.EntryKey;
            return Utils.CompareResult(Utils.CompareNatural(entryX, entryY), SortDirection);

        }
    }

    public class ValueComparer : IComparer<object>, ISortDirection
    {
        public ListSortDirection SortDirection { get; set; }

        public int Compare(object x, object y)
        {
            string entryX = ((EntryModel)x)?.EntryValue;
            string entryY = ((EntryModel)y)?.EntryValue;
            return Utils.CompareResult(Utils.CompareNatural(entryX, entryY), SortDirection);
        }
    }
}