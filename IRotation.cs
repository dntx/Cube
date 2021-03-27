namespace Cube
{
    interface IRotation {
        IRotation GetInverseRotation();

        string ToString(ICube baseCube);
    }
}