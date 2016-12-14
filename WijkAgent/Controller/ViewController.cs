using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WijkAgent.Controller
{
    public delegate void ThreadActionRefresh();

    class ViewController
    {
        public View view;
        public ThreadActionRefresh ThreadDelegate;
        private Thread myThread;

        #region Constructor
        public ViewController(string _username)
        {
            view = new View(_username);
            view.OnRefreshButtonClick += RefreshButton_Clicked;

            ThreadDelegate = new ThreadActionRefresh(view.RefreshThreatAction);
        }
        #endregion

        #region ThreadFunction
        private void ThreadFunction()
        {
            RefreshThread RefreshThread = new RefreshThread(this);
            RefreshThread.Run();
        }
        #endregion

        #region RefreshButtonController_Clicked
        public void RefreshButton_Clicked()
        {
            // refreshed alles in de laatst geselecteerd wijk om nieuwe tweets weer te geven.
            view.modelClass.map.changeDistrict(view.modelClass.map.currentLatitudePoints, view.modelClass.map.currentLongitudePoints);
            view.modelClass.TweetsToDb();

            // update nieuwe tweets label            
            view.UpdateTwitterpanel();
            view.UpdateNewTweetsLabel();


            myThread = new Thread(new ThreadStart(ThreadFunction));
            myThread.Start();
        }
        #endregion
    }
}