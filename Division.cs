using System.Collections.Generic;

namespace sq1code
{
    class Division {
        public Half left { get; }
        public Half right { get; }

        private Division(Half left, Half right) {
            this.left = left;
            this.right = right;
        }

        public Division(List<int> leftCells, List<int> rightCells) {
            this.left = new Half(leftCells);
            this.right = new Half(rightCells);
        }

        public static bool operator == (Division lhs, Division rhs) {
            return (lhs.left == rhs.left) && (lhs.right == rhs.right);
        }

        public static bool operator != (Division lhs, Division rhs) {
            return !(lhs == rhs);
        }

        public static Division operator - (Division me) {
            return new Division(me.right, me.left);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Division);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            int code = 0;
            foreach (int cell in left.cells) {
                code = code * 10 + cell;
            }
            code *= 10;

            foreach (int cell in right.cells) {
                code = code * 10 + cell;
            }
            return code;
        }

        public override string ToString() {
            return left.ToString() + "-" + right.ToString();
        }
    }

}