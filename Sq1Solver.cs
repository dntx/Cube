using System;
using System.Collections.Generic;

namespace sq1code
{
    class Sq1Solver {
        public static void Run() {
            Console.WriteLine("hello");

            Queue<State> openStates = new Queue<State>();
            ISet<Cube> seenCubes = new HashSet<Cube>();

            int id = 0;
            Cube startCube = Cube.Square;
            openStates.Enqueue(new State(id, startCube));
            seenCubes.Add(startCube);
            do {
                State state = openStates.Dequeue();
                Cube cube = state.Cube;
                if (cube.IsHexagram()) {
                    outputState(state);
                    Console.WriteLine();
                }

                List<Rotation> rotations = cube.GetRotations();
                foreach (Rotation rotation in rotations) {
                    Cube nextCube = cube.ApplyRotation(rotation);
                    if (!seenCubes.Contains(nextCube)) {
                        id++;
                        State nextState = new State(id, state, nextCube);
                        openStates.Enqueue(nextState);
                        seenCubes.Add(nextCube);
                    }
                }
            } while (openStates.Count > 0); 

            Console.WriteLine("total: {0}", seenCubes.Count);
            Console.WriteLine("end");
        }

        private static void outputState(State state) {
            Console.WriteLine("cube: {0}", state.Cube);

            Console.Write("depth：{0}", state.Depth);
            do {
                Console.Write(" ==> {0}({1})", state.Cube.ToString(withFromInfo: true), state.Id);
                state = state.From;
            } while (state != null);
            Console.WriteLine();
        }
    }
}