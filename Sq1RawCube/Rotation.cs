namespace Cube.Sq1RawCube
{
    class Rotation : IRotation {
        public Division Up { get; }
        public Division Down { get; }

        public Rotation(Division up, Division down) {
            Up = up;
            Down = down;
        }

        public override string ToString() {
            return string.Format("{0,9},{1,-9}", Up, Down);
        }

        public IRotation GetReversedRotation() {
            Division reversedUp = new Division(Up.Left, Down.Right);
            Division reversedDown = new Division(Down.Left, Up.Right);
            return new Rotation(reversedUp, reversedDown);
        }
    }

}