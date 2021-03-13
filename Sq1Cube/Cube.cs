using System;
using System.Collections.Generic;
using System.Linq;
using Cube;

namespace Cube.Sq1Cube
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

        public ICollection<IRotation> GetRotations() {
            List<IRotation> rotations = new List<IRotation>();

            ISet<Division> upDivisions = Up.GetDivisions(ascendingOnly: true);
            ISet<Division> downDivisions = Down.GetDivisions(ascendingOnly: false);

            foreach (Division upDivision in upDivisions) {
                foreach (Division downDivsion in downDivisions) {
                    Rotation rotation = new Rotation(upDivision, downDivsion);
                    if (!rotation.IsIdentical() && rotation.IsSquareShapeLocked()) {
                        rotations.Add(rotation);
                    }
                }
            }

            return rotations;
        }

        public ICube RotateBy(IRotation iRotation) {
            Rotation rotation = iRotation as Rotation;
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
            Cube targetCube = iTargetCube as Cube;
            //return 0; /*
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