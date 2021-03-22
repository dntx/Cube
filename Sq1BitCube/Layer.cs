namespace Cube.Sq1BitCube
{
    class Layer : Cells {
        public Layer(uint code) : base(code) {
            Normalize();
        }

        public Layer(params uint[] cells) : base(cells) {
            Normalize();
        }
        
        private void Normalize() {
            uint minCell = uint.MaxValue;
            int minIndex = -1;
            uint code = Code;
            for (int i = 7; i >= 0; i--) {
                uint cell = (code & 0xF);
                if (cell % 2 == 0 && cell < minCell) {
                    minCell = cell;
                    minIndex = i;
                }
                code >>= 4;
            }
            Code = RotateCodeLeft(Code, minIndex);
        }

        public static Layer WhiteL1 = new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF);
    }
}