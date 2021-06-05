//main program to execute on server side
//starts the server with parameters
//starts a thread and updates it in a loop


using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        const int TICKS_PER_SECOND = 30;
        const int TICKRATE = 1000 / TICKS_PER_SECOND;
        const int PORT = 420;
        const int MAX_CLIENTS = 100;

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
            Server.Start(PORT, MAX_CLIENTS);
            isRunning = true;

            //start thread
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
        }
    }
}
