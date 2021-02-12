using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half : Cells {
        public Half(List<int> cells, int colorCount) : base(cells, colorCount) {}
        public Half(List<int> first, List<int> second, int colorCount) : base(first, second, colorCount) {}

        public static int HashCodeUpperBound = 16^6;

        public static Half WhiteSquareLeft = new Half(new List<int>{0,1,2,3}, 6);
        public static Half WhiteSquareRight = new Half(new List<int>{4,5,6,7}, 6);
        public static Half YellowSquareLeft = new Half(new List<int>{8,9,10,11}, 6);
        public static Half YellowSquareRight = new Half(new List<int>{12,13,14,15}, 6);
        public static Half WhiteSquare = new Half(WhiteSquareLeft, 2);
        public static Half YellowSquare = new Half(YellowSquareLeft, 2);
        public static Half UnicolorSquare = new Half(WhiteSquare, 1);
    }
}