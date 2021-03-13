using System.Collections.Generic;

namespace sq1code
{
    interface ICube {
        ICollection<IRotation> GetRotations();

        ICube RotateBy(IRotation rotation);

        int PredictCost(ICube targetCube);

        int PredictCost(ICollection<ICube> targetCubes);
    }
}