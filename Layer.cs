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
            int code = 0;
            foreach (int cell in cells) {
                code = code * 10 + cell;
            }
            return code;
        }

        public bool isHexagram() {
            return this == Hexagram;
        }

        public bool isSquare() {
            return this == Square;
        }

        public ISet<Division> GetDivisions(bool normalizedOnly) {
            ISet<Division> divisions = new HashSet<Division>();

            for (int start = 0; start < cells.Count - 1; start++) {
                for (int end = start + 1; end < cells.Count; end++) {
                    List<int> selected = cells.GetRange(start, end - start);
                    int result = Half.CompareToHalf(selected);
                    if (result < 0) {
                        continue;
                    } else if (result > 0) {
                        break;
                    }
                    Half selectedHalf = new Half(selected);

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