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
            return up.GetHashCode() * 1000000000 + down.GetHashCode();
        }

        public bool isHexagram() {
            return up.isHexagram() || down.isHexagram();
        }

        public List<Rotation> GetRotations() {
            List<Rotation> rotations = new List<Rotation>();

            ISet<Division> upDivisions = up.GetDivisions(true);
            ISet<Division> downDivisions = down.GetDivisions(false);

            foreach (Division upDivision in upDivisions) {
                foreach (Division downDivsion in downDivisions) {
                    Rotation rotation = new Rotation(upDivision, downDivsion);
                    if (!rotation.isIdenticalRotation()) {
                        rotations.Add(rotation);
                    }
                }
            }

            return rotations;
        }

        public Cube ApplyRotation(Rotation rotation) {
            Layer up = new Layer(rotation.up.left, rotation.down.right);
            Layer down = new Layer(rotation.down.left, rotation.up.right);

            return new Cube(up, down);
        }

        public override string ToString()
        {
            return up.ToString() + "," + down.ToString();
        }

        public static Cube Square = new Cube(Layer.Square, Layer.Square);
    }

}