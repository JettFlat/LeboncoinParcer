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
        public static WebProxy Cut(this ObservableCollection<WebProxy> items, WebProxy item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                return item;
            }
            return null;
        }
        public static WebProxy Cut(this ObservableCollection<WebProxy> items)
        {
            if (items.Count < 1)
                return null;
            return items.Cut(items[0]);
        }

    }
}
