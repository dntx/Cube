using System.Collections.Generic;

namespace sq1code {
    class State {
        public int Id { get; }
        public int Depth { get; }
        public State From { get; }
        public Cube Cube { get; }

        public State(int id, Cube cube) {
            Id = id;
            Depth = 0;
            From = null;
            Cube = cube;
        }

        public State(int id, State from, Cube cube) {
            Id = id;
            Depth = from.Depth + 1;
            From = from;
            Cube = cube;
        }
    }
}