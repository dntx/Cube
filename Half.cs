using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half {
        public List<int> cells { get; }

        public Half(List<int> cells) {
            this.cells = new List<int>(cells);
        }

        public override string ToString() {
            string s = "";
            int countOf1 = 0;
            foreach (int cell in cells) {
                if (cell == 1) {
                    countOf1++;
                } else {    // cell == 2
                    if (countOf1 > 0) {
                        s += countOf1;
                        countOf1 = 0;
                    }
                    s += "0";
                } 
            }

            if (countOf1 > 0) {
                s += countOf1;
                countOf1 = 0;
            }

            return s;
        }

        public static int CompareToHalf(List<int> cells) {
            int sum = 0;
            foreach (int i in cells) {
                sum += i;
            }
            return sum - 6;
        }

        public static bool operator == (Half lhs, Half rhs) {
            if (lhs.cells.Count != rhs.cells.Count) {
                return false;
            }

            for (int i = 0; i < lhs.cells.Count; i++) {
                if (lhs.cells[i] != rhs.cells[i]) {
                    return false;
                }
            }

            return true;
        }

        public static bool operator != (Half lhs, Half rhs) {
            return !(lhs == rhs);
        }

        public static bool operator < (Half lhs, Half rhs) {
            int minCount = Math.Min(lhs.cells.Count, rhs.cells.Count);
            for (int i = 0; i < minCount; i++) {
                if (lhs.cells[i] < rhs.cells[i]) {
                    return true;
                } else if (lhs.cells[i] > rhs.cells[i]) {
                    return false;
                }
            }
            return (lhs.cells.Count == minCount) && (rhs.cells.Count > minCount);
        }

        public static bool operator > (Half lhs, Half rhs) {
            int minCount = Math.Min(lhs.cells.Count, rhs.cells.Count);
            for (int i = 0; i < minCount; i++) {
                if (lhs.cells[i] < rhs.cells[i]) {
                    return false;
                } else if (lhs.cells[i] > rhs.cells[i]) {
                    return true;
                }
            }
            return (lhs.cells.Count > minCount) && (rhs.cells.Count == minCount);
        }

        public static bool operator <= (Half lhs, Half rhs) {
            return !(lhs > rhs);
        }

        public static bool operator >= (Half lhs, Half rhs) {
            return !(lhs < rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Half);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            int code = 0;
            foreach (int cell in cells) {
                code = code * 10 + cell;
            }
            return code;
        }

        public static Half Square = new Half(new List<int>{1,2,1,2});
        public static Half Hexagram = new Half(new List<int>{2,2,2});    
    }
}