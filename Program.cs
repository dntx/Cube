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

            //successful &= Sq1List16Cube.Solver.SolveEasy(ASolver.Mode.BidiBfSearch);  // slow, solution must be optimal
            //successful &= Sq1List16Cube.Solver.SolveEasy(ASolver.Mode.BidiASearch);   // fast, solution may not be optimal

            //successful &= Sq1List16Cube.Solver.SolveHard(ASolver.Mode.BidiASearch);   // can't find the solution in 5 minutes

            //successful &= Sq1BitCube.Solver.SolveEasy(ASolver.Mode.BidiBfSearch);     // solution must be optimal
            successful &= Sq1BitCube.Solver.SolveEasy(ASolver.Mode.BidiASearch);      // solution may not be optimal
            //successful &= Sq1BitCube.Solver.SolveEasy(ASolver.Mode.ASearch);          // solution may not be optimal
            //successful &= Sq1BitCube.Solver.SolveEasy(ASolver.Mode.PermuteASearch);   // solution may not be optimal

            //successful &= Sq1BitCube.Solver.SolveHard(ASolver.Mode.BidiBfSearch);     // solution must be optimal
            //successful &= Sq1BitCube.Solver.SolveHard(ASolver.Mode.BidiASearch);      // solution may not be optimal
            //successful &= Sq1BitCube.Solver.SolveHard(ASolver.Mode.PermuteASearch);   // solution may not be optimal
            
            Console.WriteLine("overall successful: {0}", successful);
            Console.WriteLine();
        }

    }
}
