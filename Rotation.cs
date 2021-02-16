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

        public bool Is3060PairLocked() {
            if (Up.Right[Up.Right.Count - 1].Degree == 30 && Up.Left[0].Degree == 60) {
                return false;
            }
            if (Up.Left[Up.Left.Count - 1].Degree == 30 && Up.Right[0].Degree == 60) {
                return false;
            }

            if (Down.Right[Down.Right.Count - 1].Degree == 30 && Down.Left[0].Degree == 60) {
                return false;
            }
            if (Down.Left[Down.Left.Count - 1].Degree == 30 && Down.Right[0].Degree == 60) {
                return false;
            }
            
            return true;
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }
    }

}