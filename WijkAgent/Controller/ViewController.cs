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

        public ViewController()
        {
            view = new View();
            view.OnRefreshButtonClick += RefreshButton_Clicked;

            ThreadDelegate = new ThreadActionRefresh(view.RefreshThreatAction);
        }

        #region Thread Function
        private void ThreadFunction()
        {
            RefreshThread RefreshThread = new RefreshThread(this);
            RefreshThread.Run();
        }
        #endregion

        #region RefreshButtonController_Clicked
        public void RefreshButton_Clicked()
        {
            //refreshed alles in de laatst geselecteerd wijk om nieuwe tweets weer te geven.
            view.modelClass.map.changeDistrict(view.modelClass.map.currentLatitudePoints, view.modelClass.map.currentLongitudePoints);
            view.modelClass.TweetsToDb();
            myThread = new Thread(new ThreadStart(ThreadFunction));
            myThread.Start();
        }
        #endregion
    }

    #region ThreadClass
    
    #endregion
}