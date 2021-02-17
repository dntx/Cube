using System;
using System.Collections.Generic;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveUpDownShape);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveL3P75Quaters);
            new Sq1Solver().Solve(Sq1Solver.Goal.SolveL3Quaters);
            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveQuaterPosition);

            //new Sq1Solver().Solve(Sq1Solver.Goal.SolveUpDownColor);
        }
    }
}
