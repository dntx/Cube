using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1BitCube
{
    class Cube : ICube {
        public Layer Up { get; }
        public Layer Down { get; }

        public Cube(Layer up, Layer down) {
            if (up.Code <= down.Code) {
                Up = up;
                Down = down;
            } else {
                Up = down;
                Down = up;
            }
        }

        public static bool operator == (Cube lhs, Cube rhs) {
            return (lhs.Up == rhs.Up && lhs.Down == rhs.Down);
        }

        public static bool operator != (Cube lhs, Cube rhs) {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Cube);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return (int)(Up.Code ^ Down.Code);
        }

        public ICollection<IRotation> GetRotations() {
            var rotations = new List<IRotation>();
            int maxUpIndex = 4;
            int maxDownIndex = 8;
            var upDivisions = Up.GetDivisions(maxUpIndex);
            var downDivisions = Down.GetDivisions(maxDownIndex);

            // i and j should have the same odevity
            for (int i = 0; i < maxUpIndex; i++) {
                for (int j = i % 2; j < maxDownIndex; j += 2) {
                    Rotation rotation = new Rotation(upDivisions[i], downDivisions[j]);
                    rotations.Add(rotation);
                }
            }

            return rotations;
        }

        public ICube RotateBy(IRotation iRotation) {
            Rotation rotation = iRotation as Rotation;
            uint rotatedUpCode = (rotation.Up.Code & 0xFFFF0000) | (rotation.Down.Code & 0xFFFF);
            uint rotatedDownCode = (rotation.Down.Code & 0xFFFF0000) | (rotation.Up.Code & 0xFFFF);

            return new Cube(new Layer(rotatedUpCode), new Layer(rotatedDownCode));
        }

        public ICube PermuteBy(IPermutation iPermutation) {
            Permutation permutation = iPermutation as Permutation;
            Cells permutedUp = Up.PermuteBy(permutation);
            Cells permutedDown = Down.PermuteBy(permutation);
            return new Cube(new Layer(permutedUp.Code), new Layer(permutedDown.Code));
        }

        public override string ToString()
        {
            return string.Format("{0,9},{1,-9}", Up, Down);
        }

        public static Cube Solved = new Cube(new Layer(0x01234567), Layer.WhiteL1);
        public static Cube Cell46Swapped = new Cube(new Layer(0x01236547), Layer.WhiteL1);
        public static Cube Cell57Swapped = new Cube(new Layer(0x01234765), Layer.WhiteL1);

        public static Cube CellDepth1 = new Cube(new Layer(0x123CDEF), new Layer(0x89AB4567));
        public static Cube CellDepth2 = new Cube(new Layer(0x01AB45EF), new Layer(0x8923CD67));
        public static Cube CellDepth6 = new Cube(new Layer(0x05876B2D), new Layer(0x49AFE1C3));
        public static Cube CellDepth7 = new Cube(new Layer(0x01C96325), new Layer(0x478BEFAD));
        public static Cube CellDepthX = new Cube(new Layer(0x01A5EF2B), new Layer(0x47CD6389));
        public static Cube CellDepthY = new Cube(new Layer(0x09A1256F), new Layer(0x438BEDC7));
    }
}