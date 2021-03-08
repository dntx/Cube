using System;

namespace sq1code
{
    class Program
    {
        static void Main(string[] args)
        {
            //DoBfsSolve();
            DoASolve();
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

        private static void DoASolve() {
            DateTime startTime = DateTime.Now;
            bool successful = true;
            successful &= ASolve(Goal.SolveShape, ASolver.Mode.ASearch);
            successful &= ASolve(Goal.SolveL1Quarter123, ASolver.Mode.ASearch);
            successful &= ASolve(Goal.SolveL1Quarter4, ASolver.Mode.ASearch);
            successful &= ASolve(Goal.SolveL3Cell01, ASolver.Mode.ASearch);
            successful &= ASolve(Goal.SolveL3Cell2, ASolver.Mode.ASearch);
            successful &= ASolve(Goal.SolveL3Cell3, ASolver.Mode.ASearch);
            successful &= ASolve(Goal.SolveL3Cell46, ASolver.Mode.ASearch);
            //successful &= ASolve(Goal.SolveL3Cell57Then, ASolver.Mode.ASearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell57, ASolver.Mode.ASearch);
            //successful &= ASolver.Solve(Goal.SolveL3Cell46Then, ASolver.Mode.ASearch);
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
