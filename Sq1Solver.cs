using System;
using System.Collections.Generic;

namespace sq1code
{
    class Sq1Solver {
        public enum Goal { 
            SolveShape,

            // L1 strategy 1
            SolveL1Quarter123,
            SolveL1Quarter4,

            // L1 strategy 2
            SolveUpDownColor,
            SolveL1,

                /// L3 strategy 1
                SolveL3Cross,
                SolveL3Corner34,

                /// L3 strategy 2
                SolveL3Quarter1,
                SolveL3Quarter2,
                SolveL3Quarter34,

                /// L3 strategy 3
                SolveL3QuarterPairs,
                SolveL3QuarterPosition
        };

        Dictionary<Cube, int> seenCubes = new Dictionary<Cube, int>();
        Dictionary<Layer, int> seenLayers = new Dictionary<Layer, int>();
        Dictionary<Half, int> seenHalfs = new Dictionary<Half, int>();

        public Sq1Solver() {
            seenCubes = new Dictionary<Cube, int>();
            seenLayers = new Dictionary<Layer, int>();
            seenHalfs = new Dictionary<Half, int>();
        }

        private int VisitCube(Cube cube) {
            if (seenCubes.ContainsKey(cube)) {
                return seenCubes[cube];
            }
            
            int cubeId = seenCubes.Count;
            seenCubes.Add(cube, cubeId);
            VisitLayer(cube.Up);
            VisitLayer(cube.Down);
            return cubeId;
        }

        private bool VisitLayer(Layer layer) {
            // no matter layer is seen or not, 
            // we always need visit the halfs because 
            // different divison may cause different half on the same layer
            VisitHalf(layer.Left);
            VisitHalf(layer.Right);

            if (seenLayers.ContainsKey(layer)) {
                return false;
            }

            seenLayers.Add(layer, seenLayers.Count);
            return true;
        }

        private bool VisitHalf(Half half) {
            if (seenHalfs.ContainsKey(half)) {
                return false;
            }

            seenHalfs.Add(half, seenHalfs.Count);
            return true;
        }

        private void VisitSolution(State state, Cube cubeSolution) {
            state.Solutions.Add(cubeSolution);
            state.Froms.ForEach(from => VisitSolution(from.Key, cubeSolution));
        }

        public void Solve(Goal goal) {
            Console.WriteLine("start {0}", goal);
            switch (goal)
            {
                case Goal.SolveShape:
                    SolveSq1Cube(
                        Cube.ShapeSolvedCube, 
                        cube => cube.IsUpOrDownHexagram());
                    break;

                // L1 strategy 1
                case Goal.SolveL1Quarter123:
                    // using L3 instead L1 to make the NO. of cells in solution easier to read 
                    SolveSq1Cube(
                        Cube.L3Quarter123SolvedCube, 
                        cube => cube.IsL3CellSolved(5), 
                        rotation => rotation.IsShapeIdentical());
                    break;

                case Goal.SolveL1Quarter4:
                    // using L3 instead L1 to make the NO. of cells in solution easier to read 
                    SolveSq1Cube(
                        Cube.L3SolvedCube,
                        cube => cube.IsL3CellSolved(6),
                        rotation => rotation.IsShapeIdentical());
                    break;

                // L1 strategy 2
                case Goal.SolveUpDownColor:
                    SolveSq1Cube(
                        Cube.UpDownColorSolvedCube, 
                        cube => cube.IsUpDwonColorGrouped(), 
                        rotation => rotation.IsShapeIdentical());
                    break;

                case Goal.SolveL1:
                    throw new NotImplementedException();

                // L3 strategy 1
                case Goal.SolveL3Cross:
                    SolveSq1Cube(
                        Cube.SolvedCubeExceptL3Corners, 
                        cube => cube.IsL1Solved(),
                        rotation => rotation.IsQuarterLocked(),
                        firstSolutionOnly: true);
                    break;

                case Goal.SolveL3Corner34:
                    SolveSq1Cube(
                        Cube.SolvedCube,
                        cube => cube.IsSolvedExceptL3Cells(4, 6),
                        rotation => rotation.IsShapeIdentical(),
                        firstSolutionOnly: true);
                    break;

                // L3 strategy 2
                case Goal.SolveL3Quarter1:
                    SolveSq1Cube(
                        Cube.SolvedCubeExceptL3Quarter234, 
                        cube => cube.IsL1Solved(), 
                        rotation => rotation.IsShapeIdentical(),
                        firstSolutionOnly: true);
                    break;

                case Goal.SolveL3Quarter2:
                    SolveSq1Cube(
                        Cube.SolvedCubeExceptL3Quarter34, 
                        cube => cube.IsSolvedExceptL3Cells(2, 3, 4, 5, 6, 7), 
                        rotation => rotation.IsShapeIdentical(),
                        firstSolutionOnly: true);
                    break;

                case Goal.SolveL3Quarter34:
                    SolveSq1Cube(
                        Cube.SolvedCube,
                        cube => cube.IsSolvedExceptL3Cells(4, 5, 6, 7),
                        rotation => rotation.IsShapeIdentical(),
                        firstSolutionOnly: true);
                    break;

                // L3 strategy 3
                case Goal.SolveL3QuarterPairs:
                    throw new NotImplementedException();

                case Goal.SolveL3QuarterPosition:
                    SolveSq1Cube(
                        Cube.SolvedCube, 
                        cube => cube.IsL1Solved(), 
                        rotation => rotation.IsQuarterLocked());
                    break;
            }
            Console.WriteLine("end");
        }

        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube) {
            SolveSq1Cube(startCube, IsTargetCube, firstSolutionOnly: false);
        }

        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, bool firstSolutionOnly) {
            SolveSq1Cube(startCube, IsTargetCube, IsFocusRotation: rotation => true, firstSolutionOnly);
        }
        
        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, Predicate<Rotation> IsFocusRotation) {
            SolveSq1Cube(startCube, IsTargetCube, IsFocusRotation, firstSolutionOnly: false);
        }
        
        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, Predicate<Rotation> IsFocusRotation, bool firstSolutionOnly) {
            DateTime startTime = DateTime.Now;
            Predicate<State> IsTargetState = (state => state.Depth > 0 && IsTargetCube(state.Cube));

            int closedStateCount = 0;
            int solutionCount = 0;
            int minSolutionDepth = int.MaxValue;

            int totalEdgeCount = 0;
            int netEdgeCount = 0;
            Queue<State> openStates = new Queue<State>();
            List<State> seenStates = new List<State>();

            VisitCube(startCube);
            State startState = new State(startCube, 0);
            openStates.Enqueue(startState);
            seenStates.Add(startState);
            do {
                State state = openStates.Dequeue();
                Cube cube = state.Cube;
                if (!firstSolutionOnly || state.Depth < minSolutionDepth) {
                    List<Rotation> rotations = cube.GetRotations();
                    List<Rotation> focusRotations = rotations.FindAll(IsFocusRotation);

                    int nextDepth = state.Depth + 1;
                    foreach (Rotation rotation in focusRotations) {
                        Cube nextCube = cube.RotateBy(rotation);
                        totalEdgeCount++;
                        int nextCubeId = VisitCube(nextCube);
                        if (nextCubeId < seenStates.Count) {    
                            // existing cube
                            State existingState = seenStates[nextCubeId];
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
                            State nextState = new State(nextCube, nextCubeId, state, rotation);
                            netEdgeCount++;
                            openStates.Enqueue(nextState);

                            if (IsTargetState(nextState)) {
                                solutionCount++;
                                if (nextState.Depth < minSolutionDepth) {
                                    minSolutionDepth = nextState.Depth;
                                }
                            }
                            seenStates.Add(nextState);
                        }
                    }
                }

                closedStateCount++;
                if (closedStateCount == 1 || closedStateCount % 1000 == 0 || openStates.Count == 0) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "seconds: {0:0.00}, depth: {1}, solution: {2}, closed: {3}({4:p}), open: {5}({6:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Depth, 
                        solutionCount,
                        closedStateCount, 
                        (float)closedStateCount / totalCount,
                        openStates.Count, 
                        (float)openStates.Count / totalCount
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

        private void OutputState(State state) {
            Console.WriteLine("cube: {0}", state.Cube);
            Console.WriteLine("depth：{0}", state.Depth);
            do {
                State fromState = state.BestFrom.Key;
                Rotation fromRotation = state.BestFrom.Value;

                // todo: consider up/down reverse situation if necessary
                // todo: consider change case 301-0101 to 0101-301
                Cube rotatedCube = (fromState != null)? fromState.Cube.RotateBy(fromRotation) : state.Cube;

                Console.WriteLine(
                    " ==> {0} | {1,2}({2,2}) | {3,2},{4,-2} | {5,2}-{6,-2},{7,2}-{8,-2}", 
                    rotatedCube.ToString(verbose: true),
                    seenCubes[state.Cube],
                    state.Solutions.Count, 
                    seenLayers[state.Cube.Up], 
                    seenLayers[state.Cube.Down],
                    seenHalfs[state.Cube.Up.Left],
                    seenHalfs[state.Cube.Up.Right],
                    seenHalfs[state.Cube.Down.Left],
                    seenHalfs[state.Cube.Down.Right]
                    );
                state = fromState;
            } while (state != null);
        }
   }
}