using System;

namespace Cube.Sq1BitCube
{
    class Solver
    {
        public static bool Solve(ASolver.Mode mode)
        {
            DateTime startTime = DateTime.Now;
            bool successful = true;

            successful &= DoASolve(Goal.SwapCell46, mode);
            //successful &= DoASolve(Goal.SwapCell57, mode);

            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
            Console.WriteLine();
            return successful;
        }

        private static bool DoASolve(Goal goal, ASolver.Mode mode) {
            return DoASolve(goal, mode, int.MaxValue);
        }

        private static bool DoASolve(Goal goal, ASolver.Mode mode, int maxStateCount) {
            Console.WriteLine("start {0} ...", goal);
            
            bool successful = DoASolve(new ASolver(mode, maxStateCount), goal);

            Console.WriteLine("end {0}, successful: {1}", goal, successful);
            Console.WriteLine("############################################");
            Console.WriteLine();
            return successful;
        }

        private static bool DoASolve(ASolver solver, Goal goal) {
            ASolver.CreatePredictor createPredictor = (targetCube => new Predictor(targetCube));

            switch (goal)
            {
                case Goal.SwapCell46:
                    return solver.Solve(Cube.Cell46Swapped, Cube.Solved, createPredictor);

                case Goal.SwapCell57:
                    return solver.Solve(Cube.Cell57Swapped, Cube.Solved, createPredictor);
            }
            return false;
        }
    }
}
