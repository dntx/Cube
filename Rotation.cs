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

        public bool isIdenticalRotation() {
            return (Up.Left == Down.Left) || (Up.Right == Down.Right);
        }

        public override string ToString() {
            return Up.ToString() + "," + Down.ToString();
        }
    }

}