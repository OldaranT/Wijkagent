using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WijkAgent.Controller;
using WijkAgent.Model;

namespace WijkAgent
{
    static class Program
    { 
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            View view = new View();
            ViewController viewController = new ViewController(view);
            Application.Run(view);
        }
    }
}
