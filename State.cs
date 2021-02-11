using System.Collections.Generic;

namespace sq1code {
    class State {
        public int Depth { get; }
        public List<State> Froms { get; }
        public State BestFrom { get; private set; }
        public Cube Cube { get; }
        public int CubeId { get; }
        public ISet<Cube> Solutions { get; set; }

        public State(Cube cube, int cubeId) : this(cube, cubeId, null) {
        }

        public State(Cube cube, int cubeId, State from) {
            Depth = 0;
            Froms = new List<State>();
            BestFrom = null;
            Cube = cube;
            CubeId = cubeId;
            Solutions = new HashSet<Cube>();

            if (from != null) {
                Depth = from.Depth + 1;
                Froms.Add(from);
            }
        }

        public void CalculateBestFrom() {
            if (Froms.Count == 0) {
                return;
            }

            BestFrom = Froms[0];
            Froms.ForEach(from => BestFrom = GetBetterFrom(from, BestFrom));
        }

        private State GetBetterFrom(State from1, State from2) {
            if (from1 == from2) {
                return from1;
            }

            State state1 = from1;
            State state2 = from2;
            do {
                if (state1.Solutions.Count > state2.Solutions.Count) {
                    return from1;
                }
                if (state1.Solutions.Count < state2.Solutions.Count) {
                    return from2;
                }
                state1 = state1.BestFrom;
                state2 = state2.BestFrom;
            } while (state1 != null && state2 != null && state1 != state2);

            return (from1.CubeId < from2.CubeId)? from1 : from2;
        }
    }
}