using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1List2Cube
{
    class Cube : ICube {
        public Layer Up { get; }
        public Layer Down { get; }

        public Cube(Layer up, Layer down) {
            Up = up;
            Down = down;
        }

        public static bool operator == (Cube lhs, Cube rhs) {
            return (lhs.Up == rhs.Up) && (lhs.Down == rhs.Down);
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
            return Up.Code * 2^10 + Down.Code;
        }

        public ICollection<IRotation> GetRotations() {
            List<IRotation> rotations = new List<IRotation>();

            ISet<Division> upDivisions = Up.GetDivisions();
            ISet<Division> downDivisions = Down.GetDivisions();

            foreach (Division upDivision in upDivisions) {
                foreach (Division downDivision in downDivisions) {
                    if (upDivision.Right != downDivision.Right) {
                        rotations.Add(new Rotation(upDivision, downDivision));
                    }
                }
            }

            return rotations;
        }

        public ICube RotateBy(IRotation iRotation) {
            Rotation rotation = iRotation as Rotation;
            Layer up = new Layer(rotation.Up.Left.Concat(rotation.Down.Right));
            Layer down = new Layer(rotation.Down.Left.Concat(rotation.Up.Right));

            return new Cube(up, down);
        }

        public override string ToString()
        {
            return string.Format("{0,9},{1,-9}", Up, Down);
        }

        public int PredictCost(ICube iTargetCube) {
            return 0;
        }

        public static Cube Solved = new Cube(Layer.Squre, Layer.Squre);
        public static ISet<ICube> UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(60, 60, 30, 30, 30, 30, 30, 30, 30, 30), Layer.Hexagram),
            new Cube(new Layer(60, 30, 60, 30, 30, 30, 30, 30, 30, 30), Layer.Hexagram),
            new Cube(new Layer(60, 30, 30, 60, 30, 30, 30, 30, 30, 30), Layer.Hexagram),
            new Cube(new Layer(60, 30, 30, 30, 60, 30, 30, 30, 30, 30), Layer.Hexagram),
            new Cube(new Layer(60, 30, 30, 30, 30, 60, 30, 30, 30, 30), Layer.Hexagram)
        };
    }
}