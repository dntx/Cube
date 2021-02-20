using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half : Cells {
        public Half(params int[] cells) : base(new List<int>(cells)) {}
        public Half(List<int> cells, Cell.Type type) : base(cells, type) {}
        public Half(List<Cell> cells, Cell.Type type) : base(cells, type) {}
        public Half(List<Cell> first, List<Cell> second, Cell.Type type) : base(first, second, type) {}

        public static int HashCodeUpperBound = 16^6;

        public static Half L3FirstHalf = new Half(0, 1, 2, 3);
        public static Half L3SecondHalf = new Half(4, 5, 6, 7);
        public static Half L1FirstHalf = new Half(0x8, 0x9, 0xA, 0xB);
        public static Half L1SecondHalf = new Half(0xC, 0xD, 0xE, 0xF);
        public static Half YellowSquareHalf = new Half(L3FirstHalf, Cell.Type.IgnoreSideColor);
        public static Half WhiteSquareHalf = new Half(L1FirstHalf, Cell.Type.IgnoreSideColor);
        public static Half SquareHalf = new Half(WhiteSquareHalf, Cell.Type.IgnoreColor);
    }
}