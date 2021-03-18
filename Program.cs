using System;

namespace Cube
{
    class Program
    {
        static void Main(string[] args)
        {
            bool successful = true;
            //successful &= Sq1List2Cube.Solver.Solve(ASolver.Mode.ReverseBfSearch);
            successful &= Sq1List16Cube.Solver.SolveEasy(ASolver.Mode.BidiASearch);
            //successful &= Sq1List16Cube.Solver.SolveHard(ASolver.Mode.BidiASearch);
            //successful &= Sq1BitCube.Solver.Solve();
            Console.WriteLine("overall successful: {0}", successful);
            Console.WriteLine();
        }

    }
}
