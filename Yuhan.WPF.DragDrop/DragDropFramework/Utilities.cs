using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;



namespace Yuhan.WPF.DragDrop.DragDropFramework
{
    public class Utilities
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pos);

        /// <summary>
        /// Returns the position of the mouse cursor in screen coordinates
        /// </summary>
        /// <returns></returns>
        public static Point Win32GetCursorPos() {
            Win32Point position = new Win32Point();
            GetCursorPos(ref position);
            return new Point((double)position.X, (double)position.Y);
        }

        /// <summary>
        /// Loops to find parent control of type <code>T</code>
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="depObj">Object where search begins</param>
        /// <returns>Object of type <code>T</code> or null</returns>
        public static T FindParentControlIncludingMe<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            while(depObj != null) {
                if(depObj is T)
                    return depObj as T;
                if(depObj is Visual)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else if(depObj is FrameworkContentElement) {
                    depObj = ((FrameworkContentElement)depObj).Parent;
                }
                else
                    depObj = null;
            }

            return null;
        }

        /// <summary>
        /// Loops to find parent control of type <code>T</code>
        /// </summary>
        /// <typeparam name="T">Type of object to return</typeparam>
        /// <param name="depObj">Child of object where search begins</param>
        /// <returns>Object of type <code>T</code> or null</returns>
        public static T FindParentControlExcludingMe<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            while(depObj != null) {
                if(depObj is Visual)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else if(depObj is FrameworkContentElement) {
                    depObj = ((FrameworkContentElement)depObj).Parent;
                }
                else
                    depObj = null;
                if(depObj is T)
                    return depObj as T;
            }

            return null;
        }

        /// <summary>
        /// Serializes <code>obj</code> into a XAML string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Serialized XAML version of <code>obj</code></returns>
        public static string SerializeObject(object obj) {
            return XamlWriter.Save(obj);
        }

        /// <summary>
        /// Converts XAML serialized string back into object
        /// </summary>
        /// <param name="xaml">XAML serialized object</param>
        /// <returns>Object represented by XAML string</returns>
        public static object DeserializeObject(string xaml) {
            return XamlReader.Load(new XmlTextReader(new StringReader(xaml)));
        }

        /// <summary>
        /// Clones an object
        /// </summary>
        /// <param name="obj">object to clone</param>
        /// <returns>Clone of <code>obj</code></returns>
        public static object CloneElement(object obj) {
            return DeserializeObject(SerializeObject(obj));
        }
    }
}
