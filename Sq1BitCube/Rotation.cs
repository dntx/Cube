using System.Collections.Generic;

namespace Cube.Sq1BitCube
{
    class Rotation : IRotation {
        public int UpRightStart { get; }
        public int DownRightStart { get; }

        public Rotation(int upRightStart, int downRightStart) {
            UpRightStart = upRightStart;
            DownRightStart = downRightStart;
        }

        public override string ToString() {
            return string.Format("{0}-{1}", UpRightStart, DownRightStart);
        }
        public IRotation GetReversedRotation() {
            return this;
        }

        public static List<IRotation> AllRotations = new List<IRotation>() {
            new Rotation(0, 0),
            new Rotation(0, 2),
            new Rotation(0, 4),
            new Rotation(0, 6),

            new Rotation(1, 1),
            new Rotation(1, 3),
            new Rotation(1, 5),
            new Rotation(1, 7),

            new Rotation(2, 0),
            new Rotation(2, 2),
            new Rotation(2, 4),
            new Rotation(2, 6),

            new Rotation(3, 1),
            new Rotation(3, 3),
            new Rotation(3, 5),
            new Rotation(3, 7)
        };
    }
}