using System;
using System.Collections.Generic;

namespace Cube {
    class AState : IComparable<AState> {
        public int Depth { get; private set; }
        public int PredictedCost { get; }
        public AState FromState { get; private set; }
        public IRotation FromRotation { get; private set; }
        public ICube StartCube { get; }
        public ICube Cube { get; }
        public int CubeId { get; }
        public bool IsClosed { get; set; }

        public AState(ICube cube, int cubeId) 
                : this(cube, cubeId, predictedCost:0) {}

        public AState(ICube cube, int cubeId, int predictedCost) 
                : this(startCube:null, cube, cubeId, predictedCost) {}

        public AState(ICube startCube, ICube cube, int cubeId, int predictedCost) 
                : this(startCube, cube, cubeId, predictedCost, fromState:null, fromRotation:null) {}

        public AState(ICube cube, int cubeId, int predictedCost, AState fromState, IRotation fromRotation) 
                : this(startCube:null, cube, cubeId, predictedCost, fromState, fromRotation) {}

        public AState(ICube startCube, ICube cube, int cubeId, int predictedCost, AState fromState, IRotation fromRotation) {
            StartCube = startCube;
            Depth = (fromState != null) ? fromState.Depth + 1 : 0;
            PredictedCost = predictedCost;
            FromState = fromState;
            FromRotation = fromRotation;
            Cube = cube;
            CubeId = cubeId;
            IsClosed = false;
        }

        public void UpdateFrom(AState fromState, IRotation fromRotation) {
            Depth = fromState.Depth + 1;
            FromState = fromState;
            FromRotation = fromRotation;
        }

        public int CompareTo(AState other) {
            int result = (Depth + PredictedCost).CompareTo(other.Depth + other.PredictedCost);
            if (result != 0) {
                return result;
            }

            if (Depth != other.Depth) {
                // prefer deeper cube / new cube
                return -Depth.CompareTo(other.Depth);
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