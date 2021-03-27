namespace Cube.Sq1List16Cube
{
    class Rotation : IRotation {
        public Division Up { get; }
        public Division Down { get; }

        public Rotation(Division up, Division down) {
            Up = up;
            Down = down;
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }

        public string ToString(ICube baseCube) {
            return ToString();
        }

        public IRotation GetInverseRotation() {
            Division rotatedUp = new Division(Up.Left, Down.Right);
            Division rotatedDown = new Division(Down.Left, Up.Right);
            return new Rotation(rotatedUp, rotatedDown);
        }
    }
}