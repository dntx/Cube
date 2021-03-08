using System;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoASolve(ASolver.Mode.ReverseBfSearch);
            DoASolve(ASolver.Mode.ASearch);
        }

        private static void DoASolve(ASolver.Mode mode) {
            DateTime startTime = DateTime.Now;
            bool successful = true;
            successful &= ASolve(Goal.SolveShape, mode);
            successful &= ASolve(Goal.SolveL1Quarter123, mode);
            successful &= ASolve(Goal.SolveL1Quarter4, mode);
            successful &= ASolve(Goal.SolveL3Cell01, mode);
            successful &= ASolve(Goal.SolveL3Cell2, mode);
            successful &= ASolve(Goal.SolveL3Cell3, mode);
            successful &= ASolve(Goal.SolveL3Cell46, mode);
            //successful &= ASolve(Goal.SolveL3Cell57Then, mode);
            successful &= ASolve(Goal.SolveL3Cell57, mode);
            //successful &= ASolve(Goal.SolveL3Cell46Then, mode);
            Console.WriteLine("total seconds: {0:0.00}, successful: {1}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                successful);
        }

        private static bool ASolve(Goal goal, ASolver.Mode mode) {
            Console.WriteLine("start {0} ...", goal);
            bool successful = new ASolver(mode).Solve(goal);
            Console.WriteLine("end {0}, successful: {1}", goal, successful);
            Console.WriteLine("############################################");
            Console.WriteLine();
            return successful;
        }
    }
}
