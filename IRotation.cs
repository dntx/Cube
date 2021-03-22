namespace Cube
{
    interface IRotation {
        IRotation GetReversedRotation();

        string ToString(ICube baseCube);
    }
}