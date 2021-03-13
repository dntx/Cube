using System;

namespace Cube.Sq1RawCube
{
    class Solver
    {
        public static void Solve()
        {
            ASolver.Mode mode = ASolver.Mode.ReverseSearch;
            //ASolver.Mode mode = ASolver.Mode.ASearch;
            //ASolver.Mode mode = ASolver.Mode.BiDiSearch;
            
            DateTime startTime = DateTime.Now;
            bool successful = new ASolver(mode).Solve(Cube.UnsolvedList, Cube.Solved);
            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
        }
    }
}
