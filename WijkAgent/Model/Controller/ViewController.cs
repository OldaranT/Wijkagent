using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Controller
{
    class ViewController
    {
        private View view;

        public ViewController(View _view)
        {
            view = _view;
            view.OnRefreshButtonClick += RefreshButton_Clicked;
        }

        #region RefreshButtonController_Clicked
        public void RefreshButton_Clicked()
        {
            //refreshed alles in de laatst geselecteerd wijk om nieuwe tweets weer te geven.
            view.modelClass.map.changeDistrict(view.modelClass.map.currentLatitudePoints, view.modelClass.map.currentLongitudePoints);
        }
        #endregion
    }
}