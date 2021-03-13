using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1RawCube
{
    class Layer : Cells {
        public Layer(Cells cells) : base(GetNormalizedCells(cells)) {}
        public Layer(params int[] cells) : this(new Cells(cells)) {}
        public Layer(IEnumerable<int> cells) : this(new Cells(cells)) {}

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

        public bool IsSquare() {
            return Code == 0x55 || Code == 0xAA;
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

        public override string ToString()
        {
            String separator = "-";
            if (IsSquare()) {
                separator = "=";
            } else if (IsSymmetric()) {
                separator = ":";
            }

            return ToString(separator);
        }

        public ISet<Division> GetDivisions() {
            ISet<Division> divisions = new HashSet<Division>();
            int start = 0;
            int last = 0;
            int degreeSum = this[start];
            while (true) {
                if (degreeSum < 180) {
                    last++;
                    if (last >= Count) {
                        break;
                    }
                    degreeSum += this[last];
                } else {
                    if (degreeSum == 180) {
                        int end = last + 1;
                        Cells first = new Cells(GetRange(start, end - start));
                        Cells second = new Cells(GetRange(end, Count - end), GetRange(0, start));
                        divisions.Add(new Division(first, second));
                        divisions.Add(new Division(second, first));
                    }
                    degreeSum -= this[start];
                    start++;
                }
            }
            return divisions;
        }
   }
}