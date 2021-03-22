using System.Text;
using System.Collections.Generic;

namespace Cube.Sq1BitCube
{
    class Cells {
        public uint Code { get; protected set; }

        public Cells(uint code) {
            Code = code;
        }

        public Cells(params uint[] cells) : this(GetCode(cells)) {}
        
        protected static uint GetCode(IEnumerable<uint> cells) {
            uint code = 0;
            foreach (uint cell in cells) {
                code = (code << 4) | cell;
            }
            return code;
        }

        public override string ToString() {
            uint code = Code;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 8; i++) {
                if (i == 4) {
                    sb.Insert(0, "-");
                }
                sb.Insert(0, string.Format("{0:X}", code & 0xF));
                code >>= 4;
            }
            return sb.ToString();
        }

        public static uint RotateCodeLeft(uint code, int shift) {
            if (shift == 0) {
                return code;
            }

            if (shift < 0) {
                return RotateCodeRight(code, -shift);
            }
            
            return (code << (4 * shift)) | (code >> (32 - (4 * shift)));
        }

        public static uint RotateCodeRight(uint code, int shift) {
            if (shift == 0) {
                return code;
            }

            if (shift < 0) {
                return RotateCodeLeft(code, -shift);
            }
            
            return (code >> (4 * shift)) | (code << (32 - (4 * shift)));
        }
 
        public Cells RotateLeft(int shift) {
            return new Cells(RotateCodeLeft(Code, shift));
        }

        public Cells RotateRight(int shift) {
            return new Cells(RotateCodeRight(Code, shift));
        }

        public static bool operator == (Cells lhs, Cells rhs) {
            if (lhs is null || rhs is null) {
                return (lhs is null) && (rhs is null);
            }

            return lhs.Code == rhs.Code;
        }

        public static bool operator != (Cells lhs, Cells rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Cells);
        }
        
        public override int GetHashCode()
        {
            return (int)Code;
        }
    }
}