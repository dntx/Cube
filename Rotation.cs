using System.Collections.Generic;

namespace sq1code
{
    class Rotation {
        public Division up { get; }
        public Division down { get; }

        public Rotation(Division up, Division down) {
            this.up = up;
            this.down = down;
        }

        public bool isIdenticalRotation() {
            return (up.left == down.left) || (up.right == down.right);
        }

        public override string ToString() {
            return up.ToString() + "," + down.ToString();
        }
    }

}