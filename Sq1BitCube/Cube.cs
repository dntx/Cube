using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1BitCube
{
    class Cube : ICube {
        public Layer Up { get; }
        public Layer Down { get; }

        public Cube(Layer up, Layer down) {
            if (up.Code <= down.Code) {
                Up = up;
                Down = down;
            } else {
                Up = down;
                Down = up;
            }
        }

        public static bool operator == (Cube lhs, Cube rhs) {
            return (lhs.Up == rhs.Up && lhs.Down == rhs.Down);
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
            return Rotation.AllRotations;
        }

        public ICube RotateBy(IRotation iRotation) {
            Rotation rotation = iRotation as Rotation;
            
            uint upCode = Layer.RotateLeft(Up.Code, rotation.UpRightStart);
            uint downCode = Layer.RotateLeft(Down.Code, rotation.DownRightStart);
            
            uint rotatedUpCode = (downCode & 0xFFFF0000) | (upCode & 0xFFFF);
            uint rotatedDownCode = (upCode & 0xFFFF0000) | (downCode & 0xFFFF);

            return new Cube(new Layer(rotatedUpCode), new Layer(rotatedDownCode));
        }

        public override string ToString()
        {
            return string.Format("{0,9},{1,-9}", Up, Down);
        }

        private enum QuarterState { 
            SolvedNone,
            Solved1,
            Solved12,
            Solved13,
            Solved123,
            Solved1234
        };

        public int PredictCost(ICube iTargetCube) {
            Cube targetCube = iTargetCube as Cube;
            return 0; /*
            if (targetCube != Cube.Solved) {
                return PredictCostByPairs(this, targetCube);
            } else {
                return PredictCostByQuarters(this);
            }
            //*/
        }

        private static int PredictCostByQuarters(Cube cube) {
            KeyValuePair<QuarterState, int> up30State = GetQuarterState(cube.Up, 30);
            KeyValuePair<QuarterState, int> down30State = GetQuarterState(cube.Down, 30);
            KeyValuePair<QuarterState, int> up60State = GetQuarterState(cube.Up, 60);
            KeyValuePair<QuarterState, int> down60State = GetQuarterState(cube.Down, 60);

            int cost30 = PredictCostByQuarterState(up30State, down30State);
            int cost60 = PredictCostByQuarterState(up60State, down60State);
            return cost30 + cost60;
        }

        private static KeyValuePair<QuarterState, int> GetQuarterState(Layer layer, int startDegree) {
            // bool[] isQuarterSolved = new bool[4];
            // int quarterSolvedCount = 0;
            // int start = layer.FindIndex(cell => Cell.GetDegree(cell) == startDegree);
            // for (int i = start; i < layer.Count; i += 2) {
            //     int first = layer[i];
            //     int second = layer[(i + 1) % layer.Count];
            //     if (Cell.GetLayer(first) == Cell.GetLayer(second)) {
            //         if (startDegree == 60 && Cell.GetLeftSideColor(first) == Cell.GetSideColor(second)
            //             || startDegree == 30 && Cell.GetSideColor(first) == Cell.GetRightSideColor(second)) {
            //             isQuarterSolved[i/2] = true;
            //             quarterSolvedCount++;
            //         }
            //     }
            // }

            // switch (quarterSolvedCount) {
            //     case 0:
            //         return new KeyValuePair<QuarterState, int>(QuarterState.SolvedNone, 0);
            //     case 1:
            //         return new KeyValuePair<QuarterState, int>(QuarterState.Solved1, 1);
            //     case 2:
            //         QuarterState state = (isQuarterSolved[0] && isQuarterSolved[2] || isQuarterSolved[1] && isQuarterSolved[3])? QuarterState.Solved13 : QuarterState.Solved12;
            //         return new KeyValuePair<QuarterState, int>(state, 2);
            //     case 3:
            //         return new KeyValuePair<QuarterState, int>(QuarterState.Solved123, 3);
            //     case 4:
            //         return new KeyValuePair<QuarterState, int>(QuarterState.Solved1234, 4);
            // }
            //throw new Exception(string.Format("Quarter ready count {0} is not valid", quarterSolvedCount));
            throw new NotImplementedException();
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

        private static int PredictCostByPairs(Cube cube, Cube targetCube) {
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

        private static List<KeyValuePair<int, int>> BreakCubeToPairs(Cube cube) {
            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
            // for (int i = 0; i < cube.Up.Count; i++) {
            //     pairs.Add(new KeyValuePair<int, int>(cube.Up[i], cube.Up[(i+1) % cube.Up.Count]));
            // }
            // for (int i = 0; i < cube.Down.Count; i++) {
            //     pairs.Add(new KeyValuePair<int, int>(cube.Down[i], cube.Down[(i+1) % cube.Down.Count]));
            // }
            return pairs;
        }
 
        public static Cube Solved = new Cube(new Layer(0, 1, 2, 3, 4, 5, 6, 7), Layer.WhiteL1);
        public static Cube Cell46Swapped = new Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 5), Layer.WhiteL1);
        public static Cube Cell57Swapped = new Cube(new Layer(0, 1, 2, 3, 6, 5, 4, 7), Layer.WhiteL1);
    }
}