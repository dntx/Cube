using System;

namespace Cube.Sq1List2Cube
{
    class Predictor : IPredictor {
        public Predictor(ICube targetCube) {
        }
        
        public int PredictCost(ICube cube) {
            return 0;
        }

        public IPermutation CalcPermutation(ICube cube) {
            throw new NotSupportedException();
        }
    }
}