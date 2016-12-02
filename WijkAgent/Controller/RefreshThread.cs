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
            Console.WriteLine("threat started running like forest");
            int sleepfor = 5000;

            Console.WriteLine("Child Thread Paused for {0} seconds", sleepfor / 1000);
            Thread.Sleep(sleepfor);

            controller.view.Invoke(controller.ThreadDelegate);
            Console.WriteLine("view is invoked");
        }
    }
}
