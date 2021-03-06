using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code
{
    class ASolver {
        public static bool Solve(Goal goal) {
            switch (goal)
            {
                case Goal.SolveShape:
                    return SolveSq1Cube(
                        Cube.ShapeUnsolvedList,
                        Cube.ShapeSolved,
                        rotation => true);

                // L1 strategy 1
                case Goal.SolveL1Quarter123:
                    return SolveSq1Cube(
                        Cube.L1Quarter123UnsolvedList, 
                        Cube.L1Quarter123Solved, 
                        rotation => rotation.IsSquareShapeLocked());

                case Goal.SolveL1Quarter4:
                    return SolveSq1Cube(
                        Cube.L1Quarter4UnsolvedList, 
                        Cube.L1Quarter4Solved,
                        rotation => rotation.IsSquareShapeLocked());

                // // L1 strategy 2
                // case Goal.SolveUpDownColor:
                //     return SolveSq1Cube(
                //         Cube.UpDownColorSolved, 
                //         cube => cube.IsUpDwonColorGrouped(), 
                //         rotation => rotation.IsSquareShapeLocked());

                // case Goal.SolveL1:
                //     throw new NotImplementedException();

                // L3 strategy 1
                case Goal.SolveL3Cross:
                    return SolveSq1Cube(
                        Cube.L3CrossUnsolvedList,
                        Cube.L3CrossSolved, 
                        rotation => rotation.IsSquareQuarterLocked());

                // case Goal.SolveL3CornersThen:
                //     return SolveSq1Cube(
                //         Cube.Solved,
                //         cube => cube.IsL3CrossSolved(),
                //         rotation => rotation.IsSquareShapeLocked());

                // // L3 strategy 2
                // case Goal.SolveL3Quarter1:
                //     return SolveSq1Cube(
                //         Cube.L3Cell01Solved, 
                //         cube => cube.IsL1Solved(), 
                //         rotation => rotation.IsSquareShapeLocked());

                // case Goal.SolveL3Quarter2:
                //     return SolveSq1Cube(
                //         Cube.L3Cell0123Solved, 
                //         cube => cube.IsL3CellSolved(0, 1),
                //         rotation => rotation.IsSquareQuarterLocked());

                // case Goal.SolveL3Quarter34:
                //     return SolveSq1Cube(
                //         Cube.Solved,
                //         cube => cube.IsL3CellSolved(0, 1, 2, 3),
                //         rotation => rotation.IsSquareShapeLocked());

                // L3 strategy 3
                case Goal.SolveL3Cell01:
                    return SolveSq1Cube(
                        Cube.L3Cell01UnsolvedList, 
                        Cube.L3Cell01Solved, 
                        rotation => rotation.IsSquareShapeLocked());

                case Goal.SolveL3Cell2:
                    return SolveSq1Cube(
                        Cube.L3Cell012UnsolvedList, 
                        Cube.L3Cell012Solved, 
                        rotation => rotation.IsSquareQuarterLocked());

                case Goal.SolveL3Cell3:
                    return SolveSq1Cube(
                        Cube.L3Cell0123UnsolvedList, 
                        Cube.L3Cell0123Solved,
                        rotation => rotation.IsSquareShapeLocked());

                // L3 strategy 3.1
                case Goal.SolveL3Cell46:
                    return SolveSq1Cube(
                        Cube.L3Cell012364, 
                        Cube.L3Cell012346,
                        rotation => rotation.IsSquareShapeLocked());

                case Goal.SolveL3Cell57Then:
                    return SolveSq1Cube(
                        Cube.L3Cell01234765, 
                        Cube.Solved,
                        rotation => rotation.IsSquareShapeLocked());

                // L3 strategy 3.2
                case Goal.SolveL3Cell57:
                    return SolveSq1Cube(
                        Cube.L3Cell012375,
                        Cube.L3Cell012357,
                        rotation => rotation.IsSquareShapeLocked());

                case Goal.SolveL3Cell46Then:
                    return SolveSq1Cube(
                        Cube.L3Cell01236547, 
                        Cube.Solved,
                        rotation => rotation.IsSquareShapeLocked());


                case Goal.SolveScratch:
                    return SolveSq1Cube(
                        Cube.Solved, 
                        Cube.L1L3Cell08Swapped,
                        rotation => rotation.IsSquareShapeLocked());


                // L3 strategy 4
                // case Goal.SolveL3QuarterPairs:
                //     throw new NotImplementedException();

                // case Goal.SolveL3QuarterPosition:
                //     return SolveSq1Cube(
                //         Cube.Solved, 
                //         cube => cube.IsL1Solved(), 
                //         rotation => rotation.IsSquareQuarterLocked());
            }
            return false;
        }

        private static bool SolveSq1Cube(Cube startCube, Cube targetCube, Predicate<Rotation> IsFocusRotation) {
            return SolveSq1Cube(new List<Cube>{startCube}, targetCube, IsFocusRotation);
        }

        private static bool SolveSq1Cube(List<Cube> startCubeList, Cube targetCube, Predicate<Rotation> IsFocusRotation) {
            DateTime startTime = DateTime.Now;
            int solvedCount = 0;
            for (int i = 0; i < startCubeList.Count; i++) {
                Console.WriteLine("searching solution {0}/{1} ...", i + 1, startCubeList.Count);
                bool successful = SolveSq1CubeKernel(startCubeList[i], targetCube, IsFocusRotation);
                if (successful) {
                    solvedCount++;
                }
                Console.WriteLine("-----------------------------------");
                Console.WriteLine();
            }
            Console.WriteLine("total seconds: {0:0.00}, total request: {1}, solved: {2}", 
                DateTime.Now.Subtract(startTime).TotalSeconds,
                startCubeList.Count,
                solvedCount);

            return solvedCount == startCubeList.Count;
        }

        private static bool SolveSq1CubeKernel(Cube startCube, Cube targetCube, Predicate<Rotation> IsFocusRotation) {
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

            AState targetState = null;
            do {
                AState state = openStates.Min;
                openStates.Remove(state);
                state.IsClosed = true;

                Cube cube = state.Cube;
                List<Rotation> rotations = cube.GetRotations();
                List<Rotation> focusRotations = rotations.FindAll(IsFocusRotation);

                int nextDepth = state.Depth + 1;
                foreach (Rotation rotation in focusRotations) {
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
                        int predictedCost = APredictor.PredictCost(nextCube, targetCube);
                        int nextCubeId = seenCubeStates.Count();
                        AState nextState = new AState(nextCube, nextCubeId, predictedCost, state, rotation);
                        netEdgeCount++;

                        openStates.Add(nextState);
                        seenCubeStates.Add(nextCube, nextState);

                        if (nextCube == targetCube) {
                            targetState = nextState;
                            break;
                        }
                    }
                }

                closedStateCount++;
                if (targetState != null || openStates.Count == 0 || closedStateCount % 1000 == 0) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "seconds: {0:0.00}, {1}, depth: {2}, h: {3}, closed: {4}({5:p}), open: {6}({7:p}), reopened: {8}({9:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Cube,
                        state.Depth,
                        state.PredictedCost, 
                        closedStateCount, 
                        (float)closedStateCount / totalCount,
                        openStates.Count,
                        (float)openStates.Count / totalCount,
                        reopenStateCount,
                        (float)reopenStateCount / openStates.Count
                        );
                }
            } while (targetState == null && openStates.Count > 0);
            Console.WriteLine();

            if (targetState == null) {
                return false;
            }

            OutputSolution(startState, targetState);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, closed: {1}", seenCubeStates.Count, closedStateCount);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private static void OutputSolution(AState startState, AState targetState) {
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
                targetState.Cube,
                targetState.CubeId
                );
        }
   }
}