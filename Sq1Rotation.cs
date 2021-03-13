using System.Collections.Generic;

namespace sq1code
{
    class Sq1Rotation : IRotation {
        public Division Up { get; }
        public Division Down { get; }

        public Sq1Rotation(Division up, Division down) {
            Up = up;
            Down = down;
        }

        public bool IsIdentical() {
            return (Up.Left == Down.Left) || (Up.Right == Down.Right);
        }

        public bool IsSquareShapeLocked() {
            return Up.Left[0].Shape == Down.Left[0].Shape;
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }
    }

}