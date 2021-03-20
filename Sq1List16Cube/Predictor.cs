using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1List16Cube
{
    class Predictor : IPredictor {
        Dictionary<Cells, int> TargetQuarters { get; }

        public Predictor(ICube iTargetCube) {
            Cube targetCube = iTargetCube as Cube;
            var upQuarters = BreakLayerToQuarters(targetCube.Up);
            var downQuarters = BreakLayerToQuarters(targetCube.Down);

            // note: peformance of this linq option maybe a little slow
            // it should be acceptable as it will only be called once
            TargetQuarters = upQuarters.Concat(downQuarters).GroupBy(pair => pair.Key)
                    .ToDictionary(pair => pair.Key, pair => pair.Sum(pair => pair.Value));
        }
        
        public int PredictCost(ICube iCube) {
            Cube cube = iCube as Cube;
            int unsolvedUpQuarterCount = GetLayerUnsolvedQuarterCount(cube.Up);
            int unsolvedDownQuarterCount = GetLayerUnsolvedQuarterCount(cube.Down);
            return (unsolvedUpQuarterCount + unsolvedDownQuarterCount + 3) / 4;
        }

        private int GetLayerUnsolvedQuarterCount(Layer layer) {
            var quarters = BreakLayerToQuarters(layer);
            foreach (var quarter in quarters.Keys) {
                if (TargetQuarters.ContainsKey(quarter)) {
                    int unsolvedCount = Math.Max(0, quarters[quarter] - TargetQuarters[quarter]);
                    quarters[quarter] = unsolvedCount;
                }
            }
            return quarters.Sum(pair => pair.Value);
        }

        private static Dictionary<Cells, int> BreakLayerToQuarters(Layer layer) {
            Dictionary<Cells, int> quarters = new Dictionary<Cells, int>();
            for (int i = 0; i < 8; i++) {
                Cells quarter = new Cells(layer[i], layer[(i + 1) % 8]);
                if (!quarters.ContainsKey(quarter)) {
                    quarters.Add(quarter, 1);
                } else {
                    quarters[quarter]++;
                }
            }
            return quarters;
        }
      }
}