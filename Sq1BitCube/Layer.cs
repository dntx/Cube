using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1BitCube
{
    class Layer {
        public uint Code { get; }

        public Layer(params int[] cells) : this(cells as IEnumerable<int>) {}
        
        public Layer(IEnumerable<int> cells) {
            IEnumerable<int> maxPermutedCells = GetMaxPermutedCells(cells);
            Code = GetCode(cells);
        }
        
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

        protected static uint GetCode(IEnumerable<int> cells) {
            uint code = 0;
            foreach (uint cell in cells) {
                code = (code << 4) | cell;
            }
            return code;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            uint degreeSum = 0;
            uint code = Code;
            for (int i = 0; i < 8; i++) {
                uint cell = code & 0xF;
                code >>= 4;
                uint degree = Cell.GetDegree(cell);
                sb.Insert(0, string.Format("{0:X}", cell));
                degreeSum += degree;
                if (degreeSum == 180) {
                    sb.Insert(0, "-");
                }
            }
            return sb.ToString();
        }

        public static bool operator == (Layer lhs, Layer rhs) {
            if (lhs is null || rhs is null) {
                return (lhs is null) && (rhs is null);
            }

            return lhs.Code == rhs.Code;
        }

        public static bool operator != (Layer lhs, Layer rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as Layer);
        }
        
        public override int GetHashCode()
        {
            return (int)Code;
        }

        public static Layer WhiteL1 = new Layer(0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF);
        public static Layer YellowL3 = new Layer(0, 1, 2, 3, 4, 5, 6, 7);
    }
}