using System;
using System.Collections.Generic;

namespace sq1code
{
    class BfsSolver {
        private static int VisitCube(Cube cube, Dictionary<Cube, int> seenCubes) {
            if (seenCubes.ContainsKey(cube)) {
                return seenCubes[cube];
            }
            
            int cubeId = seenCubes.Count;
            seenCubes.Add(cube, cubeId);
            return cubeId;
        }

        private static void VisitSolution(BfsState state, Cube cubeSolution) {
            state.Solutions.Add(cubeSolution);
            state.Froms.ForEach(from => VisitSolution(from.Key, cubeSolution));
        }

        public static void Solve(Goal goal) {
            Console.WriteLine("start {0}", goal);
            switch (goal)
            {
                case Goal.SolveShape:
                    SolveSq1Cube(
                        Cube.ShapeSolved, 
                        cube => cube.IsUpOrDownHexagram());
                    break;

                // L1 strategy
                case Goal.SolveL1Quarter123:
                    SolveSq1Cube(
                        Cube.L1Quarter123Solved, 
                        cube => cube.IsL1CellSolved(5), 
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                case Goal.SolveL1Quarter4:
                    SolveSq1Cube(
                        Cube.L1Quarter4Solved,
                        cube => cube.IsL1CellSolved(6),
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                // L3 strategy 1
                case Goal.SolveL3Cross:
                    SolveSq1Cube(
                        Cube.L3CrossSolved, 
                        cube => cube.IsL1Solved(),
                        rotation => rotation.IsSquareQuarterLocked());
                    break;

                case Goal.SolveL3CornersThen:
                    SolveSq1Cube(
                        Cube.Solved,
                        cube => cube.IsL3CrossSolved(),
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                // L3 strategy 2
                case Goal.SolveL3Corners:
                    throw new NotImplementedException();

                case Goal.SolveL3CrossThen:
                    throw new NotImplementedException();

                // L3 strategy 3
                case Goal.SolveL3Cell01:
                    SolveSq1Cube(
                        Cube.L3Cell01Solved, 
                        Cube.L3Cell01UnsolvedList, 
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                case Goal.SolveL3Cell2:
                    SolveSq1Cube(
                        Cube.L3Cell012Solved, 
                        Cube.L3Cell012UnsolvedList, 
                        rotation => rotation.IsSquareQuarterLocked());
                    break;

                case Goal.SolveL3Cell3:
                    SolveSq1Cube(
                        Cube.L3Cell0123Solved,
                        Cube.L3Cell0123UnsolvedList, 
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                // L3 strategy 3.1
                case Goal.SolveL3Cell46:
                    SolveSq1Cube(
                        Cube.L3Cell012346,
                        Cube.L3Cell012364, 
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                case Goal.SolveL3Cell57Then:
                    SolveSq1Cube(
                        Cube.Solved,
                        Cube.L3Cell01234765, 
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                // L3 strategy 3.2
                case Goal.SolveL3Cell57:
                    SolveSq1Cube(
                        Cube.L3Cell012357,
                        Cube.L3Cell012375,
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                case Goal.SolveL3Cell46Then:
                    SolveSq1Cube(
                        Cube.Solved,
                        Cube.L3Cell01236547, 
                        rotation => rotation.IsSquareShapeLocked());
                    break;

                // scratch
                case Goal.Scratch:
                    SolveSq1Cube(
                        Cube.Solved, 
                        cube => false,
                        rotation => rotation.IsSquareShapeLocked());
                    break;
            }
            Console.WriteLine("end");
        }

        private static void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube) {
            SolveSq1Cube(startCube, IsTargetCube, targetCubeCount: int.MaxValue, IsFocusRotation: rotation => true);
        }
        
        private static void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, Predicate<Rotation> IsFocusRotation) {
            SolveSq1Cube(startCube, IsTargetCube, targetCubeCount: int.MaxValue, IsFocusRotation);
        }

        private static void SolveSq1Cube(Cube startCube, Cube targetCube, Predicate<Rotation> IsFocusRotation) {
            SolveSq1Cube(startCube, cube => cube == targetCube, targetCubeCount: 1, IsFocusRotation);
        }

        private static void SolveSq1Cube(Cube startCube, ICollection<Cube> targetCubes, Predicate<Rotation> IsFocusRotation) {
            SolveSq1Cube(startCube, cube => targetCubes.Contains(cube), targetCubes.Count, IsFocusRotation);
        }
        
        private static void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, int targetCubeCount, Predicate<Rotation> IsFocusRotation) {
            Console.WriteLine("target cube count: {0}", targetCubeCount);
            DateTime startTime = DateTime.Now;
            Predicate<BfsState> IsTargetState = (state => state.Depth > 0 && IsTargetCube(state.Cube));

            int closedStateCount = 0;
            int solutionCount = 0;

            int totalEdgeCount = 0;
            int netEdgeCount = 0;
            Queue<BfsState> openStates = new Queue<BfsState>();
            int[] openStateCountByDepth = new int[100];
            List<BfsState> seenStates = new List<BfsState>();

            Dictionary<Cube, int> seenCubes = new Dictionary<Cube, int>();
            VisitCube(startCube, seenCubes);

            BfsState startState = new BfsState(startCube, 0);
            openStates.Enqueue(startState);
            openStateCountByDepth[startState.Depth]++;
            seenStates.Add(startState);
            do {
                BfsState state = openStates.Dequeue();
                openStateCountByDepth[state.Depth]--;
                Cube cube = state.Cube;
                bool shouldSkip = (solutionCount >= targetCubeCount);
                if (!shouldSkip) {
                    List<Rotation> rotations = cube.GetRotations();
                    List<Rotation> focusRotations = rotations.FindAll(IsFocusRotation);

                    int nextDepth = state.Depth + 1;
                    foreach (Rotation rotation in focusRotations) {
                        Cube nextCube = cube.RotateBy(rotation);
                        totalEdgeCount++;
                        int nextCubeId = VisitCube(nextCube, seenCubes);
                        if (nextCubeId < seenStates.Count) {    
                            // existing cube
                            BfsState existingState = seenStates[nextCubeId];
                            if (nextDepth < existingState.Depth) {
                                // a better path found, should not happen as we are using BFS
                                throw new Exception("error: a better path found in BFS");
                            } else if (nextDepth == existingState.Depth) {
                                // an alternative path may found, update if necessary
                                if (existingState.Froms.TrueForAll(from => from.Key != state)) {
                                    existingState.AddFrom(state, rotation);
                                    netEdgeCount++;
                                }
                            }
                        } else {    
                            // new cube
                            BfsState nextState = new BfsState(nextCube, nextCubeId, state, rotation);
                            netEdgeCount++;
                            openStates.Enqueue(nextState);
                            openStateCountByDepth[nextState.Depth]++;

                            if (IsTargetState(nextState)) {
                                solutionCount++;
                            }
                            seenStates.Add(nextState);
                        }
                    }
                }

                closedStateCount++;
                if (closedStateCount == 1 || openStates.Count == 0 || (!shouldSkip && closedStateCount % 1000 == 0)) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "seconds: {0:0.00}, depth: {1}, solution: {2}, closed: {3}({4:p}), open[{5}]: {6}({7:p}), open[{8}]: {9}({10:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Depth, 
                        solutionCount,
                        closedStateCount, 
                        (float)closedStateCount / totalCount,
                        state.Depth,
                        openStateCountByDepth[state.Depth], 
                        (float)openStateCountByDepth[state.Depth] / totalCount,
                        state.Depth + 1,
                        openStateCountByDepth[state.Depth + 1], 
                        (float)openStateCountByDepth[state.Depth + 1] / totalCount
                        );
                }
            } while (openStates.Count > 0);
            Console.WriteLine();

            seenStates.ForEach(state => {
                if (IsTargetState(state)) {
                    VisitSolution(state, state.Cube);
                }
            });

            seenStates.ForEach(state => state.CalculateBestFrom());

            seenStates.ForEach(state => {
                if (IsTargetState(state)) {
                    OutputState(state);
                    Console.WriteLine();
                }
            });

            Console.WriteLine("cubes: {0}, closed: {1}", seenCubes.Count, closedStateCount);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
        }

        private static void OutputState(BfsState state) {
            Console.WriteLine("cube: {0}", state.Cube);
            Console.WriteLine("depth：{0}", state.Depth);
            do {
                BfsState fromState = state.BestFrom.Key;
                Rotation fromRotation = state.BestFrom.Value;

                // todo: consider up/down reverse situation if necessary
                // todo: consider change case 301-0101 to 0101-301
                Cube rotatedCube = (fromState != null)? fromState.Cube.RotateBy(fromRotation) : state.Cube;

                Console.WriteLine(
                    " ==> {0} | {1,2}({2,2})", 
                    rotatedCube.ToString(verbose: true),
                    state.CubeId,
                    state.Solutions.Count
                    );
                state = fromState;
            } while (state != null);
        }
   }
}