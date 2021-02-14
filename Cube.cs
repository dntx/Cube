using System.Collections.Generic;

namespace sq1code
{
    class Cube {
        public Layer Up { get; }
        public Layer Down { get; }

        public Cube(Layer up, Layer down) {
            Up = up;
            Down = down;
        }

        public static bool operator == (Cube lhs, Cube rhs) {
            return (lhs.Up == rhs.Up && lhs.Down == rhs.Down) || (lhs.Up == rhs.Down && lhs.Down == rhs.Up);
        }

        public static bool operator != (Cube lhs, Cube rhs) {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Cube);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            int upHashCode = Up.GetHashCode();
            int downHashCode = Down.GetHashCode();
            if (upHashCode <= downHashCode) {
                return upHashCode * Layer.HashCodeUpperBound + downHashCode;
            } else {
                return downHashCode * Layer.HashCodeUpperBound + upHashCode;
            }
        }

        public bool IsUpOrDownHexagram() {
            return Up.IsHexagram() || Down.IsHexagram();
        }

        public bool IsUpDwonSquareColorAdjacent() {
            return Up.IsSquare() && Up.IsColorAdjacent() && Down.IsSquare() && Down.IsColorAdjacent();
        }

        public bool IsUpDownColorMatched() {
            return Up.IsSquare() && Up.IsSameColor() && Down.IsSquare() && Down.IsSameColor();
        }

        public List<Rotation> GetRotations() {
            List<Rotation> rotations = new List<Rotation>();

            ISet<Division> upDivisions = Up.GetDivisions(ascendingOnly: false);
            ISet<Division> downDivisions = Down.GetDivisions(ascendingOnly: false);

            foreach (Division upDivision in upDivisions) {
                foreach (Division downDivsion in downDivisions) {
                    Rotation rotation = new Rotation(upDivision, downDivsion);
                    if (!rotation.IsIdentical()) {
                        rotations.Add(rotation);
                    }
                }
            }

            return rotations;
        }

        public Cube ApplyRotation(Rotation rotation) {
            Layer up = new Layer(rotation.Up.Left, rotation.Down.Right);
            Layer down = new Layer(rotation.Down.Left, rotation.Up.Right);

            return new Cube(up, down);
        }

        public override string ToString()
        {
            return ToString(verbose: false);
        }

        public string ToString(bool verbose)
        {
            if (Up.ColorCount == 2) {
                return string.Format("{0},{1}({2})", Up.ToString(verbose), Down.ToString(verbose), Up.GetDiffColorCellCount());
            } else {
                return string.Format("{0},{1}", Up.ToString(verbose), Down.ToString(verbose));
            }
        }

        public static Cube UnicolorCube = new Cube(Layer.Square, Layer.Square);
        public static Cube BicolorCube = new Cube(Layer.WhiteSquare, Layer.YellowSquare);
        public static Cube FullColorCube = new Cube(Layer.WhiteSquareL1, Layer.YellowSquareL1);
    }

}