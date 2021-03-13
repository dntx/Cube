using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1RawCube
{
    class Cube : ICube {
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
                return upHashCode * 2^10 + downHashCode;
            } else {
                return downHashCode * 2^10 + upHashCode;
            }
        }

        public ICollection<IRotation> GetRotations() {
            List<IRotation> rotations = new List<IRotation>();

            ISet<Division> upDivisions = Up.GetDivisions();
            ISet<Division> downDivisions = Down.GetDivisions();

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

        public int PredictCost(ICollection<ICube> targetCubes) {
            return 0;
        }

        public int PredictCost(ICube iTargetCube) {
            return 0;
        }

        public static Cube Solved = 
            new Cube(new Layer(60, 30, 60, 30, 60, 30, 60, 30), new Layer(60, 30, 60, 30, 60, 30, 60, 30));
        public static ISet<ICube> UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(60, 60, 30, 30, 30, 30, 30, 30, 30, 30), new Layer(60, 60, 60, 60, 60, 60)),
            new Cube(new Layer(60, 30, 60, 30, 30, 30, 30, 30, 30, 30), new Layer(60, 60, 60, 60, 60, 60)),
            new Cube(new Layer(60, 30, 30, 60, 30, 30, 30, 30, 30, 30), new Layer(60, 60, 60, 60, 60, 60)),
            new Cube(new Layer(60, 30, 30, 30, 60, 30, 30, 30, 30, 30), new Layer(60, 60, 60, 60, 60, 60)),
            new Cube(new Layer(60, 30, 30, 30, 30, 60, 30, 30, 30, 30), new Layer(60, 60, 60, 60, 60, 60))
        };
    }
}