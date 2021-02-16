using System;
using System.Collections.Generic;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveUpDownShape);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveUpDownColor);
            //new Sq1Solver().Solve(Sq1Solver.Goal.Solve6030Pair);
            new Sq1Solver().Solve(Sq1Solver.Goal.Solve6030Position);
        }
    }
}
