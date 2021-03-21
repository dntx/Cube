using System;
using System.Collections.Generic;

namespace Cube.Sq1BitCube
{
    class Predictor : IPredictor {
        ISet<uint> TargetQuarters { get; }

        public Predictor(ICube targetCube) {
            TargetQuarters = BreakCubeToQuarters(targetCube as Cube);
        }
        
        public int PredictCost(ICube cube) {
            //return PredictByUnsolvedQuarterCount(cube as Cube);
            return PredictByQuarterState(cube as Cube);
        }

        private enum QuarterState { 
            SolvedNone,
            Solved1,
            Solved12,
            Solved13,
            Solved123,
            Solved1234
        };

        private int PredictByQuarterState(Cube cube) {
            QuarterState up30State = GetQuarterState(cube.Up, 30);
            QuarterState down30State = GetQuarterState(cube.Down, 30);
            QuarterState up60State = GetQuarterState(cube.Up, 60);
            QuarterState down60State = GetQuarterState(cube.Down, 60);

            int cost30 = PredictByQuarterState(up30State, down30State);
            int cost60 = PredictByQuarterState(up60State, down60State);
            return cost30 + cost60;
        }

        private QuarterState GetQuarterState(Layer layer, int startDegree) {
            bool[] isSolved = new bool[4];
            int solvedCount = 0;
            uint code = layer.Code;
            if (startDegree == 30) {
                code = Layer.RotateLeft(code, 1);
            }
 
            for (int i = 0; i < 4; i++) {
                uint quarter = code & 0xFF;
                code <<= 8;
 
                if (TargetQuarters.Contains(quarter)) {
                    isSolved[i] = true;
                    solvedCount++;
                }
            }

            switch (solvedCount) {
                case 0:
                    return QuarterState.SolvedNone;
                case 1:
                    return QuarterState.Solved1;
                case 2:
                    return (isSolved[0] && isSolved[2] || isSolved[1] && isSolved[3])? QuarterState.Solved13 : QuarterState.Solved12;
                case 3:
                    return QuarterState.Solved123;
                case 4:
                    return QuarterState.Solved1234;
            }
            throw new Exception(string.Format("Quarter solved count {0} is not valid", solvedCount));
        }

        private static int PredictByQuarterState(QuarterState upState, QuarterState downState) {
            // transition table:
            //
            // none => 13
            //    1 => 123, none
            //   12 => 1
            //   13 => 
            //  123 => 13
            // 1234 => 13
            if (upState == QuarterState.Solved12 || downState == QuarterState.Solved12) {
                return 4;
            }

            if (upState == QuarterState.Solved1 || downState == QuarterState.Solved1) {
                return 3;
            }

            if (upState == QuarterState.Solved13 && downState == QuarterState.Solved13) {
                return 1;
            }

            return 2;
        }

        private int PredictByUnsolvedQuarterCount(Cube cube) {
            ISet<uint> quarters = BreakCubeToQuarters(cube);
            quarters.ExceptWith(TargetQuarters);
            int unsolvedCount = quarters.Count;
            return (unsolvedCount + 3) / 4;
        }

        private static ISet<uint> BreakCubeToQuarters(Cube cube) {
            ISet<uint> quarters = new HashSet<uint>();
            quarters.UnionWith(BreakLayerToQuarters(cube.Up));
            quarters.UnionWith(BreakLayerToQuarters(cube.Down));
            return quarters;
        }

        private static ISet<uint> BreakLayerToQuarters(Layer layer) {
            ISet<uint> quarters = new HashSet<uint>();
            uint code = layer.Code;
            uint cell7 = code & 0xF;
            for (int i = 0; i < 7; i++) {
                quarters.Add(code & 0xFF);
                code >>= 4;
            }
            uint cell0 = code;
            quarters.Add((cell7 << 4) | cell0);
            return quarters;
        }
    }
}