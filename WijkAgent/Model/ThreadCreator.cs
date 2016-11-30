using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WijkAgent.Model
{
    public delegate void Refreshed();
    class ThreadCreator
    {
        public bool refreshed = false;
        public void CallToChildThread()
        {
            while (true)
            {
                if (refreshed)
                {
                    Console.WriteLine("Child thread starts");

                    // the thread is paused for x amount of miliseconds
                    int sleepfor = 60000;

                    Console.WriteLine("Child Thread Paused for {0} seconds", sleepfor / 1000);
                    Thread.Sleep(sleepfor);
                    Console.WriteLine("Child thread resumes");
                    refreshed = false;
                }
                
            }
        }

        public void CreateChildThread()
        {
            ThreadStart childref = new ThreadStart(CallToChildThread);
            Console.WriteLine("In Main: Creating the Child Thread");
            Thread childThreat = new Thread(childref);
            childThreat.Start();
            //Console.ReadKey();
        }
    }
}
