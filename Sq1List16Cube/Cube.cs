using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1List16Cube
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
            return (int)(Up.Code ^ Down.Code);
        }

        public ICollection<IRotation> GetRotations() {
            List<IRotation> rotations = new List<IRotation>();

            ISet<Division> upDivisions = Up.GetDivisions();
            ISet<Division> downDivisions = Down.GetDivisions();

            foreach (Division upDivision in upDivisions) {
                foreach (Division downDivision in downDivisions) {
                    if (upDivision.Right != downDivision.Right
                        && upDivision.Left[0] % 2 == downDivision.Left[0] % 2) {
                        Rotation rotation = new Rotation(upDivision, downDivision);
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

        public ICube PermuteBy(IPermutation permutation) {
            if (permutation == null) {
                return this;
            }
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return string.Format("{0,9},{1,-9}", Up, Down);
        }

        public static Cube Solved = new Cube(Layer.YellowL3, Layer.WhiteL1);

        // cubes that L1 need solved first
        public static Cube L1Quarter123Solved = 
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7));
        public static ISet<ICube> L1Quarter123UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 7, 6, 0xD)),
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 0xD), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 7, 6, 7))
        };

        public static Cube L1Quarter4Solved = 
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF));
        public static ISet<ICube> L1Quarter4UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 0xF), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 7)),
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 0xE, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 0xF)),
            new Cube(new Layer(6, 7, 6, 7, 6, 7, 0xE, 0xF), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7)),
            new Cube(new Layer(6, 7, 6, 7, 0xE, 7, 6, 0xF), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7)),
            new Cube(new Layer(6, 7, 6, 0xF, 6, 7, 0xE, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7)),
            new Cube(new Layer(6, 7, 6, 7, 6, 0xF, 0xE, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7))
        };

        // cubes that L3 need solved then
        public static Cube L3CrossSolved = 
            new Cube(new Layer(0, 1, 0, 3, 0, 5, 0, 7), Layer.WhiteL1);
        public static ISet<ICube> L3CrossUnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(0, 1, 0, 3, 0, 7, 0, 5), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 5, 0, 3, 0, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 5, 0, 7, 0, 3), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 7, 0, 3, 0, 5), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 7, 0, 5, 0, 3), Layer.WhiteL1)
        };

        public static Cube L3CornersSolved = 
            new Cube(new Layer(0, 7, 2, 7, 4, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3CornersUnSolvedList = new HashSet<ICube> {
            new Cube(new Layer(0, 7, 2, 7, 6, 7, 4, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 4, 7, 2, 7, 6, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 4, 7, 6, 7, 2, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 6, 7, 2, 7, 4, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 6, 7, 4, 7, 2, 7), Layer.WhiteL1)
        };
 

        public static Cube L3Cell01Solved = 
            new Cube(new Layer(0, 1, 6, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3Cell01UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(0, 7, 6, 1, 6, 7, 6, 7), Layer.WhiteL1), 
            new Cube(new Layer(1, 6, 7, 0, 7, 6, 7, 6), Layer.WhiteL1), 
            new Cube(new Layer(1, 0, 7, 6, 7, 6, 7, 6), Layer.WhiteL1)
        };


        public static Cube L3Cell012Solved = 
            new Cube(new Layer(0, 1, 2, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3Cell012UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(0, 1, 6, 7, 2, 7, 6, 7), Layer.WhiteL1),
            new Cube(new Layer(2, 7, 0, 1, 6, 7, 6, 7), Layer.WhiteL1)
        };


        public static Cube L3Cell0123Solved = 
            new Cube(new Layer(0, 1, 2, 3, 6, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3Cell0123UnsolvedList = new HashSet<ICube> {
            new Cube(new Layer(0, 1, 2, 7, 6, 3, 6, 7), Layer.WhiteL1),
            new Cube(new Layer(3, 0, 1, 2, 7, 6, 7, 6), Layer.WhiteL1)
        };


        // solve 46 first, then 57
        public static Cube L3Cell012346 = new Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 7), Layer.WhiteL1);
        public static Cube L3Cell012364 = new Cube(new Layer(0, 1, 2, 3, 6, 7, 4, 7), Layer.WhiteL1);
        public static Cube L3Cell01234765 = new Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 5), Layer.WhiteL1);


        // solve 57 first, then 46
        public static Cube L3Cell012357 = new Cube(new Layer(0, 1, 2, 3, 6, 5, 6, 7), Layer.WhiteL1);
        public static Cube L3Cell012375 = new Cube(new Layer(0, 1, 2, 3, 6, 7, 6, 5), Layer.WhiteL1);
        public static Cube L3Cell01236547 = new Cube(new Layer(0, 1, 2, 3, 6, 5, 4, 7), Layer.WhiteL1);


        // scratch
        public static Cube L3Cell01456723 = new Cube(new Layer(0, 1, 4, 5, 6, 7, 2, 3), Layer.WhiteL1);
        public static Cube L3Cell01274563 = new Cube(new Layer(0, 1, 2, 7, 4, 5, 6, 3), Layer.WhiteL1);
        public static Cube L1L3Cell08Swapped = new Cube(new Layer(0x8, 1, 2, 3, 4, 5, 6, 7), new Layer(0, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF));
    }
}