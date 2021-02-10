using System.Collections.Generic;

namespace sq1code
{
    class Cube {
        Layer up;
        Layer down;

        public Cube(Layer up, Layer down) {
            this.up = up;
            this.down = down;
        }

        public static bool operator == (Cube lhs, Cube rhs) {
            return (lhs.up == rhs.up && lhs.down == rhs.down) || (lhs.up == rhs.down && lhs.down == rhs.up);
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
            int upHashCode = up.GetHashCode();
            int downHashCode = down.GetHashCode();
            if (upHashCode <= downHashCode) {
                return upHashCode * Layer.HashCodeUpperBound + downHashCode;
            } else {
                return downHashCode * Layer.HashCodeUpperBound + upHashCode;
            }
        }

        public bool IsHexagram() {
            return up.IsHexagram() || down.IsHexagram();
        }

        public List<Rotation> GetRotations() {
            List<Rotation> rotations = new List<Rotation>();

            ISet<Division> upDivisions = up.GetDivisions(ascendingOnly: true);
            ISet<Division> downDivisions = down.GetDivisions(ascendingOnly: false);

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
            return string.Format("{0},{1}", up.ToString(verbose), down.ToString(verbose));
        }

        public static Cube Square = new Cube(Layer.Square, Layer.Square);
    }

}