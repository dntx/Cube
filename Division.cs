namespace sq1code
{
    class Division {
        public Half Left { get; }
        public Half Right { get; }

        public Division(Half left, Half right) {
            Left = left;
            Right = right;
        }

        public static bool operator == (Division lhs, Division rhs) {
            return (lhs.Left == rhs.Left) && (lhs.Right == rhs.Right);
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
            return Left.GetHashCode() * Half.HashCodeUpperBound + Right.GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0,4}-{1,-4}", Left, Right);
        }
    }

}