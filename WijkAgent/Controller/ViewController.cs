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
            view.OnCleanDistrictTweetsButtonClick += CleanDistrictTweetsButtonController_clicked;

            ThreadDelegate = new ThreadActionRefresh(view.RefreshThreatAction);
        }
        #endregion

        #region ThreadFunction
        private void ThreadFunction()
        {
            RefreshThread RefreshThread = new RefreshThread(this);
            int sleepfor = view.modelClass.databaseConnectie.GetRefreshButtonHide(view.modelClass.idDistrict) * 1000;
            RefreshThread.Run(sleepfor);
        }
        #endregion

        #region RefreshButtonController_Clicked
        public void RefreshButton_Clicked()
        {
            //Reset selected tag.
            view.ResetClickEventTwitterTag();

            // refreshed alles in de laatst geselecteerd wijk om nieuwe tweets weer te geven.
            view.modelClass.map.changeDistrict(view.modelClass.map.currentLatitudePoints, view.modelClass.map.currentLongitudePoints);

            // update nieuwe tweets label            
            view.UpdateTwitterpanel();

            myThread = new Thread(new ThreadStart(ThreadFunction));
            myThread.Start();
        }
        #endregion

        #region CleanDistrictTweetsButton_Clicked
        public void CleanDistrictTweetsButtonController_clicked()
        {
            view.modelClass.databaseConnectie.DeleteUnSavedTweetsForDistrict(view.modelClass.map.idDistrict);
        }
        #endregion
    }
}