using System;
using System.Collections.Generic;

namespace sq1code
{
    class Half : Cells {
        public Half(List<int> cells) : base(cells) {}
        public Half(List<int> first, List<int> second) : base(first, second) {}

        public static int HashCodeUpperBound = 10^6;

        public static Half Square = new Half(new List<int>{1,2,1,2});
        public static Half Hexagram = new Half(new List<int>{2,2,2});    
    }
}