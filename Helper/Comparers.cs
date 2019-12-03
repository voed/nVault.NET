using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
            return Utils.CompareResult(StringComparer.OrdinalIgnoreCase.WithNaturalSort().Compare(entryX, entryY), SortDirection);

        }
    }

    public class ValueComparer : IComparer<object>, ISortDirection
    {
        public int Compare(object x, object y)
        {
            string entryX = ((EntryModel)x)?.EntryValue;
            string entryY = ((EntryModel)y)?.EntryValue;
            return Utils.CompareResult(StringComparer.OrdinalIgnoreCase.WithNaturalSort().Compare(entryX, entryY), SortDirection);
        }

        public ListSortDirection SortDirection { get; set; }
    }
}