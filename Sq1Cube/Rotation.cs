using System.Collections.Generic;
using Cube;

namespace Cube.Sq1Cube
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

        public bool IsSquareShapeLocked() {
            return Up.Left[0].Shape == Down.Left[0].Shape;
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }
    }

}