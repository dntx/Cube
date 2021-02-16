using System;

namespace sq1code
{
    class Cell {
        public int Value { get; }
        public int Degree { get; }
        public int Color { get; }
        public int SideColor { get; }

        public Cell(int cell, int colorCount) {
            if (colorCount != 1 && colorCount != 2 && colorCount != 6) {
                throw new ArgumentException("color count should be 1, 2, or 6");
            }

            if (colorCount == 1) {
                Value = GetDegree(cell) / 30;
            } else if (colorCount == 2) {
                Value = GetDegree(cell) / 30 + GetColor(cell) * 8;
            } else {
                Value = cell;
            }

            Degree = GetDegree(Value);
            Color = GetColor(Value);
            SideColor = GetSideColor(Value);
        }

        public override string ToString()
        {
            return string.Format("{0:X}", Value);
        }

        public static bool operator == (Cell lhs, Cell rhs) {
            return lhs.Value == rhs.Value;
        }

        public static bool operator != (Cell lhs, Cell rhs) {
            return lhs.Value != rhs.Value;
        }

        public static bool operator < (Cell lhs, Cell rhs) {
            return lhs.Value < rhs.Value;
        }

        public static bool operator > (Cell lhs, Cell rhs) {
            return lhs.Value > rhs.Value;
        }

        public static bool operator <= (Cell lhs, Cell rhs) {
            return lhs.Value <= rhs.Value;
        }

        public static bool operator >= (Cell lhs, Cell rhs) {
            return lhs.Value >= rhs.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this.Value == (obj as Cell).Value;
        }
        
        public override int GetHashCode()
        {
            return Value;
        }
    
        private static int GetDegree(int cell) {
            return (cell % 2 == 1) ? 30 : 60;
        }

        private static int GetColor(int cell) {
            // 0,1,2,3,4,5,6,7: color=0, yellow
            // 8,9,A,B,C,D,E,F: color=1, white
            return cell / 8;
        }

        private static int GetSideColor(int cell) {
            // 7,0: color=0, red
            // 1,2: color=1, blue
            // 3,4: color=2, orange
            // 5,6: color=3, green

            // F,8: color=0, red
            // 9,A: color=1, green
            // B,C: color=2, orange
            // D,E: color=3, blue
            return (cell + 1) % 8 / 2;
        }
    }
}