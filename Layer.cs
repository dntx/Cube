using System;
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
                Cells shiftedCells = new Cells(cells.GetRange(start, cells.Count - start), cells.GetRange(0, start), cells.ColorCount);
                if (shiftedCells < minCells) {
                    minCells = shiftedCells;
                }
            }
            return minCells;
        }

        public bool IsSymmetric() {
            for (int start = 0; start <= Count/2; start++) {
                bool isSymmetric = true;
                for (int i = 0; i <= Count/2; i++) {
                    int left = (start + i) % Count;
                    int right = (start + Count - i - 1) % Count;
                    if (this[left] != this[right]) {
                        isSymmetric = false;
                        break;
                    }
                }
                if (isSymmetric) {
                    return true;
                }
            }
            return false;
        }

        private int GetColorSegmentCount() {
            int minSegmentCount = Count;
            for (int start = 0; start < Count; start++) {
                int previousColor = -1;
                int segmentCount = 0;
                for (int i = 0; i < Count; i++) {
                    int index = (start + i) % Count;
                    int color = GetColor(this[index]);
                    if (color != previousColor) {
                        segmentCount++;
                    }
                    previousColor = color;
                }
                if (segmentCount < minSegmentCount) {
                    minSegmentCount = segmentCount;
                }
            }
            return minSegmentCount;
        }

        public bool IsColorAdjacent() {
            return GetColorSegmentCount() == 2;
        }

        public override string ToString()
        {
            return ToString(verbose:false);
        }

        public string ToString(bool verbose) {
            String separator = "-";
            if (IsSquare()) {
                separator = "=";
            } else if (IsSymmetric()) {
                separator = ":";
            }

            if (verbose) {
                return string.Format("{0,4}{1}{2,-4}", Left, separator, Right);
            } else {
                return ToString(degreeBar: 180, separator: separator);
            }
        }

        public static int HashCodeUpperBound = 16^10;

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
                    Half first = new Half(GetRange(start, count), ColorCount);
                    Half second = new Half(GetRange(end, Count - end), GetRange(0, start), ColorCount);

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

        public static Layer Square = new Layer(Half.SquareHalf, Half.SquareHalf);
        public static Layer WhiteSquare = new Layer(Half.WhiteSquareHalf, Half.WhiteSquareHalf);
        public static Layer YellowSquare = new Layer(Half.YellowSquareHalf, Half.YellowSquareHalf);
        public static Layer WhiteSquareL1 = new Layer(Half.WhiteSquareBlueHalf, Half.WhiteSquareGreenHalf);
        public static Layer YellowSquareL1 = new Layer(Half.YellowSquareBlueHalf, Half.YellowSquareGreenHalf);
    }
}