using System;
using System.Collections.Generic;

namespace sq1code
{
    class Sq1Solver {
        public static void Run() {
            Console.WriteLine("hello");

            Queue<State> openQueue = new Queue<State>();
            openQueue.Enqueue(new State(Cube.Square));
            Queue<State> closedQueue = new Queue<State>();
            do {
                State current = openQueue.Dequeue();
                closedQueue.Enqueue(current);
                if (current.cube.isHexagram()) {
                    Console.WriteLine(current.ToString());
                }

                Cube cube = current.cube;
                List<Rotation> rotations = cube.GetRotations();
                foreach (Rotation rotation in rotations) {
                    //Console.WriteLine("rotation: " + rotation.ToString());
                    Cube nextCube = cube.ApplyRotation(rotation);
                    if (isNewCube(openQueue, nextCube) && isNewCube(closedQueue, nextCube)) {
                        //Console.WriteLine("new cube: {0}", nextCube.ToString());
                        State nextState = new State(current, rotation, nextCube);
                        openQueue.Enqueue(nextState);
                    }
                }
                //Console.WriteLine();
                //Console.WriteLine("open: {0}, closed: {1}", openQueue.Count, closedQueue.Count);
            } while (openQueue.Count > 0); 

            Console.WriteLine("end");
        }

        private static bool isNewCube(Queue<State> stateQueue, Cube cube) {
            foreach (State state in stateQueue) {
                if (state.cube == cube) {
                    return false;
                }
            }
            return true;
        }
    }
}