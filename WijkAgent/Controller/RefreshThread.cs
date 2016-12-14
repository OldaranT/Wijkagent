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

        public void Run(int _sleepfor)
        {
            // variabele voor het aantal miliseconden dat de thread wacht
            int sleepfor = _sleepfor;
            
            // de thread wacht hier voor het aantal miliseconden dat is opgegeven
            Thread.Sleep(sleepfor);

            // unhide de refresh knop door een delegate te invoken
            // invoke betekend dat je van af een thread een delegate
            // aanroept van een andere thread zonder een exception te veroorzaken
            controller.view.Invoke(controller.ThreadDelegate);
        }
    }
}
