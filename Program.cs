using System;

namespace Cube
{
    class Program
    {
        static void Main(string[] args)
        {
            bool successful = true;
            successful &= Sq1RawCube.Solver.Solve();
            successful &= Sq1Cube.Solver.Solve();
            Console.WriteLine("overall successful: {0}", successful);
            Console.WriteLine();
        }

    }
}
