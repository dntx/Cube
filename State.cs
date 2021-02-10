namespace sq1code {
    class State {
        public int Depth { get; }
        public State From { get; }
        public Cube Cube { get; }

        public State(Cube cube) : this(cube, null) {
        }

        public State(Cube cube, State from) {
            Depth = (from == null)? 0 : from.Depth + 1;
            From = from;
            Cube = cube;
        }
    }
}