using System.Text;
using System.Collections.Generic;

namespace Cube.Sq1List16Cube
{
    class Cells : List<uint> {
        public uint Code { get; }

        public Cells(IEnumerable<uint> cells) : base(cells) {
            Code = GetCode(this);
        }
        
        public Cells(params uint[] cells) : this(cells as IEnumerable<uint>) {}
        
        protected static uint GetCode(IEnumerable<uint> cells) {
            uint code = 0;
            foreach (uint cell in cells) {
                code = (code << 4) | cell;
            }
            return code;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; i++) {
                if (i == 4) {
                    sb.Append("-");
                }
                sb.AppendFormat("{0:X}", this[i]);
            }
            return sb.ToString();
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