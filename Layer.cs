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

        public Layer(Layer layer, Cell.Type type) 
                : this(new Half(layer.Left, type), new Half(layer.Right, type)) {}

        private static Cells GetNormalizedCells(Cells cells) {
            Cells minCells = cells;
            for (int start = 1; start < cells.Count; start++) {
                Cells shiftedCells = new Cells(cells.GetRange(start, cells.Count - start), cells.GetRange(0, start), cells.Type);
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

        public bool IsQuarterSolved() {
            if (!IsSquare()) {
                return false;
            }

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

        public bool IsL3QuartersSolved(int minSolvedCount, int minUnsolvedCount) {
            if (!IsSquare()) {
                return false;
            }

            for (int start = 0; start < Count; start++) {
                int solvedCount = 0;
                int unsolvedCount = 0;
                for (int i = 0; i < Count; i += 2) {
                    Cell cell0 = this[(i + start) % Count];
                    Cell cell1 = this[(i + start + 1) % Count];
                    if (cell0.Layer != 3 || cell0.Degree != 60) {
                        break;
                    }

                    if (cell1.Layer == 3 && cell1.Degree == 30 && cell1.SideColor == cell0.LeftSideColor) {
                        solvedCount++;
                    } else {
                        unsolvedCount++;
                        break;
                    }
                }
                if (solvedCount >= minSolvedCount && unsolvedCount >= minUnsolvedCount) {
                    return true;
                }
            }

            return false;
        }

        public bool IsL3Solved(int minCellSolvedCount) {
            if (!IsSquare()) {
                return false;
            }

            for (int i = 0; i < minCellSolvedCount; i++) {
                if (this[i].Value != i) {
                    return false;
                }
            }

            return true;
        }

        public bool IsL3CrossSolved() {
            if (!IsSquare()) {
                return false;
            }

            Cell previousCell = null;
            int count = 0;
            foreach (Cell cell in this) {
                if (cell.Degree == 30) {
                    if (previousCell != null && cell.Value != (previousCell.Value + 2) % 8) {
                        return false;
                    }
                    previousCell = cell;
                    count++;
                }
            }
            return count == 4;
        }
        
        public bool IsL3SolvedExceptLast2Corners() {
            if (!IsSquare()) {
                return false;
            }

            for (int i = 0; i < 8; i++) {
                if (i == 4 || i == 6) {
                    continue;
                }
                
                if (this[i].Value != i) {
                    return false;
                }
            }

            return true;
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
                    degreeSum += this[i].Degree;
                    count++;
                }

                if (degreeSum == 180) {
                    int end = start + count;
                    Half first = new Half(GetRange(start, count), Type);
                    Half second = new Half(GetRange(end, Count - end), GetRange(0, start), Type);

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
        public static Layer WhiteL1 = new Layer(Half.L1FirstHalf, Half.L1SecondHalf);
        public static Layer YellowL3 = new Layer(Half.L3FirstHalf, Half.L3SecondHalf);
    }
}