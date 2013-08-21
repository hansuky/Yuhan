using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.DragDrop.Demo
{
    public class ExampleImage
    {
        public string ImageName {get; set;}
        public string ImageSource { get; set; }

        public ExampleImage() { }

        public ExampleImage(string name, string source)
        {
            ImageName = name;
            ImageSource = source;
        }
    }
}
