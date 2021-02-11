using System;
using System.Text;
using System.Collections.Generic;

namespace sq1code
{
    class Cells : List<int> {
        bool isUnicolor;

        public Cells(List<int> cells) : base(cells) {
            isUnicolor = cells.TrueForAll(cell => cell < 2);
        }

        public Cells(List<int> first, List<int> second) : this(MergeList(first, second)) {}

        private static List<int> MergeList(List<int> first, List<int> second) {
            List<int> result = new List<int>();
            result.AddRange(first);
            result.AddRange(second);
            return result;
        }

        protected string ToString(int degreeBar, string separator) {
            return isUnicolor? ToUnicolorString(degreeBar, separator) : ToLiteralString(degreeBar, separator);
        }

        private string ToUnicolorString(int degreeBar, string separator) {
            StringBuilder sb = new StringBuilder();
            int countOf30 = 0;
            int degreeSum = 0;
            ForEach(cell => {
                int degree = GetDegree(cell);
                if (degree == 30) {
                    degreeSum += degree;
                    countOf30++;
                } else {
                    if (countOf30 > 0) {
                        sb.Append(countOf30);
                        countOf30 = 0;
                        if (degreeSum == degreeBar) {
                            sb.Append(separator);
                        }
                    }

                    sb.Append(cell);
                    degreeSum += degree;
                    if (degreeSum == degreeBar) {
                        sb.Append(separator);
                    }
                }
            });

            if (countOf30 > 0) {
                sb.Append(countOf30);
                countOf30 = 0;
            }

            return sb.ToString();
        }

        private string ToLiteralString(int degreeBar, string separator) {
            StringBuilder sb = new StringBuilder();
            int degreeSum = 0;
            ForEach(cell => {
                int degree = GetDegree(cell);
                sb.Append(cell);
                degreeSum += degree;
                if (degreeSum == degreeBar) {
                    sb.Append(separator);
                }
            });

            return sb.ToString();
        }

        protected static int GetDegree(int cell) {
            return (cell % 2 == 1) ? 30 : 60;
        }

        public bool IsHexagram() {
            return TrueForAll(cell => GetDegree(cell) == 60);
        }

        public bool IsSquare() {
            int previousDegree = 0;
            return TrueForAll(cell => {
                int thisDegree = GetDegree(cell);
                bool isChanged = (thisDegree != previousDegree);
                previousDegree = thisDegree;
                return isChanged;
            });
        }

        public override string ToString() {
            return ToString(0, separator: "");
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