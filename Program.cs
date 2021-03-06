using System;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //BfsSolver.Solve(Goal.SolveShape);
            //BfsSolver.Solve(Goal.SolveL1Quarter123);
            //BfsSolver.Solve(Goal.SolveL1Quarter4);
            //BfsSolver.Solve(Goal.SolveL3Cross);
            //BfsSolver.Solve(Goal.SolveL3Corners);

            //BfsSolver.Solve(Goal.SolveShape);
            //BfsSolver.Solve(Goal.SolveL1Quarter123);
            //BfsSolver.Solve(Goal.SolveL1Quarter4);
            //BfsSolver.Solve(Goal.SolveL3Cell01);
            //BfsSolver.Solve(Goal.SolveL3Cell2);
            //BfsSolver.Solve(Goal.SolveL3Cell3);
            //BfsSolver.Solve(Goal.SolveL3Cell46);
            //BfsSolver.Solve(Goal.SolveL3Cell57Then);
            //BfsSolver.Solve(Goal.SolveL3Cell57);
            //BfsSolver.Solve(Goal.SolveL3Cell46Then);

            //BfsSolver.Solve(Goal.SolveScratch);


            DateTime startTime = DateTime.Now;
            bool successful = true;
            successful &= ASolve(Goal.SolveShape);
            successful &= ASolve(Goal.SolveL1Quarter123);
            successful &= ASolve(Goal.SolveL1Quarter4);
            successful &= ASolve(Goal.SolveL3Cell01);
            successful &= ASolve(Goal.SolveL3Cell2);
            successful &= ASolve(Goal.SolveL3Cell3);
            successful &= ASolve(Goal.SolveL3Cell46);
            //successful &= ASolver.Solve(Goal.SolveL3Cell57Then);
            //successful &= ASolver.Solve(Goal.SolveL3Cell57);
            //successful &= ASolver.Solve(Goal.SolveL3Cell46Then);
            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
        }

        private static bool ASolve(Goal goal) {
            Console.WriteLine("start {0}", goal);
            bool successful = ASolver.Solve(goal);
            Console.WriteLine("end {0}, successful: {1}", goal, successful);
            Console.WriteLine();
            return successful;
        }
    }
}
