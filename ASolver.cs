using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube
{
    class ASolver {
        public enum Mode {
            BfSearch,
            ASearch,
            BackwardBfSearch,
            BidiBfSearch,
            BidiASearch,
            PermuteBfSearch,
            PermuteASearch
        }

        private Mode mode;
        private int maxStateCount;

        public ASolver(Mode mode) : this(mode, int.MaxValue) {
        }

        public ASolver(Mode mode, int maxStateCount) {
            this.mode = mode;
            this.maxStateCount = maxStateCount;
        }

        public delegate IPredictor CreatePredictor(ICube targetCube);

        public bool Solve(ICube startCube, ICube targetCube, CreatePredictor createPredictor) {
            switch (mode) {
                case Mode.BackwardBfSearch:
                    return BackwardBfSearch(targetCube, new HashSet<ICube>{startCube});
                case Mode.BfSearch:
                case Mode.ASearch:
                    return ForwardSearch(startCube, targetCube, createPredictor, (mode == Mode.BfSearch));
                case Mode.PermuteBfSearch:
                case Mode.PermuteASearch:
                    return PermuteSearch(startCube, targetCube, createPredictor, (mode == Mode.PermuteBfSearch));
                case Mode.BidiBfSearch:
                case Mode.BidiASearch:
                    return BidiSearch(startCube, targetCube, createPredictor, (mode == Mode.BidiBfSearch));
            }

            throw new Exception(string.Format("Mode {0} is not recognized", mode));
        }

        public bool Solve(ICollection<ICube> startCubes, ICube targetCube, CreatePredictor createPredictor) {
            if (mode == Mode.BackwardBfSearch) {
                return BackwardBfSearch(targetCube, startCubes);
            } else {
                DateTime startTime = DateTime.Now;
                Console.WriteLine("total request for \"{0}\": {1}", targetCube, startCubes.Count);

                int solvedCount = 0;
                int searchedCount = 0;
                foreach (ICube startCube in startCubes) {
                    searchedCount++;
                    Console.WriteLine("searching solution for \"{0}\": {1}/{2} ...", targetCube, searchedCount, startCubes.Count);
                    bool successful = Solve(startCube, targetCube, createPredictor);
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

        private bool BackwardBfSearch(ICube startCube, ICollection<ICube> targetCubes) {
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
                OutputSolutionBackward(targetState);
            });
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, solutions: {1}, closed: {2}, open: {3}", seenCubeStates.Count, targetStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private bool ForwardSearch(ICube startCube, ICube targetCube, CreatePredictor createPredictor, bool isBfSearch) {
            DateTime startTime = DateTime.Now;

            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;

            IPredictor predictor = createPredictor(targetCube);
            AState targetState = null;

            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<ICube, AState> seenCubeStates = new Dictionary<ICube, AState>();
            int forwardCost = isBfSearch? 0 : predictor.PredictCost(startCube);
            AState startState = new AState(startCube, seenCubeStates.Count, forwardCost);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);

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
                        int nextCubeId = seenCubeStates.Count;
                        int predictedCost = isBfSearch? 0 : predictor.PredictCost(nextCube);
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
            OutputSolution(targetState, outputTargetCube:true);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, closed: {1}, open: {2}", seenCubeStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private bool PermuteSearch(ICube startCube, ICube targetCube, CreatePredictor createPredictor, bool isBfSearch) {
            DateTime startTime = DateTime.Now;

            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;

            IPredictor predictor = createPredictor(targetCube);
            IPermutation permutation = predictor.CalcPermutation(startCube);
            IPermutation inversePermutation = permutation.GetInversePermutation();

            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<ICube, AState> seenCubeStates = new Dictionary<ICube, AState>();
            int forwardCost = isBfSearch? 0 : predictor.PredictCost(startCube);
            AState startState = new AState(startCube, seenCubeStates.Count, forwardCost);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);

            bool completed = false;
            bool needStop = false;
            AState midState = null;
            AState midPermutedState = null;
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
                        int nextCubeId = seenCubeStates.Count;
                        int predictedCost = isBfSearch? 0 : predictor.PredictCost(nextCube);
                        AState nextState = new AState(nextCube, nextCubeId, predictedCost, state, rotation);
                        netEdgeCount++;

                        openStates.Add(nextState);
                        seenCubeStates.Add(nextCube, nextState);

                        // check permutation
                        ICube nextPermutedCube = nextCube.PermuteBy(inversePermutation);
                        if (seenCubeStates.ContainsKey(nextPermutedCube)) {
                            // meet from perumuted way !!!
                            midState = nextState;
                            midPermutedState = seenCubeStates[nextPermutedCube];
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
            Console.WriteLine("solution depth：{0}", midState.Depth + midPermutedState.Depth);
            OutputSolution(midState, outputTargetCube:false);
            OutputSolutionBackward(midPermutedState, permutation);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, closed: {1}, open: {2}", seenCubeStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private bool BidiSearch(ICube startCube, ICube targetCube, CreatePredictor createPredictor, bool isBfSearch) {
            DateTime startTime = DateTime.Now;

            int startStateCount = 0;
            int targetStateCount = 0;
            int reopenStateCount = 0;
            int closedStateCount = 0;
            int totalEdgeCount = 0;
            int netEdgeCount = 0;

            // set start state
            IPredictor forwardPredictor = createPredictor(targetCube);
            SortedSet<AState> openStates = new SortedSet<AState>();
            Dictionary<ICube, AState> seenCubeStates = new Dictionary<ICube, AState>();
            int forwardCost = isBfSearch? 0 : forwardPredictor.PredictCost(startCube);
            AState startState = new AState(startCube, startCube, seenCubeStates.Count, forwardCost);
            openStates.Add(startState);
            seenCubeStates.Add(startCube, startState);
            startStateCount++;

            // set target state
            IPredictor backwardPredictor = createPredictor(startCube);
            int backwardCost = isBfSearch? 0 : backwardPredictor.PredictCost(startCube);
            AState targetState = new AState(targetCube, targetCube, seenCubeStates.Count, backwardCost);
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
                            int predictedCost = 0;
                            if (!isBfSearch) {
                                if (state.StartCube == startCube) {
                                    predictedCost = forwardPredictor.PredictCost(nextCube);
                                } else {
                                    predictedCost = backwardPredictor.PredictCost(nextCube);
                                }
                            }
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
                        int nextCubeId = seenCubeStates.Count;
                        int predictedCost = 0;
                        if (!isBfSearch) {
                            if (state.StartCube == startCube) {
                                predictedCost = forwardPredictor.PredictCost(nextCube);
                            } else {
                                predictedCost = backwardPredictor.PredictCost(nextCube);
                            }
                        }
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
            OutputSolution(midStateFromStart, outputTargetCube:false);
            OutputSolutionBackward(midStateFromTarget);
            Console.WriteLine();
            Console.WriteLine("cubes: {0}, closed: {1}, open: {2}", seenCubeStates.Count, closedStateCount, openStates.Count);
            Console.WriteLine("edges: {0}, net: {1}", totalEdgeCount, netEdgeCount);
            return true;
        }

        private void OutputSolution(AState targetState, bool outputTargetCube) {
            Stack<AState> solutionPath = new Stack<AState>();
            for (AState state = targetState; state.FromState != null; state = state.FromState) {
                solutionPath.Push(state);
            }

            while (solutionPath.Count > 0) {
                AState state = solutionPath.Pop();
                AState fromState = state.FromState;
                IRotation fromRotation = state.FromRotation;

                Console.WriteLine(
                    " ==> {0} -> | g:{1} | h:{2} | No.{3}", 
                    fromRotation,
                    fromState.Depth,
                    fromState.PredictedCost,
                    fromState.CubeId
                    );
            }
            if (outputTargetCube) {
                Console.WriteLine(
                    " ==> {0} -> | g:{1} | h:{2} | No.{3}", 
                    targetState.Cube,
                    targetState.Depth,
                    targetState.PredictedCost,
                    targetState.CubeId
                    );
                Console.WriteLine();
            }
        }

        private void OutputSolutionBackward(AState targetState) {
            AState state = targetState;
            do {
                AState fromState = state.FromState;
                IRotation fromRotation = state.FromRotation;

                // todo: consider change case 301-0101 to 0101-301
                Console.WriteLine(
                    " ==> {0} <- | g:{1} | h:{2} | No.{3}", 
                    (fromRotation == null)? state.Cube : fromRotation.GetInverseRotation(),
                    state.Depth,
                    state.PredictedCost,
                    state.CubeId
                    );
                state = fromState;
            } while (state != null);
            Console.WriteLine();
        }

        private void OutputSolutionBackward(AState targetState, IPermutation permutation) {
            AState state = targetState;
            do {
                AState fromState = state.FromState;
                IRotation fromRotation = state.FromRotation;

                // todo: consider up/down reverse situation if necessary
                Console.WriteLine(
                    " ==> {0} <- | g:{1} | h:{2} | No.{3}'", 
                    (fromRotation == null)? state.Cube.PermuteBy(permutation) : fromRotation.GetInverseRotation().PermuteBy(permutation),
                    state.Depth,
                    state.PredictedCost,
                    state.CubeId
                    );
                state = fromState;
            } while (state != null);
            Console.WriteLine();
        }
   }
}