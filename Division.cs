using System.Collections.Generic;

namespace sq1code
{
    class Division {
        public Half left { get; }
        public Half right { get; }

        public Division(List<int> leftCells, List<int> rightCells) {
            this.left = new Half(leftCells);
            this.right = new Half(rightCells);
        }

        public bool SameAs(Division other) {
            return (left == other.left) && (right == other.right);
        }

        public bool UTurnAs(Division other) {
            return (left == other.right) && (right == other.left);
        }

        public override string ToString() {
            return left.ToString() + "-" + right.ToString();
        }
    }

}