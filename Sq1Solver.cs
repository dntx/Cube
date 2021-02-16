using System;
using System.Collections.Generic;

namespace sq1code
{
    class Sq1Solver {
        public enum Goal { SolveUpDownShape, SolveUpDownColor, SolveL3P75, Solve6030Position};

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
            Console.WriteLine("start");
            switch (goal)
            {
                case Goal.SolveUpDownShape:
                    SolveSq1Cube(
                        Cube.UpDownShapeSolvedCube, 
                        cube => cube.IsUpOrDownHexagram());
                    break;

                case Goal.SolveUpDownColor:
                    SolveSq1Cube(
                        Cube.UpDownColorSolvedCube, 
                        cube => cube.IsUpDwonColorGrouped(), 
                        rotation => rotation.IsShapeIdentical());
                    break;

                case Goal.SolveL3P75:
                    SolveSq1Cube(
                        Cube.L3P75SolvedCube, 
                        cube => cube.IsL3P625Solved(), 
                        rotation => rotation.IsShapeIdentical());
                    break;

                case Goal.Solve6030Position:
                    SolveSq1Cube(
                        Cube.L1L3SolvedCube, 
                        cube => cube.IsL1Solved(), 
                        rotation => rotation.Is6030Locked());
                    break;
            }
            Console.WriteLine("end");
        }

        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube) {
            SolveSq1Cube(startCube, IsTargetCube, maxDepth: int.MaxValue);
        }

        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, int maxDepth) {
            SolveSq1Cube(startCube, IsTargetCube, IsFocusRotation: rotation => true, maxDepth);
        }
        
        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, Predicate<Rotation> IsFocusRotation) {
            SolveSq1Cube(startCube, IsTargetCube, IsFocusRotation, maxDepth: int.MaxValue);
        }
        
        private void SolveSq1Cube(Cube startCube, Predicate<Cube> IsTargetCube, Predicate<Rotation> IsFocusRotation, int maxDepth) {
            VisitCube(startCube);

            int closedStateCount = 0;
            int ignoreStateCount = 0;
            int solutionCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;
            Queue<State> openStates = new Queue<State>();
            List<State> seenStates = new List<State>();

            State startState = new State(startCube, 0);
            openStates.Enqueue(startState);
            seenStates.Add(startState);
            do {
                State state = openStates.Dequeue();
                Cube cube = state.Cube;
                if (state.Depth > 0 && IsTargetCube(state.Cube)) {
                    solutionCount++;
                }

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
                        if (nextState.Depth <= maxDepth) {
                            openStates.Enqueue(nextState);
                        } else {
                            ignoreStateCount++;
                        }
                        seenStates.Add(nextState);
                    }
                }
                closedStateCount++;
                if (closedStateCount == 1 || closedStateCount % 1000 == 0 || openStates.Count == 0) {
                    int totalCount = closedStateCount + openStates.Count + ignoreStateCount;
                    Console.WriteLine(
                        "depth: {0}, solution: {1}, closed: {2}({3:p}), open: {4}({5:p}), ignored: {6}({7:p})", 
                        state.Depth, 
                        solutionCount,
                        closedStateCount, 
                        (float)closedStateCount / totalCount,
                        openStates.Count, 
                        (float)openStates.Count / totalCount,
                        ignoreStateCount,
                        (float)ignoreStateCount / totalCount
                        );
                }
            } while (openStates.Count > 0);
            Console.WriteLine(); 

            seenStates.ForEach(state => {
                if (state.Depth > 0 && IsTargetCube(state.Cube)) {
                    VisitSolution(state, state.Cube);
                }
            });

            seenStates.ForEach(state => state.CalculateBestFrom());

            seenStates.ForEach(state => {
                if (state.Depth > 0 && IsTargetCube(state.Cube)) {
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