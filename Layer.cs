using System.Collections.Generic;

namespace sq1code
{
    class Layer : Cells {
        public Half Left { get; }
        public Half Right { get; }

        public Layer(Half left, Half right) : base(GetNormalizedCells(new Cells(left, right))) {
            Left = left;
            Right = right;
        }

        private static Cells GetNormalizedCells(Cells cells) {
            Cells minCells = cells;
            for (int start = 1; start < cells.Count; start++) {
                Cells shiftedCells = new Cells(cells.GetRange(start, cells.Count - start), cells.GetRange(0, start));
                if (shiftedCells < minCells) {
                    minCells = shiftedCells;
                }
            }
            return minCells;
        }

        public bool IsHexagram() {
            return Count == 6 && TrueForAll(cell => GetDegree(cell) == 60);
        }

        public bool IsSquare() {
            int previousDegree = 0;
            return Count == 8 && TrueForAll(cell => {
                int thisDegree = GetDegree(cell);
                bool isChanged = (thisDegree != previousDegree);
                previousDegree = thisDegree;
                return isChanged;
            });
        }

        public override string ToString()
        {
            return ToString(verbose:false);
        }

        public string ToString(bool verbose) {
            if (verbose) {
                return string.Format("{0,4}-{1,-4}", Left, Right);
            } else {
                return ToString(degreeBar: 180, separator: "-");
            }
        }

        public static int HashCodeUpperBound = 10^10;

        public ISet<Division> GetDivisions(bool ascendingOnly) {
            ISet<Division> divisions = new HashSet<Division>();
            for (int start = 0; start < Count - 1; start++) {
                int degreeSum = 0;
                int count = 0;
                for (int i = start; i < Count && degreeSum < 180; i++) {
                    degreeSum += GetDegree(this[i]);
                    count++;
                }

                if (degreeSum == 180) {
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

        public static Layer UnicolorSquare = new Layer(Half.UnicolorSquare, Half.UnicolorSquare);
        public static Layer WhiteSquare = new Layer(Half.WhiteSquare, Half.WhiteSquare);
        public static Layer YellowSquare = new Layer(Half.YellowSquare, Half.YellowSquare);
    }
}