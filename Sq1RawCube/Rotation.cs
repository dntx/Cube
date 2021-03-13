using System.Collections.Generic;

namespace Cube.Sq1RawCube
{
    class Rotation : IRotation {
        public Division Up { get; }
        public Division Down { get; }

        public Rotation(Division up, Division down) {
            Up = up;
            Down = down;
        }

        public bool IsIdentical() {
            return (Up.Left == Down.Left) || (Up.Right == Down.Right);
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }
    }

}