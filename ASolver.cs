using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code
{
    class ASolver {
        public enum Mode {
            ASearch,
            ReverseSearch,
            BiDiSearch
        }

        private Mode mode;

        public ASolver(Mode mode) {
            this.mode = mode;
        }

        public bool Solve(Cube startCube, Cube targetCube) {
            bool lockSquareShape = startCube.IsShapeSolved() && targetCube.IsShapeSolved();
            if (mode == Mode.ReverseSearch) {
                return DoSolve(targetCube, new HashSet<Cube>{startCube}, lockSquareShape);
            } else {
                return DoSolve(startCube, new HashSet<Cube>{targetCube}, lockSquareShape);
            }
        }

        public bool Solve(ICollection<Cube> startCubes, Cube targetCube) {
            if (mode == Mode.ReverseSearch) {
                bool lockSquareShape = startCubes.All(cube => cube.IsShapeSolved()) && targetCube.IsShapeSolved();
                return DoSolve(targetCube, startCubes, lockSquareShape);
            } else {
                DateTime startTime = DateTime.Now;
                Console.WriteLine("total request for \"{0}\": {1}", targetCube, startCubes.Count);

                int solvedCount = 0;
                int searchedCount = 0;
                foreach (Cube startCube in startCubes) {
                    searchedCount++;
                    Console.WriteLine("searching solution for \"{0}\": {1}/{2} ...", targetCube, searchedCount, startCubes.Count);
                    bool lockSquareShape = startCube.IsShapeSolved() && targetCube.IsShapeSolved();
                    bool successful = DoSolve(startCube, new HashSet<Cube>{targetCube}, lockSquareShape);
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

        private bool DoSolve(Cube startCube, ICollection<Cube> targetCubes, bool lockSquareShape) {
            DateTime startTime = DateTime.Now;

            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;
            int solutionCount = 0;

            // solution for one-way search
            Dictionary<Cube, AState> targetStates = new Dictionary<Cube, AState>();

            // solution for BiDiSearch
            AState targetState = null;
            AState midStateFromStart = null;
            AState midStateFromTarget = null;

            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<Cube, AState> seenCubeStates = new Dictionary<Cube, AState>();
            AState startState = new AState(startCube, seenCubeStates.Count);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);

            if (mode == Mode.BiDiSearch) {
                if (targetCubes.Count != 1) {
                    throw new Exception("target cube should be only one for BidiSearch mode.");
                }
                Cube targetCube = targetCubes.First();
                targetState = new AState(targetCube, seenCubeStates.Count);
                openStates.Add(targetState);
                seenCubeStates.Add(targetCube, targetState);
            }

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
                        if (state.StartCube == existingState.StartCube) {
                            // same way
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
                            // meet from two different ways !!!
                            int nextCubeId = existingState.CubeId;
                            int predictedCost = existingState.Depth;
                            AState nextState = new AState(state.StartCube, nextCube, nextCubeId, predictedCost, state, rotation);
                            netEdgeCount++;

                            if (nextState.StartCube == startCube) {
                                midStateFromStart = nextState;
                                midStateFromTarget = existingState;
                            } else {
                                midStateFromStart = existingState;
                                midStateFromTarget = nextState;
                            }
                            solutionCount++;
                            completed = true;
                            break;
                        }
                    } else {
                        // new cube
                        int predictedCost = (state.StartCube == startCube)? 
                            APredictor.PredictCost(nextCube, targetCubes) :
                            APredictor.PredictCost(nextCube, startCube);
                        int nextCubeId = seenCubeStates.Count;
                        AState nextState = new AState(state.StartCube, nextCube, nextCubeId, predictedCost, state, rotation);
                        netEdgeCount++;

                        openStates.Add(nextState);
                        seenCubeStates.Add(nextCube, nextState);

                        if (targetCubes.Contains(nextCube)) {
                            targetStates.Add(nextCube, nextState);
                            solutionCount++;
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
                        "sec: {0:0.00}, {1}{2}, g: {3}, h: {4}, solved: {5}, closed: {6}({7:p}), open: {8}({9:p}), reopened: {10}({11:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Cube,
                        (state.StartCube == startCube)? " -> " : " <- ",
                        state.Depth,
                        state.PredictedCost, 
                        solutionCount,
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

            switch (mode) {
                case Mode.ASearch:
                    targetStates.Values.ToList().ForEach(targetState => OutputSolution(startState, targetState));
                    break;
                case Mode.ReverseSearch:
                    targetStates.Values.ToList().ForEach(targetState => OutputReverseSolution(startState, targetState));
                    break;
                case Mode.BiDiSearch:
                    OutputSolution(startState, midStateFromStart);
                    OutputReverseSolution(targetState, midStateFromTarget);
                    break;
            }
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, solutions: {1}, closed: {2}", seenCubeStates.Count, targetStates.Count, closedStateCount);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private void OutputReverseSolution(AState startState, AState targetState) {
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
            Console.WriteLine();
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
            Console.WriteLine();
         }
   }
}