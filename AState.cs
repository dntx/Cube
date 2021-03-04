using System.Collections.Generic;

namespace sq1code {
    class AState {
        public int Depth { get; }
        public AState FromState { get; private set; }
        public Rotation FromRotation { get; private set; }
        public Cube Cube { get; }
        public int CubeId { get; }

        public AState(Cube cube, int cubeId) : this(cube, cubeId, null, null) {
        }

        public AState(Cube cube, int cubeId, AState fromState, Rotation fromRotation) {
            Depth = (fromState != null) ? fromState.Depth + 1 : 0;
            FromState = fromState;
            FromRotation = fromRotation;
            Cube = cube;
            CubeId = cubeId;
        }

        public void UpdateFrom(AState fromState, Rotation fromRotation) {
            FromState = fromState;
            FromRotation = fromRotation;
        }
    }
}