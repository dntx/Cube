using System.Collections.Generic;

namespace sq1code
{
    class Layer : Cells {
        Half left;
        Half right;

        public Layer(Half left, Half right) : base(left.cells, right.cells) {
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            return left.ToString() + "-" + right.ToString();
        }

        public static bool operator == (Layer lhs, Layer rhs) {
            if (lhs.cells.Count != rhs.cells.Count) {
                return false;
            }

            for (int shift = 0; shift < lhs.cells.Count; shift++) {
                bool isEqual = true;
                for (int i = 0; i < lhs.cells.Count; i++) {
                    int j = (i + shift) % rhs.cells.Count;
                    if (lhs.cells[i] != rhs.cells[j]) {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual) {
                    return true;
                }
            }
            return false;
        }

        public static bool operator != (Layer lhs, Layer rhs) {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Layer);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            int countOf1 = 0;
            int countOfChange = 0;
            for (int i = 0; i < cells.Count; i++) {
                if (cells[i] == 1) {
                    countOf1++;
                }
                int j = (i == 0)? (cells.Count - 1) : (i - 1);
                if (cells[i] != cells[j]) {
                    countOfChange++;
                }
            }
            return countOf1 * 10 + countOfChange;
        }

        public static int HashCodeUpperBound = 100;

        public bool isHexagram() {
            return this == Hexagram;
        }

        public bool isSquare() {
            return this == Square;
        }

        public ISet<Division> GetDivisions(bool ascendingOnly) {
            ISet<Division> divisions = new HashSet<Division>();
            for (int start = 0; start < cells.Count - 1; start++) {
                int sum = 0;
                int count = 0;
                for (int i = start; i < cells.Count && sum < 6; i++) {
                    sum += cells[i];
                    count++;
                }

                if (sum == 6) {
                    int end = start + count;
                    Half first = new Half(cells.GetRange(start, count));
                    Half second = new Half(cells.GetRange(end, cells.Count - end), cells.GetRange(0, start));

                    if (first > second) {
                        Half temp = first;
                        first = second;
                        second = temp;
                    }

                    divisions.Add(new Division(first, second));
                    if (!ascendingOnly && first != second) {
                        divisions.Add(new Division(second, first));
                    }
                }
            }

            return divisions;
        }

        public static Layer Square = new Layer(Half.Square, Half.Square);
        public static Layer Hexagram = new Layer(Half.Hexagram, Half.Hexagram);
    }
}