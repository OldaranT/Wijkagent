using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WijkAgent.Controller
{
    class RefreshThread
    {
        //View view;
        ViewController controller;

        public RefreshThread(ViewController _controller)
        {
            controller = _controller;
        }

        public void Run()
        {
            int sleepfor = 5000;
            
            Thread.Sleep(sleepfor);

            controller.view.Invoke(controller.ThreadDelegate);
        }
    }
}
