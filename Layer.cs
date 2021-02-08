using System.Collections.Generic;

namespace sq1code
{
    class Layer {
        public List<int> cells { get; }
        Half left;
        Half right;

        public Layer(Half left, Half right) {
            this.cells = new List<int>();
            this.cells.AddRange(left.cells);
            this.cells.AddRange(right.cells);
            
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            return left.ToString() + "-" + right.ToString();
        }

        public bool SameAs(Layer other) {
            if (cells.Count != other.cells.Count) {
                return false;
            }

            for (int shift = 0; shift < cells.Count; shift++) {
                bool sameAs = true;
                for (int i = 0; i < cells.Count; i++) {
                    int j = (i + shift) % cells.Count;
                    if (cells[i] != other.cells[j]) {
                        sameAs = false;
                        break;
                    }
                }
                if (sameAs) {
                    return true;
                }
            }
            return false;
        }

        public bool isHexagram() {
            return SameAs(Hexagram);
        }

        public bool isSquare() {
            return SameAs(Square);
        }

        public List<Division> GetDivisions(bool dedupUTurn) {
            List<Division> divisions = new List<Division>();

            for (int start = 0; start < cells.Count - 1; start++) {
                for (int end = start + 1; end < cells.Count; end++) {
                    List<int> selected = cells.GetRange(start, end - start);
                    int result = Half.CompareToHalf(selected);
                    if (result < 0) {
                        continue;
                    } else if (result > 0) {
                        break;
                    }

                    List<int> remaining = new List<int>();
                    remaining.AddRange(cells.GetRange(end, cells.Count - end));
                    remaining.AddRange(cells.GetRange(0, start));
                    Division division = new Division(selected, remaining);
                    TryAddDivision(divisions, division, dedupUTurn);

                    if (!dedupUTurn) {
                        Division divisionUTurn = new Division(remaining, selected);
                        TryAddDivision(divisions, divisionUTurn, true);
                    }
                }
            }

            return divisions;
        }

        private void TryAddDivision(List<Division> divisions, Division division, bool dedupUTurn) {
            bool isNew = true;
            foreach (Division existing in divisions) {
                if (existing.SameAs(division)) {
                    isNew = false;
                    break;
                }

                if (dedupUTurn && existing.UTurnAs(division)) {
                    isNew = false;
                    break;
                }
            }
            if (isNew) {
                divisions.Add(division);
            }
        }

        public static Layer Square = new Layer(Half.Square, Half.Square);
        public static Layer Hexagram = new Layer(Half.Hexagram, Half.Hexagram);
    }
}