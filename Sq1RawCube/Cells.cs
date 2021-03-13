using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1RawCube
{
    class Cells : List<int> {
        public int Code { get; }

        public Cells(IEnumerable<int> cells) : base(cells) {
            Code = GetCode(this);
        }
        
        public Cells(IEnumerable<int> first, IEnumerable<int> second) : this(first.Concat(second)) {}
        
        protected static int GetCode(IEnumerable<int> cells) {
            int code = 0;
            foreach (int cell in cells) {
                int degree = (int)cell;
                code = code * 2 + (degree == 30 ? 1 : 0);
            }
            return code;
        }

        protected string ToString(string separator) {
            StringBuilder sb = new StringBuilder();
            int countOf30 = 0;
            int degreeSum = 0;
            for (int i = 0; i < Count; i++) {
                bool isLast = (i == Count - 1);
                int degree = this[i];
                if (degree == 30) {
                    degreeSum += degree;
                    countOf30++;
                } else {
                    if (countOf30 > 0) {
                        sb.Append(countOf30);
                        countOf30 = 0;
                        if (degreeSum == 180 && !isLast) {
                            sb.Append(separator);
                        }
                    }

                    sb.AppendFormat("0");
                    degreeSum += degree;
                    if (degreeSum == 180 && !isLast) {
                        sb.Append(separator);
                    }
                }
            }

            if (countOf30 > 0) {
                sb.Append(countOf30);
                countOf30 = 0;
            }

            return sb.ToString();
        }

        public override string ToString() {
            return ToString(separator: "");
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