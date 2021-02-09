using System.Collections.Generic;

namespace sq1code
{
    class Division {
        public Half left { get; }
        public Half right { get; }

        public Division(Half left, Half right) : this (left, right, false) {
        }

        public Division(Half first, Half second, bool normalize) {
            if (!normalize || first <= second) {
                this.left = first;
                this.right = second;
            } else {
                this.left = second;
                this.right = first;
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
            return left.GetHashCode() * Half.MaxHashCodeUpperBound + right.GetHashCode();
        }

        public override string ToString() {
            return left.ToString() + "-" + right.ToString();
        }
    }

}