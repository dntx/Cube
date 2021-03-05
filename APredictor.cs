using System.Collections.Generic;

namespace sq1code {
    class APredictor {
        private List<KeyValuePair<int, int>> targetPairs;
        public APredictor(Cube targetCube) {
            targetPairs = BreakCubeToPairs(targetCube);
        }

        // todo: make it more accurate
        public int PredictCost(Cube cube) {
            return 0; /*
            List<KeyValuePair<int, int>> currentPairs = BreakCubeToPairs(cube);
            int matched = 0;
            for (int i = 0; i < targetPairs.Count; i++) {
                for (int j = 0; j < currentPairs.Count; j++) {
                    if (targetPairs[i].Key == currentPairs[j].Key && targetPairs[i].Value == currentPairs[j].Value) {
                        currentPairs.RemoveAt(j);
                        matched++;
                        break;                        
                    }
                }
            }
            int unmatched = targetPairs.Count - matched;
            return (unmatched + 3) / 4;
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