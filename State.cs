using System.Collections.Generic;

namespace sq1code {
    class State {
        public int id { get; }
        public int depth { get; }
        public State fromState { get; }
        public Rotation fromRotation { get; }
        public Cube cube { get; }

        public State(int id, Cube cube) {
            this.id = id;
            this.depth = 0;
            this.fromState = null;
            this.fromRotation = null;
            this.cube = cube;
        }

        public State(int id, State fromState, Rotation fromRotation, Cube cube) {
            this.id = id;
            this.depth = fromState.depth + 1;
            this.fromState = fromState;
            this.fromRotation = fromRotation;
            this.cube = cube;
        }
    }
}