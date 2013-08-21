using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections;

namespace Yuhan.WPF.DsxGridCtrl.Demo
{
    public static class ContactTitles
    {
        static ContactTitles()
        {
            List<string> _list = new List<string>()
            {
                "Owner",
                "Sales Manager",
                "Sales Representative",
                "Sales Agent",
                "Sales Associate",
                "Assistant Sales Agent",
                "Marketing Manager",
                "Marketing Assistant",
                "Order Administrator",
                "Accounting Manager",
            };


            ContactTitles.ComboSource = _list;
        }
        public static IEnumerable ComboSource { get; set; }
    }
}
