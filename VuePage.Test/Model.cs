using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VuePage.Test
{
    public class VM
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public decimal Money { get; set; }
        public bool IsEmpty { get; set; }
        public List<Todo> Items { get; set; }
    }

    public class Todo
    {
        public string Desc { get; set; }
        public bool Done { get; set; }
    }
}
