using System.Collections.Generic;

namespace sq1code {
    class State {
        public int depth { get; }
        public State fromState { get; }
        public Rotation fromRotation { get; }
        public Cube cube { get; }

        public State(Cube cube) {
            this.depth = 0;
            this.fromState = null;
            this.fromRotation = null;
            this.cube = cube;
        }

        public State(State fromState, Rotation fromRotation, Cube cube) {
            this.depth = fromState.depth + 1;
            this.fromState = fromState;
            this.fromRotation = fromRotation;
            this.cube = cube;
        }

        public override string ToString() {
            string s = "start: ";
 
            List<Rotation> rotations = cube.GetRotations();
            foreach (Rotation rotation in rotations) {
                s += rotation.ToString();
                s += " | ";
            }
            s += cube.ToString();

            State from = fromState;
            while (from != null) {
                s += " --> ";
                s += from.cube.ToString();
                from = from.fromState;
            }
            return s;
        }
    }
}