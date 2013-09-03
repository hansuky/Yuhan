using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.Login.Demo
{
    public class Program
    {
        [STAThread]
        public static void Main(String[] args)
        {
            LoginWindow window = new LoginWindow();
            window.ShowDialog();
        }
    }
}
