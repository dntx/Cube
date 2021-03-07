namespace sq1code
{
    public enum Goal { 
        SolveShape,

        // L1 strategy
        SolveL1Quarter123,
        SolveL1Quarter4,

        // L3 strategy 1
        SolveL3Cross,
        SolveL3CornersThen,

        // L3 strategy 2
        SolveL3Corners,
        SolveL3CrossThen,

        // L3 strategy 3
        SolveL3Cell01,
        SolveL3Cell2,
        SolveL3Cell3,

        // L3 strategy 3.1
        SolveL3Cell46,
        SolveL3Cell57Then,

        // L3 strategy 3.2
        SolveL3Cell57,
        SolveL3Cell46Then,

        Scratch
    };
}