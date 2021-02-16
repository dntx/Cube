using System;
using System.Text;
using System.Collections.Generic;

namespace sq1code
{
    class Cells : List<int> {
        public int ColorCount { get; }

        public Cells(List<int> cells, int colorCount) : base(cells) {
            if (colorCount != 1 && colorCount != 2 && colorCount != 6) {
                throw new ArgumentException("color count should be 1, 2, or 6");
            }
            ColorCount = colorCount;

            if (colorCount == 1 || colorCount == 2) {
                for (int i = 0; i < Count; i++) {
                    int cell = this[i];
                    int degree = GetDegree(cell);
                    int color = GetColor(cell);
                    if (colorCount == 1) {
                        this[i] = degree/30;
                    } else if (colorCount == 2) {
                        this[i] = degree/30 + color*8;
                    }
                }
            }
        }

        public Cells(List<int> first, List<int> second, int colorCount) 
                : this(MergeList(first, second), colorCount) {}
        

        public Cells(Cells cells) : this(cells, cells.ColorCount) {}

        public Cells(Cells first, Cells second) 
                : this(MergeList(first, second), MergeColorCount(first.ColorCount, second.ColorCount)) {}

        private static List<int> MergeList(List<int> first, List<int> second) {
            List<int> result = new List<int>();
            result.AddRange(first);
            result.AddRange(second);
            return result;
        }

        private static int MergeColorCount(int colorCount1, int colorCount2) {
            if (colorCount1 != colorCount2) {
                throw new ArgumentException("color count is not match");
            }

            return colorCount1;
        }

        protected string ToString(int degreeBar, string separator) {
            return (ColorCount == 1)? ToUnicolorString(degreeBar, separator) : ToLiteralString(degreeBar, separator);
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

                    sb.AppendFormat("0");
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

        public static int GetDegree(int cell) {
            return (cell % 2 == 1) ? 30 : 60;
        }

        protected int GetColor(int cell) {
            if (ColorCount == 1) {
                return 0;
            }

            // 0,1,2,3,4,5,6,7: color=0, yellow
            // 8,9,A,B,C,D,E,F: color=1, white
            return cell / 8;
        }

        protected int GetSideColor(int cell) {
            // 7,0: color=0, red
            // 1,2: color=1, blue
            // 3,4: color=2, orange
            // 5,6: color=3, green

            // F,8: color=0, red
            // 9,A: color=1, green
            // B,C: color=2, orange
            // D,E: color=3, blue
            return (cell + 1) % 8 / 2;
        }

        protected int GetPrimaryColor() {
            int color0Count = 0;
            int color1Count = 0;
            ForEach(cell => { 
                if (GetColor(cell) == 0) {
                    color0Count++;
                } else {
                    color1Count++;
                }
            });
            return (color0Count >= color1Count)? 0 : 1;
        }

        public int GetSecondaryColorCount() {
            int primaryColor = GetPrimaryColor();
            return FindAll(cell => GetColor(cell) != primaryColor).Count;
        }

        public Cells GetShape() {
            return new Cells(this, colorCount: 1);
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

        public bool IsSameColor() {
            int color = GetColor(this[0]);
            return TrueForAll(cell => GetColor(cell) == color);
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
            if (lhs.IsSquare() != rhs.IsSquare()) {
                return lhs.IsSquare() && !rhs.IsSquare();
            }

            if (lhs.IsHexagram() != rhs.IsHexagram()) {
                return !lhs.IsHexagram() && rhs.IsHexagram();
            }

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
            if (lhs.IsSquare() != rhs.IsSquare()) {
                return !lhs.IsSquare() && rhs.IsSquare();
            }

            if (lhs.IsHexagram() != rhs.IsHexagram()) {
                return lhs.IsHexagram() && !rhs.IsHexagram();
            }

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