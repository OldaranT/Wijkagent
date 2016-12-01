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
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == false)
            {
                MessageBox.Show("Geen internet verbinding");
            }
           else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                LogInScreen logInScreen = new LogInScreen();
                LogInScreenController LogInScreenControllercs = new LogInScreenController(logInScreen);
                Application.Run(logInScreen);
            }
        }
    }
}
