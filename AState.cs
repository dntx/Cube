using System.Collections.Generic;

namespace sq1code {
    class AState {
        public int Depth { get; }
        public List<KeyValuePair<AState, Rotation>> Froms { get; }
        public KeyValuePair<AState, Rotation> BestFrom { get; private set; }
        public Cube Cube { get; }
        public int CubeId { get; }
        public ISet<Cube> Solutions { get; set; }

        public AState(Cube cube, int cubeId) : this(cube, cubeId, null, null) {
        }

        public AState(Cube cube, int cubeId, AState fromState, Rotation fromRotation) {
            Depth = 0;
            Froms = new List<KeyValuePair<AState, Rotation>>();
            BestFrom = new KeyValuePair<AState, Rotation>(null, null);
            Cube = cube;
            CubeId = cubeId;
            Solutions = new HashSet<Cube>();

            if (fromState != null) {
                Depth = fromState.Depth + 1;
                AddFrom(fromState, fromRotation);
            }
        }

        public void AddFrom(AState fromState, Rotation fromRotation) {
            Froms.Add(new KeyValuePair<AState, Rotation>(fromState, fromRotation));
        }

        public void CalculateBestFrom() {
            if (Froms.Count == 0) {
                return;
            }

            BestFrom = Froms[0];
            for (int i = 1; i < Froms.Count; i++) {
                BestFrom = GetBetterFrom(Froms[i], BestFrom);
            }
            Froms.ForEach(from => BestFrom = GetBetterFrom(from, BestFrom));
        }

        private KeyValuePair<AState, Rotation> GetBetterFrom(KeyValuePair<AState, Rotation> from1, KeyValuePair<AState, Rotation> from2) {
            AState state1 = from1.Key;
            AState state2 = from2.Key;
            do {
                if (state1.Solutions.Count > state2.Solutions.Count) {
                    return from1;
                }
                if (state1.Solutions.Count < state2.Solutions.Count) {
                    return from2;
                }
                state1 = state1.BestFrom.Key;
                state2 = state2.BestFrom.Key;
            } while (state1 != null && state2 != null && state1 != state2);

            return (from1.Key.CubeId < from2.Key.CubeId)? from1 : from2;
        }
    }
}