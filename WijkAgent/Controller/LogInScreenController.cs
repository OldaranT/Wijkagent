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

        #region LogIn_Button_Passed_Click
        public void LogInButtonClicked(string _username)
        {      
            if (logInScreen.Visible)
            {
                logInScreen.Hide();
                viewController = new ViewController(_username);
                viewController.view.OnLogOutButtonClick += LogOutButtonClicked;
                viewController.view.Show();
            }
        }
        #endregion

        #region LogOut_Button_Clicked
        public void LogOutButtonClicked()
        {
            viewController.view.Hide();
            logInScreen.Show();
            viewController = null;
        }
        #endregion

    }
}
