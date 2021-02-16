using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half : Cells {
        public Half(List<int> cells) : base(cells) {}
        public Half(List<int> cells, Cell.Type type) : base(cells, type) {}
        public Half(List<Cell> cells, Cell.Type type) : base(cells, type) {}
        public Half(List<Cell> first, List<Cell> second, Cell.Type type) : base(first, second, type) {}

        public static int HashCodeUpperBound = 16^6;

        public static Half YellowSquareBlueHalf = new Half(new List<int>{0,1,2,3});
        public static Half YellowSquareGreenHalf = new Half(new List<int>{4,5,6,7});
        public static Half WhiteSquareBlueHalf = new Half(new List<int>{8,9,10,11});
        public static Half WhiteSquareGreenHalf = new Half(new List<int>{12,13,14,15});
        public static Half YellowSquareHalf = new Half(YellowSquareBlueHalf, Cell.Type.IgnoreSideColor);
        public static Half WhiteSquareHalf = new Half(WhiteSquareBlueHalf, Cell.Type.IgnoreSideColor);
        public static Half SquareHalf = new Half(WhiteSquareHalf, Cell.Type.IgnoreColor);
    }
}