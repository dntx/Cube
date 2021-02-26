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

        public bool IsL3QuartersSolved(int minSolvedCount, int minUnsolvedCount) {
            return Up.IsL3QuartersSolved(minSolvedCount, minUnsolvedCount) || Down.IsL3QuartersSolved(minSolvedCount, minUnsolvedCount);
        }

        public bool IsL3CellSolved(int minCellSolvedCount) {
            return Up.IsL3CellSolved(minCellSolvedCount) || Down.IsL3CellSolved(minCellSolvedCount);
        }

        public bool IsL1Solved() {
            return Up == Layer.WhiteL1 || Down == Layer.WhiteL1;
        }

        public bool IsSolvedExceptCells(params int[] exceptCells) {
            return Up == Layer.WhiteL1 && Down.IsSolvedExceptL3Cells(exceptCells) || Down == Layer.WhiteL1 && Up.IsSolvedExceptL3Cells(exceptCells);
        }

        public bool IsSolvedExceptL3Corners() {
            return Up == Layer.WhiteL1 && Down.IsL3CrossSolved() || Down == Layer.WhiteL1 && Up.IsL3CrossSolved();
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
        public static Cube L1Solved = new Cube(Layer.YellowSquare, Layer.WhiteL1);
        public static Cube L3Solved = new Cube(Layer.YellowL3, Layer.WhiteSquare);
        public static Cube L3Quarter123Solved = new Cube(new Layer(0, 1, 2, 3, 4, 5, 0x8, 0x9), Layer.WhiteSquare);
        
        public static Cube ExceptCell_0246 = new Cube(new Layer(0, 1, 0, 3, 0, 5, 0, 7), Layer.WhiteL1);
        public static Cube ExceptCell_234567 = new Cube(new Layer(0, 1, 6, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static Cube ExceptCell_34567 = new Cube(new Layer(0, 1, 2, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static Cube ExceptCell_4567 = new Cube(new Layer(0, 1, 2, 3, 6, 7, 6, 7), Layer.WhiteL1);
        public static Cube ExceptCell_57 = new Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 7), Layer.WhiteL1);
    }

}