using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace sq1code
{
    class Sq1Solver {
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
            if (seenLayers.ContainsKey(layer)) {
                return false;
            }

            seenLayers.Add(layer, seenLayers.Count);
            VisitHalf(layer.Left);
            VisitHalf(layer.Right);
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

        public delegate bool IsTargetFunc(Cube cube);

        private bool IsSquareCube(Cube cube, int maxColorDiff) {
            if (!cube.IsSquare()) {
                return false;
            }

            return cube.Up.GetColorDiff() <= maxColorDiff;
        }

        public void Run() {
            Console.WriteLine("hello");
            SolveSq1Cube(Cube.UnicolorCube, cube => cube.IsHexagram(), 100);
            //SolveSq1Cube(Cube.BicolorCube, cube => IsSquareCube(cube, 1), 5);
            Console.WriteLine("end");
        }

        private void SolveSq1Cube(Cube startCube, IsTargetFunc IsTarget, int maxDepth) {
            VisitCube(startCube);

            int totalEdgeCount = 0;
            int netEdgeCount = 0;
            Queue<State> openStates = new Queue<State>();
            List<State> seenStates = new List<State>();

            State startState = new State(startCube, 0);
            openStates.Enqueue(startState);
            seenStates.Add(startState);
            do {
                State state = openStates.Dequeue();
                if (state.Depth > maxDepth) {
                    break;
                }
                Cube cube = state.Cube;

                List<Rotation> rotations = cube.GetRotations();
                int nextDepth = state.Depth + 1;
                foreach (Rotation rotation in rotations) {
                    Cube nextCube = cube.ApplyRotation(rotation);
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
                        seenStates.Add(nextState);
                    }
                }
            } while (openStates.Count > 0); 

            seenStates.ForEach(state => {
                if (state.Depth <= maxDepth && IsTarget(state.Cube)) {
                    VisitSolution(state, state.Cube);
                }
            });

            seenStates.ForEach(state => state.CalculateBestFrom());

            seenStates.ForEach(state => {
                if (state.Depth <= maxDepth && IsTarget(state.Cube)) {
                    OutputState(state);
                    Console.WriteLine();
                }
            });

            Console.WriteLine("cubes: {0}, total edges: {1}, net edges: {2}", seenCubes.Count, totalEdgeCount, netEdgeCount);
        }

        private void OutputState(State state) {
            Console.WriteLine("cube: {0}", state.Cube);
            Console.WriteLine("depth：{0}", state.Depth);
            do {
                State fromState = state.BestFrom.Key;
                Rotation fromRotation = state.BestFrom.Value;
                Cube rotatedCube = (fromState != null)? fromState.Cube.ApplyRotation(fromRotation) : state.Cube;

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