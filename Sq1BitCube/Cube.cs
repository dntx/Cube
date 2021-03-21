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
            return Rotation.AllRotations;
        }

        public ICube RotateBy(IRotation iRotation) {
            Rotation rotation = iRotation as Rotation;
            
            uint upCode = Layer.RotateLeft(Up.Code, rotation.UpRightStart);
            uint downCode = Layer.RotateLeft(Down.Code, rotation.DownRightStart);
            
            uint rotatedUpCode = (downCode & 0xFFFF0000) | (upCode & 0xFFFF);
            uint rotatedDownCode = (upCode & 0xFFFF0000) | (downCode & 0xFFFF);

            return new Cube(new Layer(rotatedUpCode), new Layer(rotatedDownCode));
        }

        public override string ToString()
        {
            return string.Format("{0,9},{1,-9}", Up, Down);
        }

        public static Cube Solved = new Cube(new Layer(0, 1, 2, 3, 4, 5, 6, 7), Layer.WhiteL1);
        public static Cube Cell46Swapped = new Cube(new Layer(0, 1, 2, 3, 6, 5, 4, 7), Layer.WhiteL1);
        public static Cube Cell57Swapped = new Cube(new Layer(0, 1, 2, 3, 4, 7, 6, 5), Layer.WhiteL1);

        public static Cube CellDepth6 = new Cube(new Layer(0, 5, 0x8, 7, 6, 0xB, 2, 0xD), new Layer(4, 0x9, 0xA, 0xF, 0xE, 1, 0xC, 3));
    }
}