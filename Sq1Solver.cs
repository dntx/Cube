using System;
using System.Collections.Generic;

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

        private bool VisitCube(Cube cube) {
            if (seenCubes.ContainsKey(cube)) {
                return false;
            }
            
            seenCubes.Add(cube, seenCubes.Count);
            VisitLayer(cube.Up);
            VisitLayer(cube.Down);
            return true;
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

        public void Run() {
            Console.WriteLine("hello");
            Cube startCube = Cube.Square;
            VisitCube(startCube);

            Queue<State> openStates = new Queue<State>();
            openStates.Enqueue(new State(startCube));
            do {
                State state = openStates.Dequeue();
                Cube cube = state.Cube;
                if (cube.IsHexagram()) {
                    OutputState(state);
                    Console.WriteLine();
                }

                List<Rotation> rotations = cube.GetRotations();
                foreach (Rotation rotation in rotations) {
                    Cube nextCube = cube.ApplyRotation(rotation);
                    bool isNew = VisitCube(nextCube);
                    if (isNew) {
                        State nextState = new State(nextCube, state);
                        openStates.Enqueue(nextState);
                    }
                }
            } while (openStates.Count > 0); 

            Console.WriteLine("total: {0}", seenCubes.Count);
            Console.WriteLine("end");
        }

        private void OutputState(State state) {
            Console.WriteLine("cube: {0}", state.Cube);
            Console.WriteLine("depth：{0}", state.Depth);
            do {
                Console.WriteLine(
                    " ==> {0} | {1} | {2},{3} | {4}-{5},{6}-{7}", 
                    state.Cube.ToString(verbose: true), 
                    seenCubes[state.Cube],
                    seenLayers[state.Cube.Up], 
                    seenLayers[state.Cube.Down],
                    seenHalfs[state.Cube.Up.Left],
                    seenHalfs[state.Cube.Up.Right],
                    seenHalfs[state.Cube.Down.Left],
                    seenHalfs[state.Cube.Down.Right]
                    );
                state = state.From;
            } while (state != null);
        }
   }
}