using System.Collections.Generic;

namespace Cube.Sq1List16Cube
{
    class Predictor : IPredictor {
        List<uint> TargetQuarters { get; }

        public Predictor(ICube targetCube) {
            TargetQuarters = BreakCubeToQuarters(targetCube as Cube);
        }
        
        public int PredictCost(ICube cube) {
            // note: the quarter list may contain redundant elements
            List<uint> quarters = BreakCubeToQuarters(cube as Cube);
            List<uint> targetQuarters = new List<uint>(TargetQuarters);
            for (int i = quarters.Count - 1; i >= 0; i--) {
                for (int j = 0; j < targetQuarters.Count; j++) {
                    if (quarters[i] == targetQuarters[j]) {
                        quarters.RemoveAt(i);
                        targetQuarters.RemoveAt(j);
                        break;                        
                    }
                }
            }
            return (quarters.Count + 3) / 4;
        }

        private static List<uint> BreakCubeToQuarters(Cube cube) {
            List<uint> Quarters = new List<uint>();
            for (int i = 0; i < cube.Up.Count; i++) {
                Quarters.Add(Cells.GetCode(cube.Up[i], cube.Up[(i+1) % cube.Up.Count]));
            }
            for (int i = 0; i < cube.Down.Count; i++) {
                Quarters.Add(Cells.GetCode(cube.Down[i], cube.Down[(i+1) % cube.Down.Count]));
            }
            return Quarters;
        }
    }
}