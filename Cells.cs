using System;
using System.Text;
using System.Collections.Generic;

namespace sq1code
{
    class Cells : List<int> {
        public Cells(List<int> cells) : base(cells) {}

        public Cells(List<int> first, List<int> second) : base(MergeList(first, second)) {}

        private static List<int> MergeList(List<int> first, List<int> second) {
            List<int> result = new List<int>();
            result.AddRange(first);
            result.AddRange(second);
            return result;
        }

        protected string ToString(string halfSeparator) {
            StringBuilder sb = new StringBuilder();
            int countOf1 = 0;
            int sum = 0;
            ForEach(cell => {
                if (cell == 1) {
                    countOf1++;
                } else {    // cell == 2
                    if (countOf1 > 0) {
                        sum += countOf1;
                        sb.Append(countOf1);
                        countOf1 = 0;
                        if (sum == 6) {
                            sb.Append(halfSeparator);
                        }
                    }
                    sb.Append("0");
                    sum += cell;
                    if (sum == 6) {
                        sb.Append(halfSeparator);
                    }
                }
            });

            if (countOf1 > 0) {
                sb.Append(countOf1);
                countOf1 = 0;
            }

            return sb.ToString();
        }

        public override string ToString() {
            return ToString(halfSeparator: "");
        }

        public static bool operator == (Cells lhs, Cells rhs) {
            if (lhs.Count != rhs.Count) {
                return false;
            }

            for (int i = 0; i < lhs.Count; i++) {
                if (lhs[i] != rhs[i]) {
                    return false;
                }
            }

            return true;
        }

        public static bool operator != (Cells lhs, Cells rhs) {
            return !(lhs == rhs);
        }

        public static bool operator < (Cells lhs, Cells rhs) {
            int minCount = Math.Min(lhs.Count, rhs.Count);
            for (int i = 0; i < minCount; i++) {
                if (lhs[i] < rhs[i]) {
                    return true;
                } else if (lhs[i] > rhs[i]) {
                    return false;
                }
            }
            return (lhs.Count == minCount) && (rhs.Count > minCount);
        }

        public static bool operator > (Cells lhs, Cells rhs) {
            int minCount = Math.Min(lhs.Count, rhs.Count);
            for (int i = 0; i < minCount; i++) {
                if (lhs[i] < rhs[i]) {
                    return false;
                } else if (lhs[i] > rhs[i]) {
                    return true;
                }
            }
            return (lhs.Count > minCount) && (rhs.Count == minCount);
        }

        public static bool operator <= (Cells lhs, Cells rhs) {
            return !(lhs > rhs);
        }

        public static bool operator >= (Cells lhs, Cells rhs) {
            return !(lhs < rhs);
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
            int code = 0;
            ForEach(cell => code = code * 10 + cell);
            return code;
        }
    }
}