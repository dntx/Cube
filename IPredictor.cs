namespace Cube
{
    interface IPredictor {
        int PredictCost(ICube cube);
        IPermutation CalcPermutation(ICube cube);
    }
}