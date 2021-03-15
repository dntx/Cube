using System;

namespace Cube.Sq1BitCube
{
    class Cell {
        public static uint GetOrder(uint cell) {
            return cell % 8;
        }

        public static uint GetDegree(uint cell) {
            return (cell % 2 == 1) ? 30 : 60;
        }

        public static uint GetShape(uint cell) {
            return cell % 2;
        }

        public static uint GetColor(uint cell) {
            // 0,1,2,3,4,5,6,7: color=0, yellow, L3
            // 8,9,A,B,C,D,E,F: color=1, white, L1
            return cell / 8;
        }

        public static uint GetLayer(uint cell) {
            // 0,1,2,3,4,5,6,7: color=0, yellow, L3
            // 8,9,A,B,C,D,E,F: color=1, white, L1
            return (cell / 8 == 0)? 3 : 1;
        }

        public static uint GetSideColor(uint cell) {
            // 1: color=0, blue
            // 3: color=1, orange
            // 5: color=2, green
            // 7: color=3, red

            // 9: color=4, green
            // B: color=5, orange
            // D: color=6, blue
            // F: color=7, red
            return (cell - 1) % 8 / 2;
        }

        public static uint GetLeftSideColor(uint cell) {
            // 0: color=0, blue
            // 2: color=1, orange
            // 4: color=2, green
            // 6: color=3, red

            // 8: color=0, green
            // A: color=1, orange
            // C: color=2, blue
            // E: color=3, red
            return cell % 8 / 2;
        }

        public static uint GetRightSideColor(uint cell) {
            // 2: color=0, blue
            // 4: color=1, orange
            // 6: color=2, green
            // 0: color=3, red

            // A: color=0, green
            // C: color=1, orange
            // E: color=2, blue
            // 8: color=3, red
            return (cell + 6) % 8 / 2;
        }
    }
}