namespace Cube.Sq1Cube
{
    class Division {
        public Cells Left { get; }
        public Cells Right { get; }

        public Division(Cells left, Cells right) {
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
            return Left.GetHashCode() * 16^6 + Right.GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0,4}-{1,-4}", Left, Right);
        }
    }

}