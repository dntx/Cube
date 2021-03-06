using System.Collections.Generic;

namespace sq1code {
    class APredictor {
        public static int PredictCost(Cube cube, Cube targetCube) {
            //return 0; /*
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
            //*/
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