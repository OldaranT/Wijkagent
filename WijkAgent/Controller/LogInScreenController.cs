using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Controller
{
    class LogInScreenController
    {
        public LogInScreen logInScreen;
        public ViewController viewController;

        public LogInScreenController(LogInScreen _logInScreen)
        {
            logInScreen = _logInScreen;
            logInScreen.OnLogInButtonClick += LogInButtonClicked;
        }

        public void LogInButtonClicked()
        {
            logInScreen.Hide();
            viewController = new ViewController();
            viewController.view.Show();
        }

    }
}
