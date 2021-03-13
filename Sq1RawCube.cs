using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code
{
    class Sq1RawCube : ICube {
        public Layer Up { get; }
        public Layer Down { get; }

        public Sq1RawCube(Layer up, Layer down) {
            Up = up;
            Down = down;
        }

        public static bool operator == (Sq1RawCube lhs, Sq1RawCube rhs) {
            return (lhs.Up == rhs.Up && lhs.Down == rhs.Down) || (lhs.Up == rhs.Down && lhs.Down == rhs.Up);
            //return lhs.Up == rhs.Up && lhs.Down == rhs.Down;
        }

        public static bool operator != (Sq1RawCube lhs, Sq1RawCube rhs) {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Sq1RawCube);
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

        public bool IsShapeSolved() {
            return Up.IsSquare() && Down.IsSquare();
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

        public ICollection<IRotation> GetRotations() {
            bool lockSquareShape = IsShapeSolved();
            List<IRotation> rotations = new List<IRotation>();

            ISet<Division> upDivisions = Up.GetDivisions(ascendingOnly: true);
            ISet<Division> downDivisions = Down.GetDivisions(ascendingOnly: false);

            foreach (Division upDivision in upDivisions) {
                foreach (Division downDivsion in downDivisions) {
                    Sq1RawRotation rotation = new Sq1RawRotation(upDivision, downDivsion);
                    if (rotation.IsIdentical()) {
                        continue;
                    }
                    if (lockSquareShape && !rotation.IsSquareShapeLocked()) {
                        continue;
                    }
                    rotations.Add(rotation);
                }
            }

            return rotations;
        }

        public ICube RotateBy(IRotation iRotation) {
            Sq1RawRotation rotation = iRotation as Sq1RawRotation;
            Layer up = new Layer(rotation.Up.Left, rotation.Down.Right);
            Layer down = new Layer(rotation.Down.Left, rotation.Up.Right);

            return new Sq1RawCube(up, down);
        }

        public override string ToString()
        {
            return ToString(verbose: false);
        }

        public string ToString(bool verbose)
        {
            return string.Format("{0},{1}", Up.ToString(verbose), Down.ToString(verbose));
        }

        private enum QuarterState { 
            SolvedNone,
            Solved1,
            Solved12,
            Solved13,
            Solved123,
            Solved1234
        };

        public int PredictCost(ICollection<ICube> targetCubes) {
            if (targetCubes.Count > 1) {
                // todo: even for multiple target cubes, we still can give some meaningful prediction
                return 0;
            } else {
                return PredictCost(targetCubes.First());
            }
        }

        public int PredictCost(ICube iTargetCube) {
            Sq1RawCube targetCube = iTargetCube as Sq1RawCube;
            //return 0; /*
            return PredictCostByPairs(this, targetCube);
            //*/
        }

        private static int PredictCostByQuarters(Sq1RawCube cube) {
            KeyValuePair<QuarterState, int> up30State = GetQuarterState(cube.Up, 30);
            KeyValuePair<QuarterState, int> down30State = GetQuarterState(cube.Down, 30);
            KeyValuePair<QuarterState, int> up60State = GetQuarterState(cube.Up, 60);
            KeyValuePair<QuarterState, int> down60State = GetQuarterState(cube.Down, 60);

            int cost30 = PredictCostByQuarterState(up30State, down30State);
            int cost60 = PredictCostByQuarterState(up60State, down60State);
            return cost30 + cost60;
        }

        private static KeyValuePair<QuarterState, int> GetQuarterState(Layer layer, int startDegree) {
            bool[] isQuarterSolved = new bool[4];
            int quarterSolvedCount = 0;
            int start = layer.FindIndex(cell => cell.Degree == startDegree);
            for (int i = start; i < layer.Count; i += 2) {
                Cell first = layer[i];
                Cell second = layer[(i + 1) % layer.Count];
                if (first.Layer == second.Layer) {
                    if (startDegree == 60 && first.LeftSideColor == second.SideColor
                        || startDegree == 30 && first.SideColor == second.RightSideColor) {
                        isQuarterSolved[i/2] = true;
                        quarterSolvedCount++;
                    }
                }
            }

            switch (quarterSolvedCount) {
                case 0:
                    return new KeyValuePair<QuarterState, int>(QuarterState.SolvedNone, 0);
                case 1:
                    return new KeyValuePair<QuarterState, int>(QuarterState.Solved1, 1);
                case 2:
                    QuarterState state = (isQuarterSolved[0] && isQuarterSolved[2] || isQuarterSolved[1] && isQuarterSolved[3])? QuarterState.Solved13 : QuarterState.Solved12;
                    return new KeyValuePair<QuarterState, int>(state, 2);
                case 3:
                    return new KeyValuePair<QuarterState, int>(QuarterState.Solved123, 3);
                case 4:
                    return new KeyValuePair<QuarterState, int>(QuarterState.Solved1234, 4);
            }
            throw new Exception(string.Format("Quarter ready count {0} is not valid", quarterSolvedCount));
        }

        private static int PredictCostByQuarterState(KeyValuePair<QuarterState, int> upState, KeyValuePair<QuarterState, int> downState) {
            // transition list:
            //
            // none => 13
            //    1 => 123, none
            //   12 => 1
            //   13 => 
            //  123 => 13
            // 1234 => 13
            if (upState.Key == QuarterState.Solved12 || downState.Key == QuarterState.Solved12) {
                return 4;
            }

            if (upState.Key == QuarterState.Solved1 || downState.Key == QuarterState.Solved1) {
                return 3;
            }

            if (upState.Key == QuarterState.Solved13 && downState.Key == QuarterState.Solved13) {
                return 1;
            }

            return 2;
        }

        private static int PredictCostByPairs(Sq1RawCube cube, Sq1RawCube targetCube) {
            List<KeyValuePair<int, int>> currentPairs = BreakCubeToPairs(cube);
            List<KeyValuePair<int, int>> targetPairs = BreakCubeToPairs(targetCube);
            for (int i = 0; i < currentPairs.Count; i++) {
                for (int j = 0; j < targetPairs.Count; j++) {
                    if (targetPairs[j].Key == currentPairs[i].Key && targetPairs[j].Value == currentPairs[i].Value) {
                        targetPairs.RemoveAt(j);
                        break;                        
                    }
                }
            }
            return (targetPairs.Count + 3) / 4;
        }

        private static List<KeyValuePair<int, int>> BreakCubeToPairs(Sq1RawCube cube) {
            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < cube.Up.Count; i++) {
                pairs.Add(new KeyValuePair<int, int>(cube.Up[i].Value, cube.Up[(i+1) % cube.Up.Count].Value));
            }
            for (int i = 0; i < cube.Down.Count; i++) {
                pairs.Add(new KeyValuePair<int, int>(cube.Down[i].Value, cube.Down[(i+1) % cube.Down.Count].Value));
            }

            return pairs;
        }
 
        public static Sq1RawCube ShapeSolved = 
            new Sq1RawCube(new Layer(0, 1, 0, 1, 0, 1, 0, 1), new Layer(0, 1, 0, 1, 0, 1, 0, 1));
        public static ISet<ICube> ShapeUnsolvedList = new HashSet<ICube> {
            new Sq1RawCube(new Layer(0, 0, 1, 1, 1, 1, 1, 1, 1, 1), new Layer(0, 0, 0, 0, 0, 0)),
            new Sq1RawCube(new Layer(0, 1, 0, 1, 1, 1, 1, 1, 1, 1), new Layer(0, 0, 0, 0, 0, 0)),
            new Sq1RawCube(new Layer(0, 1, 1, 0, 1, 1, 1, 1, 1, 1), new Layer(0, 0, 0, 0, 0, 0)),
            new Sq1RawCube(new Layer(0, 1, 1, 1, 0, 1, 1, 1, 1, 1), new Layer(0, 0, 0, 0, 0, 0)),
            new Sq1RawCube(new Layer(0, 1, 1, 1, 1, 0, 1, 1, 1, 1), new Layer(0, 0, 0, 0, 0, 0))
        };
    }
}