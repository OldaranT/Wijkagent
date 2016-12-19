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

        #region Constructor
        public LogInScreenController(LogInScreen _logInScreen)
        {
            logInScreen = _logInScreen;
            logInScreen.OnLogInButtonClick += LogInButtonClicked;
        }
        #endregion

        #region LogInButtonClicked
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

        #region LogOutButtonClicked
        public void LogOutButtonClicked()
        {
            viewController.view.modelClass.databaseConnectie.ChangeAccountLocation(viewController.view.modelClass.username, null, null);
            viewController.view.modelClass.map.watcher.Stop();
            viewController.view.modelClass.map.mapThread.Abort();
            viewController.view.Dispose();
            logInScreen.Show();       
        }
        #endregion

    }
}
