using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;

namespace LeboncoinParcer
{
    public static class Extensions
    {
        public static CustomWebProxy Cut(this ObservableCollection<CustomWebProxy> items, CustomWebProxy item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                return item;
            }
            return null;
        }
        public static CustomWebProxy Cut(this ObservableCollection<CustomWebProxy> items)
        {
            if (items.Count < 1)
                return null;
            return items.Cut(items[0]);
        }

    }
}
