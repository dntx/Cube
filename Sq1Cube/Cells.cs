using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1Cube
{
    class Cells : List<int> {
        public int Code { get; }

        public Cells(IEnumerable<int> cells) : base(cells) {
            Code = GetCode(this);
        }
        
        public Cells(List<int> first, List<int> second) : this(MergeList(first, second)) {}
        
        // TODO: Note: MergeList is much faster than first.Contact(second)
        private static List<int> MergeList(List<int> first, List<int> second) {
            List<int> result = new List<int>();
            result.AddRange(first);
            result.AddRange(second);
            return result;
        }

        protected static int GetCode(IEnumerable<int> cells) {
            int code = 0;
            foreach (int cell in cells) {
                code = (code << 4) | cell;
            }
            return code;
        }

        protected string ToString(int degreeBar, string separator) {
            StringBuilder sb = new StringBuilder();
            int degreeSum = 0;
            ForEach(cell => {
                int degree = Cell.GetDegree(cell);
                sb.AppendFormat("{0:X}", cell);
                degreeSum += degree;
                if (degreeSum == degreeBar) {
                    sb.Append(separator);
                }
            });

            return sb.ToString();
        }

        public override string ToString() {
            return ToString(0, separator: "");
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

        public static bool operator < (Cells lhs, Cells rhs) {
            return lhs.Code < rhs.Code;
        }

        public static bool operator > (Cells lhs, Cells rhs) {
            return lhs.Code > rhs.Code;
        }

        public static bool operator <= (Cells lhs, Cells rhs) {
            return lhs.Code <= rhs.Code;
        }

        public static bool operator >= (Cells lhs, Cells rhs) {
            return lhs.Code >= rhs.Code;
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
            return Code;
        }
    }
}