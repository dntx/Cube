using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1List16Cube
{
    class Layer : Cells {
        public Layer(params uint[] cells) : base(GetMinPermutedCells(cells)) {}
        public Layer(IEnumerable<uint> cells) : base(GetMinPermutedCells(cells)) {}

        private static IEnumerable<uint> GetMinPermutedCells(IEnumerable<uint> rawCells) {
            uint minCode = GetCode(rawCells);
            IEnumerable<uint> minCells = rawCells;

            List<uint> cells = rawCells.ToList();
            for (int start = 1; start < cells.Count; start++) {
                IEnumerable<uint> first = cells.GetRange(start, cells.Count - start);
                IEnumerable<uint> second = cells.GetRange(0, start);
                IEnumerable<uint> shiftedCells = first.Concat(second);
                uint code = GetCode(shiftedCells);
                if (code < minCode) {
                    minCode = code;
                    minCells = shiftedCells;
                }
            }
            return minCells;
        }

        public ISet<Division> GetDivisions() {
            ISet<Division> divisions = new HashSet<Division>();
            for (int start = 0; start < 4; start++) {
                int end = start + 4;
                Cells first = new Cells(GetRange(start, end - start));
                Cells second = new Cells(GetRange(end, Count - end).Concat(GetRange(0, start)));
                divisions.Add(new Division(first, second));
                divisions.Add(new Division(second, first));
            }
            return divisions;
        }

        public static Layer WhiteL1 = new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF);
        public static Layer YellowL3 = new Layer(0, 1, 2, 3, 4, 5, 6, 7);
    }
}