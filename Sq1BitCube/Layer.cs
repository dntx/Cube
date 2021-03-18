using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1BitCube
{
    class Layer {
        public uint Code { get; }

        public Layer(uint rawCode) {
            Code = GetMinPermutedCode(rawCode);
        }

        public Layer(params uint[] cells) : this(GetCode(cells)) {}
        
        private static uint GetMinPermutedCode(uint rawCode) {
            uint minCell = uint.MaxValue;
            int minIndex = -1;
            uint code = rawCode;
            for (int i = 7; i >= 0; i--) {
                uint cell = (code & 0xF);
                if (cell % 2 == 0 && cell < minCell) {
                    minCell = cell;
                    minIndex = i;
                }
                code >>= 4;
            }
            return RotateLeft(rawCode, minIndex);
        }

        public static uint RotateLeft(uint code, int indexShift) {
            if (indexShift == 0) {
                return code;
            } else {
                return (code << (4 * indexShift)) | (code >> (32 - (4 * indexShift)));
            }
        }

        protected static uint GetCode(IEnumerable<uint> cells) {
            uint code = 0;
            foreach (uint cell in cells) {
                code = (code << 4) | cell;
            }
            return code;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            uint code = Code;
            for (int i = 0; i < 8; i++) {
                if (i == 4) {
                    sb.Insert(0, "-");
                }
                sb.Insert(0, string.Format("{0:X}", code & 0xF));
                code >>= 4;
            }
            return sb.ToString();
        }

        public static bool operator == (Layer lhs, Layer rhs) {
            if (lhs is null || rhs is null) {
                return (lhs is null) && (rhs is null);
            }

            return lhs.Code == rhs.Code;
        }

        public static bool operator != (Layer lhs, Layer rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Layer);
        }
        
        public override int GetHashCode()
        {
            return (int)Code;
        }

        public static Layer WhiteL1 = new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF);
    }
}