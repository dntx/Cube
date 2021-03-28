namespace Cube
{
    interface IRotation {
        IRotation GetInverseRotation();
        IRotation PermuteBy(IPermutation permutation);
    }
}