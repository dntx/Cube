using System;
using System.Text;
using System.Collections.Generic;

namespace sq1code
{
    class Cells : List<Cell> {
        public Cell.Type Type { get; }

        public Cells(List<int> cells, Cell.Type type) : base(GenerateCellList(cells, type)) {
            Type = type;
        }

        public Cells(List<int> cells) 
                : this(cells, Cell.Type.AsIs) {
        }

        public Cells(List<Cell> cells, Cell.Type type) 
                : this(GenerateIntList(cells), type) {
        }

        public Cells(List<Cell> first, List<Cell> second, Cell.Type type) 
                : this(MergeList(first, second), type) {}
        

        public Cells(Cells cells) : this(cells, cells.Type) {}

        public Cells(Cells first, Cells second) 
                : this(MergeList(first, second), ResolveType(first.Type, second.Type)) {}

        private static List<Cell> GenerateCellList(List<int> cells, Cell.Type type) {
            List<Cell> result = new List<Cell>();
            cells.ForEach(cell => result.Add(new Cell(cell, type)));
            return result;
        }

        private static List<int> GenerateIntList(List<Cell> cells) {
            List<int> result = new List<int>();
            cells.ForEach(cell => result.Add(cell.Value));
            return result;
        }

        private static List<Cell> MergeList(List<Cell> first, List<Cell> second) {
            List<Cell> result = new List<Cell>();
            result.AddRange(first);
            result.AddRange(second);
            return result;
        }

        private static Cell.Type ResolveType(Cell.Type type1, Cell.Type type2) {
            if (type1 != type2) {
                throw new ArgumentException("type is not match");
            }

            return type1;
        }

        protected string ToString(int degreeBar, string separator) {
            return (Type == Cell.Type.IgnoreColor)? ToColorlessString(degreeBar, separator) : ToLiteralString(degreeBar, separator);
        }

        private string ToColorlessString(int degreeBar, string separator) {
            StringBuilder sb = new StringBuilder();
            int countOf30 = 0;
            int degreeSum = 0;
            ForEach(cell => {
                int degree = cell.Degree;
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
                int degree = cell.Degree;
                sb.Append(cell);
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

        protected int GetPrimaryColor() {
            int color0Count = 0;
            int color1Count = 0;
            ForEach(cell => { 
                if (cell.Color == 0) {
                    color0Count++;
                } else {
                    color1Count++;
                }
            });
            return (color0Count >= color1Count)? 0 : 1;
        }

        public int GetSecondaryColorCount() {
            int primaryColor = GetPrimaryColor();
            return FindAll(cell => cell.Color != primaryColor).Count;
        }

        public Cells GetShape() {
            return new Cells(this, Cell.Type.IgnoreColor);
        }

        public bool IsHexagram() {
            return TrueForAll(cell => cell.Degree == 60);
        }

        public bool IsSquare() {
            int previousDegree = 0;
            return TrueForAll(cell => {
                int thisDegree = cell.Degree;
                bool isChanged = (thisDegree != previousDegree);
                previousDegree = thisDegree;
                return isChanged;
            });
        }

        public static bool operator == (Cells lhs, Cells rhs) {
            if (lhs is null || rhs is null) {
                return (lhs is null) && (rhs is null);
            }

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
            ForEach(cell => code = code * 16 + cell.Value);
            return code;
        }
    }
}