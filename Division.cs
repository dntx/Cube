using System.Collections.Generic;

namespace sq1code
{
    class Division {
        public Half left { get; private set; }
        public Half right { get; private set; }

        public Division(Half left, Half right) : this (left, right, false) {
        }

        // todo: normalze should be called as ascending
        public Division(Half first, Half second, bool needNormalize) {
            this.left = first;
            this.right = second;
            if (needNormalize) {
                Normalize();
            }
        }

        private void Normalize() {
            if (left > right) {
                Half temp = left;
                left = right;
                right = temp;
            }
        }

        public static bool operator == (Division lhs, Division rhs) {
            return (lhs.left == rhs.left) && (lhs.right == rhs.right);
        }

        public static bool operator != (Division lhs, Division rhs) {
            return !(lhs == rhs);
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
            return left.GetHashCode() * Half.HashCodeUpperBound + right.GetHashCode();
        }

        public override string ToString() {
            return left.ToString() + "-" + right.ToString();
        }
    }

}