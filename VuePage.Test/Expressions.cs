using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vue;

namespace VuePage.Test
{
    [TestClass]
    public class Expressions
    {
        [TestMethod]
        public void Should_Test_Operators()
        {
            Test(x => x.Money == 0, "x.Money == 0;");
            Test(x => x.Money > 0, "x.Money > 0;");
            Test(x => x.Money < 0, "x.Money < 0;");
            Test(x => x.Money != 0, "x.Money != 0;");
            Test(x => x.IsEmpty, "x.IsEmpty;");
            Test(x => x.IsEmpty == false, "x.IsEmpty == false;");
        }

        [TestMethod]
        public void Should_List_Filter()
        {
            Test(x => x.Items.Where(z => z.Done), "x.Items.filter(function(z) { return z.Done; });");
            Test(x => x.Items.Where(z => z.Done).Count() > 3, "x.Items.filter(function(z) { return z.Done; }).length > 3;");
        }

        [TestMethod]
        public void Should_List_Map()
        {
            Test(x => x.Items.Select(z => z.Done), "x.Items.map(function(z) { return z.Done; });");
            Test(x => x.Items.Select(z => new { z.Done }), "x.Items.map(function(z) { return { 'Done': z.Done }; });");
            Test(x => x.Items.Select(z => new { A = z.Done, B = z.Desc }), "x.Items.map(function(z) { return { 'A': z.Done, 'B': z.Desc }; });");
            Test(x => x.Items.Select(z => new { A = z.Desc.Length == 0 }), "x.Items.map(function(z) { return { 'A': z.Desc.length == 0 }; });");
        }

        [TestMethod]
        public void Should_Test_Not()
        {
            Test(x => !x.IsEmpty, "!(x.IsEmpty);");
            Test(x => !(x.IsEmpty || x.Money == 0), "!(x.IsEmpty || x.Money == 0);");
        }

        [TestMethod]
        public void Should_Array_Index()
        {
            Test(x => x.Text.Split(';'), "x.Text.split(';');");
            Test(x => x.Text.Split(';')[0], "x.Text.split(';')[0];");
            Test(x => x.Items[0].Done, "x.Items[0].Done;");
            Test(x => x.Items.ToArray()[0].Done, "x.Items[0].Done;");
        }

        [TestMethod]
        public void Should_String_Methods()
        {
            Test(x => x.Text.Length, "x.Text.length;");
            Test(x => x.Text.ToUpper(), "x.Text.toUpperCase();");
            Test(x => x.Text.ToLower(), "x.Text.toLowerCase();");
            Test(x => x.Text.StartsWith("A"), "x.Text.startsWith('A');");
            Test(x => x.Text.EndsWith("A"), "x.Text.endsWith('A');");
            Test(x => x.Text.Contains("A"), "x.Text.indexOf('A') >= 0;");

            // concat
            Test(x => x.Text + " - " + x.Text.Length, "x.Text + ' - ' + x.Text.length;");
        }

        [TestMethod]
        public void Should_Conditional()
        {
            Test(x => x.Text.Length == 0 ? "zero" : "one", "x.Text.length == 0 ? 'zero' : 'one';");
            Test(x => x.Text.Length == 0 ? "zero" : x.Text.Length == 1 ? "one" : "other", 
                "x.Text.length == 0 ? 'zero' : x.Text.length == 1 ? 'one' : 'other';");
        }

        [TestMethod]
        public void Should_DateTime_Methods()
        {
            Test(x => DateTime.Now, "new Date();");
            //Test(x => new DateTime(2012, 1, 1), "new Date(2012, 0, 1);");
        }

        public static void Test(Expression<Func<VM, object>> expr, string expect)
        {
            var js = JsExpressionVisitor.Resolve(expr);

            Assert.AreEqual("function(x) { return " + expect + " }", js);
        }
    }
}
