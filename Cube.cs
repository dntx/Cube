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

        public bool IsHexagram() {
            return Up.IsHexagram() || Down.IsHexagram();
        }

        public List<Rotation> GetRotations() {
            List<Rotation> rotations = new List<Rotation>();

            ISet<Division> upDivisions = Up.GetDivisions(ascendingOnly: true);
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
            return string.Format("{0},{1}", Up.ToString(verbose), Down.ToString(verbose));
        }

        public static Cube UnicolorCube = new Cube(Layer.UnicolorSquare, Layer.UnicolorSquare);
        public static Cube BicolorCube = new Cube(Layer.WhiteSquare, Layer.YellowSquare);
    }

}