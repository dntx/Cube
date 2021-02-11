using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half : Cells {
        public Half(List<int> cells) : base(cells) {}
        public Half(List<int> first, List<int> second) : base(first, second) {}

        public static int HashCodeUpperBound = 10^6;

        public static Half Square = new Half(new List<int>{0,1,0,1});
        public static Half Hexagram = new Half(new List<int>{0,0,0});    
    }
}