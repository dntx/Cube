using System;

namespace sq1code
{
    class Cell {
        public enum Type { 
            IgnoreColor, 
            IgnoreSideColor,

            KeepAllCells, 
            KeepCell_012345, 
            KeepL3Cells, 
            
            IgnoreCell_0246,
            IgnoreCell_234567,
            IgnoreCell_34567,
            IgnoreCell_4567,
            IgnoreCell_567
        };

        public int Value { get; }
        public int Degree { get; }
        public int Color { get; }
        public int Layer { get; }
        public int SideColor { get; }
        public int LeftSideColor { get; }
        public int RightSideColor { get; }

        public Cell(int cell, Type type) {
            switch (type) {
                case Type.IgnoreColor:
                    Value = GetShape(cell);
                    break;
                case Type.IgnoreSideColor:
                    Value = GetShape(cell) + GetColor(cell) * 8;
                    break;

                case Type.KeepAllCells:
                    Value = cell;
                    break;
                case Type.KeepCell_012345:
                    Value = (cell < 6)? cell : GetShape(cell) + 8;
                    break;
                case Type.KeepL3Cells:
                    Value = (cell < 8)? cell : GetShape(cell) + 8;
                    break;

                case Type.IgnoreCell_0246:
                    Value = (cell == 0 || cell == 2 || cell == 4 || cell == 6) ? GetShape(cell) + 0 : cell;
                    break;
                case Type.IgnoreCell_234567:
                    Value = (2 <= cell && cell <= 7) ? GetShape(cell) + 6 : cell;
                    break;
                case Type.IgnoreCell_34567:
                    Value = (3 <= cell && cell <= 7) ? GetShape(cell) + 6 : cell;
                    break;
                case Type.IgnoreCell_4567:
                    Value = (4 <= cell && cell <= 7) ? GetShape(cell) + 6 : cell;
                    break;
                case Type.IgnoreCell_567:
                    Value = (5 <= cell && cell <= 7) ? GetShape(cell) + 6 : cell;
                    break;
            }
            
            Degree = GetDegree(Value);
            Color = GetColor(Value);
            Layer = GetLayer(Value);
            SideColor = GetSideColor(Value);
            LeftSideColor = GetLeftSideColor(Value);
            RightSideColor = GetRightSideColor(Value);
        }

        public override string ToString()
        {
            return string.Format("{0:X}", Value);
        }

        public static bool operator == (Cell lhs, Cell rhs) {
            if (lhs is null || rhs is null) {
                return (lhs is null) && (rhs is null);
            }
            return lhs.Value == rhs.Value;
        }

        public static bool operator != (Cell lhs, Cell rhs) {
            return !(lhs == rhs);
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

        private static int GetShape(int cell) {
            return cell % 2;
        }

        private static int GetColor(int cell) {
            // 0,1,2,3,4,5,6,7: color=0, yellow, L3
            // 8,9,A,B,C,D,E,F: color=1, white, L1
            return cell / 8;
        }

        private static int GetLayer(int cell) {
            // 0,1,2,3,4,5,6,7: color=0, yellow, L3
            // 8,9,A,B,C,D,E,F: color=1, white, L1
            return (cell / 8 == 0)? 3 : 1;
        }

        private static int GetSideColor(int cell) {
            if (GetDegree(cell) != 30) {
                return -1;
            }

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

        private static int GetLeftSideColor(int cell) {
            if (GetDegree(cell) != 60) {
                return -1;
            }

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

        private static int GetRightSideColor(int cell) {
            if (GetDegree(cell) != 60) {
                return -1;
            }

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