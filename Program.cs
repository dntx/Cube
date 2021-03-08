using System;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoBfsSolve();
            DoASolve(true);
        }

        private static void DoBfsSolve() {
            //BfsSolver.Solve(Goal.SolveShape);
            //BfsSolver.Solve(Goal.SolveL1Quarter123);
            //BfsSolver.Solve(Goal.SolveL1Quarter4);
            //BfsSolver.Solve(Goal.SolveL3Cell01);
            //BfsSolver.Solve(Goal.SolveL3Cell2);
            //BfsSolver.Solve(Goal.SolveL3Cell3);
            BfsSolver.Solve(Goal.SolveL3Cell46);
            //BfsSolver.Solve(Goal.SolveL3Cell57Then);
            
            //BfsSolver.Solve(Goal.SolveL3Cell57);
            //BfsSolver.Solve(Goal.SolveL3Cell46Then);
        }

        private static void DoASolve(bool reverseBfsSearch) {
            DateTime startTime = DateTime.Now;
            bool successful = true;
            // successful &= ASolve(Goal.SolveShape, reverseBfsSearch);
            // successful &= ASolve(Goal.SolveL1Quarter123, reverseBfsSearch);
            // successful &= ASolve(Goal.SolveL1Quarter4, reverseBfsSearch);
            // successful &= ASolve(Goal.SolveL3Cell01, reverseBfsSearch);
            // successful &= ASolve(Goal.SolveL3Cell2, reverseBfsSearch);
            // successful &= ASolve(Goal.SolveL3Cell3, reverseBfsSearch);
            // successful &= ASolve(Goal.SolveL3Cell46, reverseBfsSearch);
            successful &= ASolver.Solve(Goal.SolveL3Cell57Then, reverseBfsSearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell57, reverseBfsSearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell46Then, reverseBfsSearch);
            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
        }

        private static bool ASolve(Goal goal, bool reverseBfsSearch) {
            Console.WriteLine("start {0} ...", goal);
            bool successful = ASolver.Solve(goal, reverseBfsSearch);
            Console.WriteLine("end {0}, successful: {1}", goal, successful);
            Console.WriteLine("############################################");
            Console.WriteLine();
            return successful;
        }
    }
}
