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
            //return lhs.Up == rhs.Up && lhs.Down == rhs.Down;
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

        public bool IsUpDwonSquareColorGrouped() {
            return Up.IsSquare() && Up.IsColorGrouped() && Down.IsSquare() && Down.IsColorGrouped();
        }

        public bool IsUpDwonColorGrouped() {
            return Up.IsColorGrouped() && Down.IsColorGrouped();
        }

        public bool IsUpDownColorSolved() {
            return Up == Layer.YellowSquare && Down == Layer.WhiteSquare;
        }

        public bool IsQuarterPairSolved() {
            return Up.IsQuarterPairSolved() && Down.IsQuarterPairSolved();
        }

        public bool IsCounterQuarterPairSolved() {
            return Up.IsCounterQuarterPairSolved() && Down.IsCounterQuarterPairSolved();
        }

        public bool IsL1CellSolved(int cellCount) {
            return Up.IsL1CellSolved(cellCount) || Down.IsL1CellSolved(cellCount);
        }

        public bool IsL1Solved() {
            return IsL1CellSolved(8);
        }

        public bool IsL3CrossSolved() {
            if (!IsL1Solved()) {
                return false;
            }
            return Up.IsL3CrossSolved() || Down.IsL3CrossSolved();
        }

        public bool IsL3CellSolved(params int[] l3Cells) {
            if (!IsL1Solved()) {
                return false;
            }
            return Up.IsL3CellSolved(l3Cells) || Down.IsL3CellSolved(l3Cells);
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

        public Cube RotateBy(Rotation rotation) {
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

        public static Cube ShapeSolved = new Cube(Layer.Square, Layer.Square);
        public static Cube UpDownColorSolved = new Cube(Layer.YellowSquare, Layer.WhiteSquare);
        public static Cube Solved = new Cube(Layer.YellowL3, Layer.WhiteL1);


        // cubes that L1 need solved first
        public static Cube L1Solved = new Cube(Layer.YellowSquare, Layer.WhiteL1);
        public static Cube L1Quarter123Solved = new Cube(Layer.YellowSquare, new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7));


        // cubes that L3 need solved then
        public static Cube L3CrossSolved = 
            new Cube(new Layer(0, 1, 0, 3, 0, 5, 0, 7), Layer.WhiteL1);
        public static List<Cube> L3CrossUnsolvedList = new List<Cube> {
            new Cube(new Layer(0, 1, 0, 3, 0, 7, 0, 5), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 5, 0, 3, 0, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 5, 0, 7, 0, 3), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 7, 0, 3, 0, 5), Layer.WhiteL1),
            new Cube(new Layer(0, 1, 0, 7, 0, 5, 0, 3), Layer.WhiteL1)
        };

        public static Cube L3CornersSolved = 
            new Cube(new Layer(0, 7, 2, 7, 4, 7, 6, 7), Layer.WhiteL1);
        public static List<Cube> L3CornersUnSolvedList = new List<Cube> {
            new Cube(new Layer(0, 7, 2, 7, 6, 7, 4, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 4, 7, 2, 7, 6, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 4, 7, 6, 7, 2, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 6, 7, 2, 7, 4, 7), Layer.WhiteL1),
            new Cube(new Layer(0, 7, 6, 7, 4, 7, 2, 7), Layer.WhiteL1)
        };
 

        public static Cube L3Cell01Solved = 
            new Cube(new Layer(0, 1, 6, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static List<Cube> L3Cell01UnsolvedList = new List<Cube> {
            new Cube(new Layer(0, 7, 6, 1, 6, 7, 6, 7), Layer.WhiteL1), 
            new Cube(new Layer(1, 6, 7, 0, 7, 6, 7, 6), Layer.WhiteL1), 
            new Cube(new Layer(1, 0, 7, 6, 7, 6, 7, 6), Layer.WhiteL1)
        };


        public static Cube L3Cell012Solved = 
            new Cube(new Layer(0, 1, 2, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static List<Cube> L3Cell012UnsolvedList = new List<Cube> {
            new Cube(new Layer(0, 1, 6, 7, 2, 7, 6, 7), Layer.WhiteL1),
            new Cube(new Layer(2, 7, 0, 1, 6, 7, 6, 7), Layer.WhiteL1)
        };


        public static Cube L3Cell0123Solved = 
            new Cube(new Layer(0, 1, 2, 3, 6, 7, 6, 7), Layer.WhiteL1);
        public static List<Cube> L3Cell0123UnsolvedList = new List<Cube> {
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