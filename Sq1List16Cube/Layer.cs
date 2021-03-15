using System;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1List16Cube
{
    class Layer : Cells {
        public Layer(params int[] cells) : base(GetMaxPermutedCells(cells)) {}
        public Layer(IEnumerable<int> cells) : base(GetMaxPermutedCells(cells)) {}

        private static IEnumerable<int> GetMaxPermutedCells(IEnumerable<int> rawCells) {
            List<int> cells = rawCells.ToList();
            int maxCell = cells[0];
            int maxIndex = 0;
            for (int i = 1; i < cells.Count; i++) {
                if (cells[i] > maxCell) {
                    maxCell = cells[i];
                    maxIndex = i;
                }
            }
            int start = maxIndex;
            IEnumerable<int> first = cells.GetRange(start, cells.Count - start);
            IEnumerable<int> second = cells.GetRange(0, start);
            return first.Concat(second);
        }

        public ISet<Division> GetDivisions(bool ascendingOnly) {
            bool needTwoDivisionsOnly = (this == Layer.YellowL3) || (this == Layer.WhiteL1);
            ISet<Division> divisions = new HashSet<Division>();
            for (int start = 0; start < 4; start++) {
                int end = start + 4;
                Cells first = new Cells(GetRange(start, end - start));
                Cells second = new Cells(GetRange(end, Count - end).Concat(GetRange(0, start)));
                if (first.Code > second.Code) {
                    Cells temp = first;
                    first = second;
                    second = temp;
                }

                divisions.Add(new Division(first, second));
                if (!ascendingOnly && !needTwoDivisionsOnly && first != second) {
                    divisions.Add(new Division(second, first));
                }

                if (needTwoDivisionsOnly && divisions.Count == 2) {
                    break;
                }
            }
            return divisions;
        }

        public static Layer WhiteL1 = new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF);
        public static Layer YellowL3 = new Layer(0, 1, 2, 3, 4, 5, 6, 7);
    }
}