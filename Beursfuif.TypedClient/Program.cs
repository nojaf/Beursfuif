using System;
using System.Diagnostics;
using Microsoft.Owin.Hosting;

namespace Beursfuif.TypedClient
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Press enter to exit");
            using (WebApp.Start<Startup>("http://localhost:8300"))
            {
                Process.Start("http://localhost:8300/");
                Console.WriteLine("Running server on port 8300");
                Console.ReadLine();
            }
        }
    }
}