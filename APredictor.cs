using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code {
    class APredictor {
        private enum QuarterState { 
            SolvedNone,
            Solved1,
            Solved12,
            Solved13,
            Solved123,
            Solved1234
        };

        public static int PredictCost(Cube cube, ICollection<Cube> targetCubes) {
            if (targetCubes.Count > 1) {
                // todo: even for multiple target cubes, we still can give some meaningful prediction
                return 0;
            } else {
                return PredictCost(cube, targetCubes.First());
            }
        }

        public static int PredictCost(Cube cube, Cube targetCube) {
            //return 0; /*
            if (targetCube != Cube.Solved) {
                return PredictCostByPairs(cube, targetCube);
            } else {
                return PredictCostByQuarters(cube);
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
            for (int i = 0; i < cube.Up.Count; i++) {
                pairs.Add(new KeyValuePair<int, int>(cube.Up[i].Value, cube.Up[(i+1) % cube.Up.Count].Value));
            }
            for (int i = 0; i < cube.Down.Count; i++) {
                pairs.Add(new KeyValuePair<int, int>(cube.Down[i].Value, cube.Down[(i+1) % cube.Down.Count].Value));
            }

            return pairs;
        }
   }
}