using System.Text;
using System.Collections.Generic;

namespace Cube.Sq1List16Cube
{
    class Cells : List<int> {
        public uint Code { get; }

        public Cells(IEnumerable<int> cells) : base(cells) {
            Code = GetCode(this);
        }
        
        protected static uint GetCode(IEnumerable<int> cells) {
            uint code = 0;
            foreach (uint cell in cells) {
                code = (code << 4) | cell;
            }
            return code;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            int degreeSum = 0;
            for (int i = 0; i < Count; i++) {
                int cell = this[i];
                bool isLast = (i == Count - 1);
                int degree = Cell.GetDegree(cell);
                sb.AppendFormat("{0:X}", cell);
                degreeSum += degree;
                if (degreeSum == 180 && !isLast) {
                    sb.Append("-");
                }
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