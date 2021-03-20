using System;

namespace Cube.Sq1List2Cube
{
    class Solver
    {
        public static bool Solve(ASolver.Mode mode)
        {
            DateTime startTime = DateTime.Now;
            bool successful = new ASolver(mode).Solve(Cube.UnsolvedList, Cube.Solved, targetCube => new Predictor(targetCube));
            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
            Console.WriteLine();
            return successful;
        }
    }
}
