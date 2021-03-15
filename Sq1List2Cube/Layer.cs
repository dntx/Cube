using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1List2Cube
{
    class Layer : Cells {
        public Layer(params int[] cells) : base(GetMinPermutedCells(cells)) {}
        public Layer(IEnumerable<int> cells) : base(GetMinPermutedCells(cells)) {}

        private static IEnumerable<int> GetMinPermutedCells(IEnumerable<int> rawCells) {
            int minCode = GetCode(rawCells);
            IEnumerable<int> minCells = rawCells;

            List<int> cells = rawCells.ToList();
            for (int start = 1; start < cells.Count; start++) {
                IEnumerable<int> first = cells.GetRange(start, cells.Count - start);
                IEnumerable<int> second = cells.GetRange(0, start);
                IEnumerable<int> shiftedCells = first.Concat(second);
                int code = GetCode(shiftedCells);
                if (code < minCode) {
                    minCode = code;
                    minCells = shiftedCells;
                }
            }
            return minCells;
        }

        public bool IsSquare() {
            return this == Layer.Squre;
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

        public static Layer Squre = new Layer(60, 30, 60, 30, 60, 30, 60, 30);
        public static Layer Hexagram = new Layer(60, 60, 60, 60, 60, 60);
   }
}