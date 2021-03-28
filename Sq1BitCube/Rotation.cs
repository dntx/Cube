using System.Collections.Generic;

namespace Cube.Sq1BitCube
{
    class Rotation : IRotation {
        public Cells Up { get; }
        public Cells Down { get; }

        public Rotation(Cells up, Cells down) {
            Up = up;
            Down = down;
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }

        public IRotation GetInverseRotation() {
            uint rotatedUpCode = (Up.Code & 0xFFFF0000) | (Down.Code & 0xFFFF);
            uint rotatedDownCode = (Down.Code & 0xFFFF0000) | (Up.Code & 0xFFFF);

            return new Rotation(new Cells(rotatedUpCode), new Cells(rotatedDownCode));
        }

        public IRotation PermuteBy(IPermutation iPermutation) {
            Permutation permutation = iPermutation as Permutation;
            Cells permutedUp = Up.PermuteBy(permutation);
            Cells permutedDown = Down.PermuteBy(permutation);
            return new Rotation(permutedUp, permutedDown);
        }
    }
}