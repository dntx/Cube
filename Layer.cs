using System;
using System.Collections.Generic;
using System.Linq;

namespace sq1code
{
    class Layer : Cells {
        public Half Left { get; }
        public Half Right { get; }

        public Layer(Half left, Half right) : base(GetNormalizedCells(new Cells(left, right))) {
            Left = left;
            Right = right;
        }

        public Layer(params int[] cells) 
                : this(new Half(cells.Take(4)), new Half(cells.TakeLast(4))) {
            if (cells.Length != 8) {
                throw new ArgumentException("cells count should be 8");
            }
        }

        private static Cells GetNormalizedCells(Cells cells) {
            Cells minCells = cells;
            for (int start = 1; start < cells.Count; start++) {
                if (cells[start] <= minCells[0]) {
                    Cells shiftedCells = new Cells(cells.GetRange(start, cells.Count - start), cells.GetRange(0, start));
                    if (shiftedCells < minCells) {
                        minCells = shiftedCells;
                    }
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

        public bool IsColorGrouped() {
            int primaryColor = GetPrimaryColor();

            int firstPrimary = -1;
            int lastPrimary = 0;
            int primaryCount = 0;

            int firstSecondary = -1;
            int lastSecondary = 0;
            int secondaryCount = 0;

            for (int i = 0; i < Count; i++) {
                if (this[i].Color == primaryColor) {
                    primaryCount++;
                    if (firstPrimary < 0) {
                        firstPrimary = i;
                    }
                    lastPrimary = i + 1;
                } else {
                    secondaryCount++;
                    if (firstSecondary < 0) {
                        firstSecondary = i;
                    }
                    lastSecondary = i + 1;
                }
            }
            
            return (lastPrimary - firstPrimary == primaryCount) || (lastSecondary - firstSecondary == secondaryCount);
        }

        public bool IsQuarterPairSolved() {
            int start = (this[0].Degree == 60)? 0 : 1;
            for (int i = start; i < Count; i += 2) {
                Cell cell60 = this[i];
                Cell cell30 = this[(i + 1) % Count];
                if (cell60.Layer != cell30.Layer || cell60.LeftSideColor != cell30.SideColor) {
                    return false;
                }
            }

            return true;
        }

        public bool IsCounterQuarterPairSolved() {
            int start = (this[0].Degree == 30)? 0 : 1;
            for (int i = start; i < Count; i += 2) {
                Cell cell30 = this[i];
                Cell cell60 = this[(i + 1) % Count];
                if (cell30.Layer != cell60.Layer || cell30.SideColor != cell60.RightSideColor) {
                    return false;
                }
            }

            return true;
        }

        public bool IsL1CellSolved(int cellCount) {
            int start = FindIndex(cell => cell.Value == 8);
            if (start < 0) {
                return false;
            }
            for (int i = 1; i < cellCount; i++) {
                if (this[(start + i) % 8].Value != i + 8) {
                    return false;
                }
            }
            return true;
        }

        public bool IsL3CrossSolved() {
            int start = FindIndex(cell => cell.Value == 1);
            if (start < 0) {
                return false;
            }
            for (int i = 2; i < 8; i += 2) {
                if (this[(start + i) % 8].Order != i + 1) {
                    return false;
                }
            }
            return true;
        }
        
        public bool IsL3CellSolved(int[] l3Cells) {
            //
            if (!IsSquare()) {
                return false;
            }

            return l3Cells.All(cell => this[cell].Value == cell);
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
            int start = 0;
            int last = 0;
            int degreeSum = this[start].Degree;
            while (true) {
                if (degreeSum < 180) {
                    last++;
                    if (last >= Count) {
                        break;
                    }
                    degreeSum += this[last].Degree;
                } else {
                    if (degreeSum == 180) {
                        int end = last + 1;
                        Half first = new Half(GetRange(start, end - start));
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
                    degreeSum -= this[start].Degree;
                    start++;
                }
            }
            return divisions;
        }

        public static Layer Square = new Layer(Half.SquareHalf, Half.SquareHalf);
        public static Layer WhiteSquare = new Layer(Half.WhiteSquareHalf, Half.WhiteSquareHalf);
        public static Layer YellowSquare = new Layer(Half.YellowSquareHalf, Half.YellowSquareHalf);
        public static Layer WhiteL1 = new Layer(Half.L1FirstHalf, Half.L1SecondHalf);
        public static Layer YellowL3 = new Layer(Half.L3FirstHalf, Half.L3SecondHalf);
    }
}