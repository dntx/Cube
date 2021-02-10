using System.Collections.Generic;

namespace sq1code
{
    class Layer {
        public List<int> cells { get; }
        Half left;
        Half right;

        public Layer(Half left, Half right) {
            this.cells = new List<int>();
            this.cells.AddRange(left.cells);
            this.cells.AddRange(right.cells);
            
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

        public ISet<Division> GetDivisions(bool normalizedOnly) {
            ISet<Division> divisions = new HashSet<Division>();

            // todo: refine sum logic
            for (int start = 0; start < cells.Count - 1; start++) {
                for (int end = start + 1; end < cells.Count; end++) {
                    Half selectedHalf = new Half(cells.GetRange(start, end - start));
                    int sum = selectedHalf.GetSum();
                    if (sum < 6) {
                        continue;
                    } else if (sum > 6) {
                        break;
                    }

                    List<int> remaining = new List<int>();
                    remaining.AddRange(cells.GetRange(end, cells.Count - end));
                    remaining.AddRange(cells.GetRange(0, start));
                    Half remainingHalf = new Half(remaining);

                    Division divisionNormalized = new Division(selectedHalf, remainingHalf, true);
                    divisions.Add(divisionNormalized);

                    if (!normalizedOnly) {
                        Division divisionRaw = new Division(selectedHalf, remainingHalf);
                        divisions.Add(divisionRaw);
                    }
                }
            }

            return divisions;
        }

        public static Layer Square = new Layer(Half.Square, Half.Square);
        public static Layer Hexagram = new Layer(Half.Hexagram, Half.Hexagram);
    }
}