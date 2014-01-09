using System;

namespace Light2CubeShader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CubeLightShader1 game = new CubeLightShader1())
            {
                game.Run();
            }
        }
    }
}

