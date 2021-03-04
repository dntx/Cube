using System;
using System.Collections.Generic;

namespace sq1code {
    class AState : IComparable<AState> {
        public int Depth { get; }
        public float PredictedCost { get; }
        public AState FromState { get; private set; }
        public Rotation FromRotation { get; private set; }
        public Cube Cube { get; }
        public int CubeId { get; }

        public AState(Cube cube, int cubeId) : this(cube, cubeId, 0, null, null) {
        }

        public AState(Cube cube, int cubeId, float predictedCost, AState fromState, Rotation fromRotation) {
            Depth = (fromState != null) ? fromState.Depth + 1 : 0;
            PredictedCost = predictedCost;
            FromState = fromState;
            FromRotation = fromRotation;
            Cube = cube;
            CubeId = cubeId;
        }

        public void UpdateFrom(AState fromState, Rotation fromRotation) {
            FromState = fromState;
            FromRotation = fromRotation;
        }

        public int CompareTo(AState other) {
            int result = (Depth + PredictedCost).CompareTo(other.Depth + other.PredictedCost);
            if (result != 0) {
                return result;
            }

            return CubeId.CompareTo(other.CubeId);
        }

        public static bool operator == (AState lhs, AState rhs) {
            if (lhs is null || rhs is null) {
                return (lhs is null) && (rhs is null);
            }
            return lhs.Cube == rhs.Cube;
        }

        public static bool operator != (AState lhs, AState rhs) {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return this == (obj as AState);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Cube.GetHashCode();
        }
    }
}