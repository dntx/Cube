using System;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoBfsSolve();
            DoASolve(false);
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

        private static void DoASolve(bool reverseSearch) {
            DateTime startTime = DateTime.Now;
            bool successful = true;
            successful &= ASolve(Goal.SolveShape, reverseSearch);
            successful &= ASolve(Goal.SolveL1Quarter123, reverseSearch);
            successful &= ASolve(Goal.SolveL1Quarter4, reverseSearch);
            successful &= ASolve(Goal.SolveL3Cell01, reverseSearch);
            successful &= ASolve(Goal.SolveL3Cell2, reverseSearch);
            successful &= ASolve(Goal.SolveL3Cell3, reverseSearch);
            successful &= ASolve(Goal.SolveL3Cell46, reverseSearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell57Then, reverseSearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell57, reverseSearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell46Then, reverseSearch);
            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
        }

        private static bool ASolve(Goal goal, bool reverseSearch) {
            Console.WriteLine("start {0} ...", goal);
            bool successful = ASolver.Solve(goal, reverseSearch);
            Console.WriteLine("end {0}, successful: {1}", goal, successful);
            Console.WriteLine("############################################");
            Console.WriteLine();
            return successful;
        }
    }
}
