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

        public bool SameAs(Cube other) {
            return (up.SameAs(other.up) && down.SameAs(other.down)) || (up.SameAs(other.down) && down.SameAs(other.up));
        }

        public bool isHexagram() {
            return up.isHexagram() || down.isHexagram();
        }

        public List<Rotation> GetRotations() {
            List<Rotation> rotations = new List<Rotation>();

            List<Division> upDivisions = up.GetDivisions(true);
            List<Division> downDivisions = down.GetDivisions(false);

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