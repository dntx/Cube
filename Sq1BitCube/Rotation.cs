namespace Cube.Sq1BitCube
{
    class Rotation : IRotation {
        public int UpStartIndex { get; }
        public int DownStartIndex { get; }

        public Rotation(int upStartIndex, int downStartIndex) {
            UpStartIndex = upStartIndex;
            DownStartIndex = downStartIndex;
        }

        public override string ToString() {
            return string.Format("{0},{1}", Up, Down);
        }
        public IRotation GetReversedRotation() {
            return this;
        }

        public ISet<Rotation> AllRotations = new HashSet
    }
}