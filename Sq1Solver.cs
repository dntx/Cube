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
            state.Froms.ForEach(from => VisitSolution(from, cubeSolution));
        }

        public void Run() {
            Console.WriteLine("hello");
            Cube startCube = Cube.Square;
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
                            Debug.Assert(false);
                        } else if (nextDepth == existingState.Depth) {
                            // an alternative path may found, update if necessary
                            if (!existingState.Froms.Contains(state)) {
                                existingState.Froms.Add(state);
                                netEdgeCount++;
                            }
                        }
                    } else {    
                        // new cube
                        State nextState = new State(nextCube, nextCubeId, state);
                        netEdgeCount++;
                        openStates.Enqueue(nextState);
                        seenStates.Add(nextState);
                    }
                }
            } while (openStates.Count > 0); 

            seenStates.ForEach(state => {
                if (state.Cube.IsHexagram()) {
                    VisitSolution(state, state.Cube);
                }
            });

            seenStates.ForEach(state => state.CalculateBestFrom());

            seenStates.ForEach(state => {
                if (state.Cube.IsHexagram()) {
                    OutputState(state);
                    Console.WriteLine();
                }
            });

            Console.WriteLine("cubes: {0}, total edges: {1}, net edges: {2}", seenCubes.Count, totalEdgeCount, netEdgeCount);
            Console.WriteLine("end");
        }

        private void OutputState(State state) {
            Console.WriteLine("cube: {0}", state.Cube);
            Console.WriteLine("depth：{0}", state.Depth);
            do {
                Console.WriteLine(
                    " ==> {0} | {1}({2}) | {3},{4} | {5}-{6},{7}-{8}", 
                    state.Cube.ToString(verbose: true),
                    seenCubes[state.Cube],
                    state.Solutions.Count, 
                    seenLayers[state.Cube.Up], 
                    seenLayers[state.Cube.Down],
                    seenHalfs[state.Cube.Up.Left],
                    seenHalfs[state.Cube.Up.Right],
                    seenHalfs[state.Cube.Down.Left],
                    seenHalfs[state.Cube.Down.Right]
                    );
                state = state.BestFrom;
            } while (state != null);
        }
   }
}