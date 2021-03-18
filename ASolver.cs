using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube
{
    class ASolver {
        public enum Mode {
            ASearch,
            ReverseBfSearch,
            BidiBfSearch,
            BidiASearch
        }

        private Mode mode;
        private int maxStateCount;

        public ASolver(Mode mode) : this(mode, int.MaxValue) {
        }

        public ASolver(Mode mode, int maxStateCount) {
            this.mode = mode;
            this.maxStateCount = maxStateCount;
        }

        public bool Solve(ICube startCube, ICube targetCube) {
            switch (mode) {
                case Mode.ReverseBfSearch:
                    return ReverseBfSearch(targetCube, new HashSet<ICube>{startCube});
                case Mode.ASearch:
                    return ASearch(startCube, targetCube);
                case Mode.BidiBfSearch:
                case Mode.BidiASearch:
                    return BidiSearch(startCube, targetCube);
            }

            throw new Exception(string.Format("Mode {0} is not recognized", mode));
        }

        public bool Solve(ICollection<ICube> startCubes, ICube targetCube) {
            if (mode == Mode.ReverseBfSearch) {
                return ReverseBfSearch(targetCube, startCubes);
            } else {
                DateTime startTime = DateTime.Now;
                Console.WriteLine("total request for \"{0}\": {1}", targetCube, startCubes.Count);

                int solvedCount = 0;
                int searchedCount = 0;
                foreach (ICube startCube in startCubes) {
                    searchedCount++;
                    Console.WriteLine("searching solution for \"{0}\": {1}/{2} ...", targetCube, searchedCount, startCubes.Count);
                    bool successful = Solve(startCube, targetCube);
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

        private bool ReverseBfSearch(ICube startCube, ICollection<ICube> targetCubes) {
            DateTime startTime = DateTime.Now;

            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;
            int solutionCount = 0;

            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<ICube, AState> seenCubeStates = new Dictionary<ICube, AState>();
            AState startState = new AState(startCube, seenCubeStates.Count);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);
            Dictionary<ICube, AState> targetStates = new Dictionary<ICube, AState>();

            bool completed = false;
            bool needStop = false;
            do {
                AState state = openStates.Min;
                openStates.Remove(state);
                state.IsClosed = true;

                ICube cube = state.Cube;
                ICollection<IRotation> rotations = cube.GetRotations();

                int nextDepth = state.Depth + 1;
                foreach (IRotation rotation in rotations) {
                    ICube nextCube = cube.RotateBy(rotation);
                    totalEdgeCount++;
                    if (!seenCubeStates.ContainsKey(nextCube)) {
                        // new cube
                        int nextCubeId = seenCubeStates.Count;
                        AState nextState = new AState(nextCube, nextCubeId, predictedCost:0, state, rotation);
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
                needStop = completed || openStates.Count == 0 || (closedStateCount + openStates.Count) >= maxStateCount;
                if (needStop || closedStateCount % 1000 == 0) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "second: {0:0.00}, {1}, depth: {2}, solved: {3}, closed: {4}({5:p}), open: {6}({7:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Cube,
                        state.Depth,
                        solutionCount,
                        closedStateCount, 
                        (float)closedStateCount / totalCount,
                        openStates.Count,
                        (float)openStates.Count / totalCount
                        );
                }
            } while (!needStop);
            Console.WriteLine();

            if (!completed) {
                return false;
            }

            targetStates.Values.ToList().ForEach(targetState => {
                Console.WriteLine("start cube: {0}", targetState.Cube);
                Console.WriteLine("solution depth：{0}", targetState.Depth);
                OutputReversedSolution(startState, targetState);
            });
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, solutions: {1}, closed: {2}, open: {3}", seenCubeStates.Count, targetStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private bool ASearch(ICube startCube, ICube targetCube) {
            DateTime startTime = DateTime.Now;

            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;

            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<ICube, AState> seenCubeStates = new Dictionary<ICube, AState>();
            AState startState = new AState(startCube, seenCubeStates.Count);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);
            AState targetState = null;

            bool completed = false;
            bool needStop = false;
            do {
                AState state = openStates.Min;
                openStates.Remove(state);
                state.IsClosed = true;

                ICube cube = state.Cube;
                ICollection<IRotation> rotations = cube.GetRotations();

                int nextDepth = state.Depth + 1;
                foreach (IRotation rotation in rotations) {
                    ICube nextCube = cube.RotateBy(rotation);
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
                        int predictedCost = nextCube.PredictCost(targetCube);
                        int nextCubeId = seenCubeStates.Count;
                        AState nextState = new AState(nextCube, nextCubeId, predictedCost, state, rotation);
                        netEdgeCount++;

                        openStates.Add(nextState);
                        seenCubeStates.Add(nextCube, nextState);

                        if (nextCube.Equals(targetCube)) {
                            targetState = nextState;
                            completed = true;
                            break;
                        }
                    }
                }

                closedStateCount++;
                needStop = completed || openStates.Count == 0 || (closedStateCount + openStates.Count) >= maxStateCount;
                if (needStop || closedStateCount % 1000 == 0) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "sec: {0:0.00}, {1}, g: {2}, h: {3}, closed: {4}({5:p}), open: {6}({7:p}), reopened: {8}({9:p})", 
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
            } while (!needStop);
            Console.WriteLine();

            if (!completed) {
                return false;
            }

            Console.WriteLine("start cube: {0}", startState.Cube);
            Console.WriteLine("solution depth：{0}", targetState.Depth);
            OutputSolution(startState, targetState, outputTargetCube:true);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, closed: {1}, open: {2}", seenCubeStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private bool BidiSearch(ICube startCube, ICube targetCube) {
            DateTime startTime = DateTime.Now;

            int startStateCount = 0;
            int targetStateCount = 0;
            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;

            // set start state
            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<ICube, AState> seenCubeStates = new Dictionary<ICube, AState>();
            AState startState = new AState(startCube, startCube, seenCubeStates.Count);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);
            startStateCount++;

            // set target state
            AState targetState = new AState(targetCube, targetCube, seenCubeStates.Count);
            openStates.Add(targetState);
            seenCubeStates.Add(targetCube, targetState);
            targetStateCount++;

            // solution for Bidi Search
            AState midStateFromStart = null;
            AState midStateFromTarget = null;

            bool completed = false;
            bool needStop = false;
            do {
                AState state = openStates.Min;
                openStates.Remove(state);
                state.IsClosed = true;

                ICube cube = state.Cube;
                ICollection<IRotation> rotations = cube.GetRotations();

                int nextDepth = state.Depth + 1;
                foreach (IRotation rotation in rotations) {
                    ICube nextCube = cube.RotateBy(rotation);
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

                            // state is a new state, need add
                            // while cube is an old cube, need not add again
                            openStates.Add(nextState);
                            //seenCubeStates.Add(nextCube, nextState);

                            if (nextState.StartCube == startCube) {
                                startStateCount++;
                                midStateFromStart = nextState;
                                midStateFromTarget = existingState;
                            } else {
                                targetStateCount++;
                                midStateFromStart = existingState;
                                midStateFromTarget = nextState;
                            }
                            completed = true;
                            break;
                        }
                    } else {
                        // new cube
                        int predictedCost = (mode == Mode.BidiBfSearch)? 0 : nextCube.PredictCost(targetCube);
                        int nextCubeId = seenCubeStates.Count;
                        AState nextState = new AState(state.StartCube, nextCube, nextCubeId, predictedCost, state, rotation);
                        netEdgeCount++;

                        openStates.Add(nextState);
                        seenCubeStates.Add(nextCube, nextState);
                        if (nextState.StartCube == startCube) {
                            startStateCount++;
                        } else {
                            targetStateCount++;
                        }
                    }
                }

                closedStateCount++;
                needStop = completed || openStates.Count == 0 || (closedStateCount + openStates.Count) >= maxStateCount;
                if (needStop || closedStateCount % 1000 == 0) {
                    int totalCount = closedStateCount + openStates.Count;
                    Console.WriteLine(
                        "sec: {0:0.00}, {1}{2}({3:p}), g: {4}, h: {5}, closed: {6}({7:p}), open: {8}({9:p}), reopened: {10}({11:p})", 
                        DateTime.Now.Subtract(startTime).TotalSeconds,
                        state.Cube,
                        (state.StartCube == startCube)? " -> " : " <- ",
                        (state.StartCube == startCube)? (float)startStateCount / totalCount : (float)targetStateCount / totalCount,
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
            } while (!needStop);
            Console.WriteLine();

            if (!completed) {
                return false;
            }

            Console.WriteLine("start cube: {0}", startState.Cube);
            Console.WriteLine("solution depth：{0}", midStateFromStart.Depth + midStateFromTarget.Depth);
            OutputSolution(startState, midStateFromStart, outputTargetCube:false);
            OutputReversedSolution(targetState, midStateFromTarget);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, closed: {1}, open: {2}", seenCubeStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private void OutputSolution(AState startState, AState targetState, bool outputTargetCube) {
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
            if (outputTargetCube) {
                Console.WriteLine(
                    " ==> {0} | {1}", 
                    targetState.Cube,
                    targetState.CubeId
                    );
                Console.WriteLine();
            }
        }

        private void OutputReversedSolution(AState startState, AState targetState) {
            AState state = targetState;
            do {
                AState fromState = state.FromState;
                IRotation fromRotation = state.FromRotation;

                // todo: consider up/down reverse situation if necessary
                // todo: consider change case 301-0101 to 0101-301
                Console.WriteLine(
                    " ==> {0} | {1}", 
                    fromRotation.GetReversedRotation(),
                    state.CubeId
                    );
                state = fromState;
            } while (state.FromRotation != null);
            Console.WriteLine(
                " ==> {0} | {1}", 
                startState.Cube,
                startState.CubeId
                );
            Console.WriteLine();
        }
   }
}