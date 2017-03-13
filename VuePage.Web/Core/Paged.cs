using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace VuePage.Web
{
    public class Paged<T>
    {
        public int Index { get; set; } = 0;
        public int Count { get; private set; } = 0;
        public int PageCount { get; private set; } = 0;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; }
        public bool Ascending { get; set; } = true;
        public List<T> Items { get; set; }

        public void Load(IEnumerable<T> items)
        {
            Count = items.Count();
            PageCount = ((Count - 1) / PageSize) + 1;
            Index = Index < 0 ? 0 :
                Index > (PageCount - 1) ? PageCount - 1 :
                Index;

            Items = items
                .Skip(Index * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}