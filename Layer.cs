using System.Collections.Generic;

namespace sq1code
{
    class Layer : Cells {
        Half left;
        Half right;

        public Layer(Half left, Half right) : base(left.cells, right.cells) {
            this.left = left;
            this.right = right;
            Normalize();
        }

        private void Normalize() {
            Cells minCells = new Cells(this.cells);
            for (int start = 1; start < cells.Count; start++) {
                if (cells[start - 1] != 1 && cells[start] == 1) {
                    Cells shiftedCells = new Cells(cells.GetRange(start, cells.Count - start), cells.GetRange(0, start));
                    if (shiftedCells < minCells) {
                        minCells = shiftedCells;
                    }
                }
            }
            this.cells = minCells.cells;
        }

        public override string ToString()
        {
            return ToString(withFromInfo:false);
        }

        public string ToString(bool withFromInfo) {
            if (withFromInfo) {
                return left.ToString() + "-" + right.ToString();
            } else {
                return ToString(halfSeparator: "-");
            }
        }

        public static int HashCodeUpperBound = 10^10;

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