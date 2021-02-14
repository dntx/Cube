using System.Collections.Generic;

namespace sq1code
{
    class Rotation {
        public Division Up { get; }
        public Division Down { get; }

        public Rotation(Division up, Division down) {
            Up = up;
            Down = down;
        }

        public bool IsIdentical() {
            return (Up.Left == Down.Left) || (Up.Right == Down.Right);
        }

        public bool IsShapeIdentical() {
            return Up.Left.GetShape() == Down.Left.GetShape() || Up.Right.GetShape() == Down.Right.GetShape();
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }
    }

}