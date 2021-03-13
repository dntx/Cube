using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code
{
    class Sq1Cube : ICube {
        public Layer Up { get; }
        public Layer Down { get; }

        public Sq1Cube(Layer up, Layer down) {
            Up = up;
            Down = down;
        }

        public static bool operator == (Sq1Cube lhs, Sq1Cube rhs) {
            return (lhs.Up == rhs.Up && lhs.Down == rhs.Down) || (lhs.Up == rhs.Down && lhs.Down == rhs.Up);
            //return lhs.Up == rhs.Up && lhs.Down == rhs.Down;
        }

        public static bool operator != (Sq1Cube lhs, Sq1Cube rhs) {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Sq1Cube);
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
                    Sq1Rotation rotation = new Sq1Rotation(upDivision, downDivsion);
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
            Sq1Rotation rotation = iRotation as Sq1Rotation;
            Layer up = new Layer(rotation.Up.Left, rotation.Down.Right);
            Layer down = new Layer(rotation.Down.Left, rotation.Up.Right);

            return new Sq1Cube(up, down);
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
            Sq1Cube targetCube = iTargetCube as Sq1Cube;
            //return 0; /*
            if (targetCube != Sq1Cube.Solved) {
                return PredictCostByPairs(this, targetCube);
            } else {
                return PredictCostByQuarters(this);
            }
            //*/
        }

        private static int PredictCostByQuarters(Sq1Cube cube) {
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

        private static int PredictCostByPairs(Sq1Cube cube, Sq1Cube targetCube) {
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

        private static List<KeyValuePair<int, int>> BreakCubeToPairs(Sq1Cube cube) {
            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < cube.Up.Count; i++) {
                pairs.Add(new KeyValuePair<int, int>(cube.Up[i].Value, cube.Up[(i+1) % cube.Up.Count].Value));
            }
            for (int i = 0; i < cube.Down.Count; i++) {
                pairs.Add(new KeyValuePair<int, int>(cube.Down[i].Value, cube.Down[(i+1) % cube.Down.Count].Value));
            }

            return pairs;
        }
 
        public static Sq1Cube UpDownColorSolved = new Sq1Cube(Layer.YellowSquare, Layer.WhiteSquare);
        public static Sq1Cube Solved = new Sq1Cube(Layer.YellowL3, Layer.WhiteL1);


        // cubes that L1 need solved first
        public static Sq1Cube L1Quarter123Solved = 
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7));
        public static ISet<ICube> L1Quarter123UnsolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 7, 6, 0xD)),
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 0xD), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 7, 6, 7))
        };

        public static Sq1Cube L1Quarter4Solved = 
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF));
        public static ISet<ICube> L1Quarter4UnsolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 6, 0xF), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 7)),
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 0xE, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 0xF)),
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 7, 0xE, 0xF), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7)),
            new Sq1Cube(new Layer(6, 7, 6, 7, 0xE, 7, 6, 0xF), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7)),
            new Sq1Cube(new Layer(6, 7, 6, 0xF, 6, 7, 0xE, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7)),
            new Sq1Cube(new Layer(6, 7, 6, 7, 6, 0xF, 0xE, 7), new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 6, 7))
        };

        // cubes that L3 need solved then
        public static Sq1Cube L3CrossSolved = 
            new Sq1Cube(new Layer(0, 1, 0, 3, 0, 5, 0, 7), Layer.WhiteL1);
        public static ISet<ICube> L3CrossUnsolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(0, 1, 0, 3, 0, 7, 0, 5), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 1, 0, 5, 0, 3, 0, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 1, 0, 5, 0, 7, 0, 3), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 1, 0, 7, 0, 3, 0, 5), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 1, 0, 7, 0, 5, 0, 3), Layer.WhiteL1)
        };

        public static Sq1Cube L3CornersSolved = 
            new Sq1Cube(new Layer(0, 7, 2, 7, 4, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3CornersUnSolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(0, 7, 2, 7, 6, 7, 4, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 7, 4, 7, 2, 7, 6, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 7, 4, 7, 6, 7, 2, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 7, 6, 7, 2, 7, 4, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(0, 7, 6, 7, 4, 7, 2, 7), Layer.WhiteL1)
        };
 

        public static Sq1Cube L3Cell01Solved = 
            new Sq1Cube(new Layer(0, 1, 6, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3Cell01UnsolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(0, 7, 6, 1, 6, 7, 6, 7), Layer.WhiteL1), 
            new Sq1Cube(new Layer(1, 6, 7, 0, 7, 6, 7, 6), Layer.WhiteL1), 
            new Sq1Cube(new Layer(1, 0, 7, 6, 7, 6, 7, 6), Layer.WhiteL1)
        };


        public static Sq1Cube L3Cell012Solved = 
            new Sq1Cube(new Layer(0, 1, 2, 7, 6, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3Cell012UnsolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(0, 1, 6, 7, 2, 7, 6, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(2, 7, 0, 1, 6, 7, 6, 7), Layer.WhiteL1)
        };


        public static Sq1Cube L3Cell0123Solved = 
            new Sq1Cube(new Layer(0, 1, 2, 3, 6, 7, 6, 7), Layer.WhiteL1);
        public static ISet<ICube> L3Cell0123UnsolvedList = new HashSet<ICube> {
            new Sq1Cube(new Layer(0, 1, 2, 7, 6, 3, 6, 7), Layer.WhiteL1),
            new Sq1Cube(new Layer(3, 0, 1, 2, 7, 6, 7, 6), Layer.WhiteL1)
        };


        // solve 46 first, then 57
        public static Sq1Cube L3Cell012346 = new Sq1Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 7), Layer.WhiteL1);
        public static Sq1Cube L3Cell012364 = new Sq1Cube(new Layer(0, 1, 2, 3, 6, 7, 4, 7), Layer.WhiteL1);
        public static Sq1Cube L3Cell01234765 = new Sq1Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 5), Layer.WhiteL1);


        // solve 57 first, then 46
        public static Sq1Cube L3Cell012357 = new Sq1Cube(new Layer(0, 1, 2, 3, 6, 5, 6, 7), Layer.WhiteL1);
        public static Sq1Cube L3Cell012375 = new Sq1Cube(new Layer(0, 1, 2, 3, 6, 7, 6, 5), Layer.WhiteL1);
        public static Sq1Cube L3Cell01236547 = new Sq1Cube(new Layer(0, 1, 2, 3, 6, 5, 4, 7), Layer.WhiteL1);


        // scratch
        public static Sq1Cube L3Cell01456723 = new Sq1Cube(new Layer(0, 1, 4, 5, 6, 7, 2, 3), Layer.WhiteL1);
        public static Sq1Cube L3Cell01274563 = new Sq1Cube(new Layer(0, 1, 2, 7, 4, 5, 6, 3), Layer.WhiteL1);
        public static Sq1Cube L1L3Cell08Swapped = new Sq1Cube(new Layer(0x8, 1, 2, 3, 4, 5, 6, 7), new Layer(0, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF));
    }
}