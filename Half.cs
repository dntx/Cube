using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half : Cells {
        public Half(List<int> cells, int colorCount) : base(cells, colorCount) {}
        public Half(List<int> first, List<int> second, int colorCount) : base(first, second, colorCount) {}

        public static int HashCodeUpperBound = 16^6;

        public static Half YellowSquareBlueHalf = new Half(new List<int>{0,1,2,3}, colorCount: 6);
        public static Half YellowSquareGreenHalf = new Half(new List<int>{4,5,6,7}, colorCount: 6);
        public static Half WhiteSquareBlueHalf = new Half(new List<int>{8,9,10,11}, colorCount: 6);
        public static Half WhiteSquareGreenHalf = new Half(new List<int>{12,13,14,15}, colorCount: 6);
        public static Half YellowSquareHalf = new Half(YellowSquareBlueHalf, colorCount: 2);
        public static Half WhiteSquareHalf = new Half(WhiteSquareBlueHalf, colorCount: 2);
        public static Half SquareHalf = new Half(WhiteSquareHalf, colorCount: 1);
    }
}