using System;

namespace Simulation
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Simulation game = new Simulation())
            {
                game.Run();
            }
        }
    }
#endif
}

