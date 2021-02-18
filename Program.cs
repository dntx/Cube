using System;
using System.Collections.Generic;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveShape);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveL3P75Quarters);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveL3Quarters);
            // ... SolveL1Quarters ...
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveQuarterPosition);

            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveShape);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveL1orL3P75);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveL1orL3LastP25);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveL3Cross);
            new Sq1Solver().Solve(Sq1Solver.Goal.SolveL3Corners);

            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveShape);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveUpDownColor);
            //...
        }
    }
}
