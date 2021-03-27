using System.Collections.Generic;
using System.Linq;

namespace Cube.Sq1BitCube
{
    class Permutation : IPermutation {
        public Dictionary<uint, uint> Map { get; }

        public Permutation(Dictionary<uint, uint> map) {
            Map = map;
        }

        public IPermutation GetInversePermutation() {
            var map = Map.ToDictionary(pair => pair.Value, pair => pair.Key);
            return new Permutation(map);
        }
    }
}