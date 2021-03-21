using System;

namespace Cube.Sq1BitCube
{
    class Solver
    {
        public static bool Solve(ASolver.Mode mode)
        {
            DateTime startTime = DateTime.Now;
            bool successful = true;

            successful &= DoASolve(Cube.CellDepth6, mode);
            //successful &= DoASolve(Cube.Cell46Swapped, mode);
            //successful &= DoASolve(Cube.Cell57Swapped, mode);

            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
            Console.WriteLine();
            return successful;
        }

        private static bool DoASolve(Cube cube, ASolver.Mode mode) {
            return DoASolve(cube, mode, int.MaxValue);
        }

        private static bool DoASolve(Cube cube, ASolver.Mode mode, int maxStateCount) {
            Console.WriteLine("start solving {0} ...", cube);
            
            ASolver.CreatePredictor createPredictor = (targetCube => new Predictor(targetCube));
            ASolver solver = new ASolver(mode, maxStateCount);
            bool successful = solver.Solve(cube, Cube.Solved, createPredictor);

            Console.WriteLine("end solving {0}, successful: {1}", cube, successful);
            Console.WriteLine("############################################");
            Console.WriteLine();
            return successful;
        }
    }
}
