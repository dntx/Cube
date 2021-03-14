using System;

namespace Cube.Sq1Cube
{
    class Solver
    {
        public static bool Solve()
        {
            //ASolver.Mode mode = ASolver.Mode.ReverseSearch;
            ASolver.Mode mode = ASolver.Mode.ASearch;
            //ASolver.Mode mode = ASolver.Mode.BiDiSearch;

            DateTime startTime = DateTime.Now;
            bool successful = true;

            successful &= DoASolve(Goal.SolveL1Quarter123, mode);
            successful &= DoASolve(Goal.SolveL1Quarter4, mode);
            successful &= DoASolve(Goal.SolveL3Cell01, mode);
            successful &= DoASolve(Goal.SolveL3Cell2, mode);
            successful &= DoASolve(Goal.SolveL3Cell3, mode);
            successful &= DoASolve(Goal.SolveL3Cell46, mode);
            successful &= DoASolve(Goal.SolveL3Cell57Then, mode, 1000000);
            //successful &= DoASolve(Goal.SolveL3Cell57, mode);
            //successful &= DoASolve(Goal.SolveL3Cell46Then, mode);

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
            switch (goal)
            {
                // L1 strategy
                case Goal.SolveL1Quarter123:
                    return solver.Solve(Sq1Cube.Cube.L1Quarter123UnsolvedList, Sq1Cube.Cube.L1Quarter123Solved);

                case Goal.SolveL1Quarter4:
                    return solver.Solve(Sq1Cube.Cube.L1Quarter4UnsolvedList, Sq1Cube.Cube.L1Quarter4Solved);

                // L3 strategy 1
                case Goal.SolveL3Cross:
                    return solver.Solve(Sq1Cube.Cube.L3CrossUnsolvedList, Sq1Cube.Cube.L3CrossSolved);

                case Goal.SolveL3CornersThen:
                    throw new NotImplementedException();

                // // L3 strategy 2
                case Goal.SolveL3Corners:
                    throw new NotImplementedException();

                case Goal.SolveL3CrossThen:
                    throw new NotImplementedException();

                // L3 strategy 3
                case Goal.SolveL3Cell01:
                    return solver.Solve(Sq1Cube.Cube.L3Cell01UnsolvedList, Sq1Cube.Cube.L3Cell01Solved);

                case Goal.SolveL3Cell2:
                    return solver.Solve(Sq1Cube.Cube.L3Cell012UnsolvedList, Sq1Cube.Cube.L3Cell012Solved);

                case Goal.SolveL3Cell3:
                    return solver.Solve(Sq1Cube.Cube.L3Cell0123UnsolvedList, Sq1Cube.Cube.L3Cell0123Solved);

                // L3 strategy 3.1
                case Goal.SolveL3Cell46:
                    return solver.Solve(Sq1Cube.Cube.L3Cell012364, Sq1Cube.Cube.L3Cell012346);

                case Goal.SolveL3Cell57Then:
                    return solver.Solve(Sq1Cube.Cube.L3Cell01234765, Sq1Cube.Cube.Solved);

                // L3 strategy 3.2
                case Goal.SolveL3Cell57:
                    return solver.Solve(Sq1Cube.Cube.L3Cell012375, Sq1Cube.Cube.L3Cell012357);

                case Goal.SolveL3Cell46Then:
                    return solver.Solve(Sq1Cube.Cube.L3Cell01236547, Sq1Cube.Cube.Solved);

                // scratch
                case Goal.Scratch:
                    return solver.Solve(Sq1Cube.Cube.Solved, Sq1Cube.Cube.L1L3Cell08Swapped);
            }
            return false;
        }
    }
}
