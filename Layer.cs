using System.Collections.Generic;

namespace sq1code
{
    class Layer : Cells {
        Half left;
        Half right;

        public Layer(Half left, Half right) : base(GetNormalizedCells(new Cells(left, right))) {
            this.left = left;
            this.right = right;
        }

        private static Cells GetNormalizedCells(Cells cells) {
            Cells minCells = cells;
            for (int start = 1; start < cells.Count; start++) {
                // should start from a brand new 1
                if (cells[start - 1] == 1 || cells[start] != 1) {
                    continue;
                }

                Cells shiftedCells = new Cells(cells.GetRange(start, cells.Count - start), cells.GetRange(0, start));
                if (shiftedCells < minCells) {
                    minCells = shiftedCells;
                }
            }
            return minCells;
        }

        public override string ToString()
        {
            return ToString(verbose:false);
        }

        public string ToString(bool verbose) {
            if (verbose) {
                return string.Format("{0}-{1}", left, right);
            } else {
                return ToString(bar: 6, separator: "-");
            }
        }

        public static int HashCodeUpperBound = 10^10;

        public bool IsHexagram() {
            return this == Hexagram;
        }

        public bool IsSquare() {
            return this == Square;
        }

        public ISet<Division> GetDivisions(bool ascendingOnly) {
            ISet<Division> divisions = new HashSet<Division>();
            for (int start = 0; start < Count - 1; start++) {
                int sum = 0;
                int count = 0;
                for (int i = start; i < Count && sum < 6; i++) {
                    sum += this[i];
                    count++;
                }

                if (sum == 6) {
                    int end = start + count;
                    Half first = new Half(GetRange(start, count));
                    Half second = new Half(GetRange(end, Count - end), GetRange(0, start));

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