using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        public const int TICKS_PER_SECOND = 30;
        public const int TICKRATE = 1000 / TICKS_PER_SECOND;

        private static bool isRunning = false;

        private static void MainThread()
        {
            //update loop
            while (isRunning)
            {
                ThreadManager.UpdateMain();
                Thread.Sleep(TICKRATE);
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Game Server";
            Server.Start(420, 100);
            isRunning = true;

            //start thread
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
        }
    }
}
