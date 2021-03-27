using System;

namespace Cube
{
    class Program
    {
        static void Main(string[] args)
        {
            bool successful = true;
            //successful &= Sq1List2Cube.Solver.Solve(ASolver.Mode.BackwardBfSearch);   // one round
            //successful &= Sq1List2Cube.Solver.Solve(ASolver.Mode.BfSearch);           // multiple round

            //successful &= Sq1List16Cube.Solver.SolveEasy(ASolver.Mode.BidiBfSearch);
            //successful &= Sq1List16Cube.Solver.SolveEasy(ASolver.Mode.BidiASearch);   // solutions may not optimal
            //successful &= Sq1List16Cube.Solver.SolveHard(ASolver.Mode.BidiASearch);
            successful &= Sq1BitCube.Solver.Solve(ASolver.Mode.BidiBfSearch);
            //successful &= Sq1BitCube.Solver.Solve(ASolver.Mode.PermuteBfSearch);
            
            Console.WriteLine("overall successful: {0}", successful);
            Console.WriteLine();
        }

    }
}
