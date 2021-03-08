using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code
{
    class ASolver {
        public enum Mode {
            ASearch,
            ReverseBfSearch
        }

        private Mode mode;

        public ASolver(Mode mode) {
            this.mode = mode;
        }

        public bool Solve(Goal goal) {
            switch (goal)
            {
                case Goal.SolveShape:
                    return SolveSq1Cube(
                        Cube.ShapeUnsolvedList,
                        Cube.ShapeSolved);

                // L1 strategy
                case Goal.SolveL1Quarter123:
                    return SolveSq1Cube(
                        Cube.L1Quarter123UnsolvedList, 
                        Cube.L1Quarter123Solved);

                case Goal.SolveL1Quarter4:
                    return SolveSq1Cube(
                        Cube.L1Quarter4UnsolvedList, 
                        Cube.L1Quarter4Solved);

                // L3 strategy 1
                case Goal.SolveL3Cross:
                    return SolveSq1Cube(
                        Cube.L3CrossUnsolvedList,
                        Cube.L3CrossSolved);

                case Goal.SolveL3CornersThen:
                    throw new NotImplementedException();

                // // L3 strategy 2
                case Goal.SolveL3Corners:
                    throw new NotImplementedException();

                case Goal.SolveL3CrossThen:
                    throw new NotImplementedException();

                // L3 strategy 3
                case Goal.SolveL3Cell01:
                    return SolveSq1Cube(
                        Cube.L3Cell01UnsolvedList, 
                        Cube.L3Cell01Solved);

                case Goal.SolveL3Cell2:
                    return SolveSq1Cube(
                        Cube.L3Cell012UnsolvedList, 
                        Cube.L3Cell012Solved);

                case Goal.SolveL3Cell3:
                    return SolveSq1Cube(
                        Cube.L3Cell0123UnsolvedList, 
                        Cube.L3Cell0123Solved);

                // L3 strategy 3.1
                case Goal.SolveL3Cell46:
                    return SolveSq1Cube(
                        Cube.L3Cell012364, 
                        Cube.L3Cell012346);

                case Goal.SolveL3Cell57Then:
                    return SolveSq1Cube(
                        Cube.L3Cell01234765, 
                        Cube.Solved);

                // L3 strategy 3.2
                case Goal.SolveL3Cell57:
                    return SolveSq1Cube(
                        Cube.L3Cell012375,
                        Cube.L3Cell012357);

                case Goal.SolveL3Cell46Then:
                    return SolveSq1Cube(
                        Cube.L3Cell01236547, 
                        Cube.Solved);

                // scratch
                case Goal.Scratch:
                    return SolveSq1Cube(
                        Cube.Solved, 
                        Cube.L1L3Cell08Swapped);
            }
            return false;
        }

        private bool SolveSq1Cube(Cube startCube, Cube targetCube) {
            bool lockSquareShape = startCube.IsShapeSolved() && targetCube.IsShapeSolved();
            if (mode == Mode.ReverseBfSearch) {
                return SolveSq1CubeKernel(targetCube, new HashSet<Cube>{startCube}, lockSquareShape);
            } else {
                return SolveSq1CubeKernel(startCube, new HashSet<Cube>{targetCube}, lockSquareShape);
            }
        }

        private bool SolveSq1Cube(ICollection<Cube> startCubes, Cube targetCube) {
            if (mode == Mode.ReverseBfSearch) {
                bool lockSquareShape = startCubes.All(cube => cube.IsShapeSolved()) && targetCube.IsShapeSolved();
                return SolveSq1CubeKernel(targetCube, startCubes, lockSquareShape);
            } else {
                DateTime startTime = DateTime.Now;
                Console.WriteLine("total request for \"{0}\": {1}", targetCube, startCubes.Count);

                int solvedCount = 0;
                int searchedCount = 0;
                foreach (Cube startCube in startCubes) {
                    searchedCount++;
                    Console.WriteLine("searching solution for \"{0}\": {1}/{2} ...", targetCube, searchedCount, startCubes.Count);
                    bool successful = SolveSq1Cube(startCube, targetCube);
                    if (successful) {
                        solvedCount++;
                    }
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine();
                }
                Console.WriteLine("total seconds: {0:0.00}, total request for \"{1}\": {2}, solved: {3}", 
                    DateTime.Now.Subtract(startTime).TotalSeconds,
                    targetCube,
                    startCubes.Count,
                    solvedCount);
                Console.WriteLine("=======================================");
                Console.WriteLine();

                return solvedCount == startCubes.Count;
            }
        }

        private bool SolveSq1CubeKernel(Cube startCube, ICollection<Cube> targetCubes, bool lockSquareShape) {
            DateTime startTime = DateTime.Now;

            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;

            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<Cube, AState> seenCubeStates = new Dictionary<Cube, AState>();
            AState startState = new AState(startCube, 0);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);

            Dictionary<Cube, AState> targetStates = new Dictionary<Cube, AState>();
            bool completed = false;
            do {
                AState state = openStates.Min;
                openStates.Remove(state);
                state.IsClosed = true;

                Cube cube = state.Cube;
                List<Rotation> rotations = cube.GetRotations(lockSquareShape);

                int nextDepth = state.Depth + 1;
                foreach (Rotation rotation in rotations) {
                    Cube nextCube = cube.RotateBy(rotation);
                    totalEdgeCount++;
                    if (seenCubeStates.ContainsKey(nextCube)) {
                        // existing cube
                        AState existingState = seenCubeStates[nextCube];
                        if (nextDepth < existingState.Depth) {
                            // a better path found
                            if (existingState.IsClosed) {
                                existingState.IsClosed = false;
                                closedStateCount--;
                                reopenStateCount++;
                            } else {
                                openStates.Remove(existingState);
                            }
                            existingState.UpdateFrom(state, rotation);
                            openStates.Add(existingState);
                        }
                    } else {
                        // new cube
                        int predictedCost = (mode == Mode.ReverseBfSearch) ? 0 : APredictor.PredictCost(nextCube, targetCubes);
                        int nextCubeId = seenCubeStates.Count();
                        AState nextState = new AState(nextCube, nextCubeId, predictedCost, state, rotation);
                        netEdgeCount++;

                        openStates.Add(nextState);
                        seenCubeStates.Add(nextCube, nextState);

                        if (targetCubes.Contains(nextCube)) {
                            targetStates[nextCube] = nextState;
                            if (targetStates.Count == targetCubes.Count) {
                                completed = true;
                                break;
                            }
                        }
                    }
                }

                closedStateCount++;
                if (completed || openStates.Count == 0 || closedStateCount % 10000 == 0) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "seconds: {0:0.00}, {1}, depth: {2}, h: {3}, solutions: {4}, closed: {5}({6:p}), open: {7}({8:p}), reopened: {9}({10:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Cube,
                        state.Depth,
                        state.PredictedCost, 
                        targetStates.Count,
                        closedStateCount, 
                        (float)closedStateCount / totalCount,
                        openStates.Count,
                        (float)openStates.Count / totalCount,
                        reopenStateCount,
                        (float)reopenStateCount / openStates.Count
                        );
                }
            } while (!completed && openStates.Count > 0);
            Console.WriteLine();

            if (!completed) {
                return false;
            }

            OutputSolutions(startState, targetStates.Values);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, solutions: {1}, closed: {2}", seenCubeStates.Count, targetStates.Count, closedStateCount);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private void OutputSolutions(AState startState, ICollection<AState> targetStates) {
            foreach (AState targetState in targetStates) {
                if (mode == Mode.ReverseBfSearch) {
                    ReverseOutputSolution(startState, targetState);
                } else {
                    OutputSolution(startState, targetState);
                }
                Console.WriteLine();
            }
        }

        private void ReverseOutputSolution(AState startState, AState targetState) {
            Console.WriteLine("cube: {0}", targetState.Cube);
            Console.WriteLine("depth：{0}", targetState.Depth);
            AState state = targetState;
            do {
                AState fromState = state.FromState;
                Rotation fromRotation = state.FromRotation;

                // todo: consider up/down reverse situation if necessary
                // todo: consider change case 301-0101 to 0101-301
                Cube rotatedCube = (fromState != null)? fromState.Cube.RotateBy(fromRotation) : state.Cube;

                Console.WriteLine(
                    " ==> {0} | {1}", 
                    rotatedCube.ToString(verbose: true),
                    state.CubeId
                    );
                state = fromState;
            } while (state != null);
        }

        private void OutputSolution(AState startState, AState targetState) {
            Console.WriteLine("cube: {0}", startState.Cube);
            Console.WriteLine("depth：{0}", targetState.Depth);
            
            Stack<AState> solutionPath = new Stack<AState>();
            for (AState state = targetState; state.FromState != null; state = state.FromState) {
                solutionPath.Push(state);
            }

            while (solutionPath.Count > 0) {
                AState state = solutionPath.Pop();
                Console.WriteLine(
                    " ==> {0} | {1}", 
                    state.FromRotation,
                    state.FromState.CubeId
                    );
            }
            Console.WriteLine(
                " ==> {0} | {1}", 
                targetState.Cube.ToString(verbose: true),
                targetState.CubeId
                );
         }
   }
}