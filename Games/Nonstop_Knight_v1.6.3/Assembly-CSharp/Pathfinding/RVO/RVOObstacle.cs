﻿namespace Pathfinding.RVO
{
    using Pathfinding;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class RVOObstacle : MonoBehaviour
    {
        private ObstacleVertexWinding _obstacleMode;
        private List<ObstacleVertex> addedObstacles;
        private bool gizmoDrawing;
        private List<Vector3[]> gizmoVerts;
        public RVOLayer layer = RVOLayer.DefaultObstacle;
        public ObstacleVertexWinding obstacleMode;
        private Matrix4x4 prevUpdateMatrix;
        protected Simulator sim;
        private List<Vector3[]> sourceObstacles;

        protected RVOObstacle()
        {
        }

        protected void AddObstacle(Vector3[] vertices, float height)
        {
            if (vertices == null)
            {
                throw new ArgumentNullException("Vertices Must Not Be Null");
            }
            if (height < 0f)
            {
                throw new ArgumentOutOfRangeException("Height must be non-negative");
            }
            if (vertices.Length < 2)
            {
                throw new ArgumentException("An obstacle must have at least two vertices");
            }
            if (this.gizmoDrawing)
            {
                Vector3[] destinationArray = new Vector3[vertices.Length];
                this.WindCorrectly(vertices);
                Array.Copy(vertices, destinationArray, vertices.Length);
                this.gizmoVerts.Add(destinationArray);
            }
            else
            {
                if (this.sim == null)
                {
                    this.FindSimulator();
                }
                if (vertices.Length == 2)
                {
                    this.AddObstacleInternal(vertices, height);
                }
                else
                {
                    this.WindCorrectly(vertices);
                    this.AddObstacleInternal(vertices, height);
                }
            }
        }

        private void AddObstacleInternal(Vector3[] vertices, float height)
        {
            this.addedObstacles.Add(this.sim.AddObstacle(vertices, height, this.GetMatrix(), this.layer));
            this.sourceObstacles.Add(vertices);
        }

        protected abstract bool AreGizmosDirty();
        protected abstract void CreateObstacles();
        protected void FindSimulator()
        {
            RVOSimulator simulator = UnityEngine.Object.FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
            if (simulator == null)
            {
                throw new InvalidOperationException("No RVOSimulator could be found in the scene. Please add one to any GameObject");
            }
            this.sim = simulator.GetSimulator();
        }

        protected virtual Matrix4x4 GetMatrix()
        {
            return (!this.LocalCoordinates ? Matrix4x4.identity : base.transform.localToWorldMatrix);
        }

        public void OnDisable()
        {
            if (this.addedObstacles != null)
            {
                if (this.sim == null)
                {
                    throw new Exception("This should not happen! Make sure you are not overriding the OnEnable function");
                }
                for (int i = 0; i < this.addedObstacles.Count; i++)
                {
                    this.sim.RemoveObstacle(this.addedObstacles[i]);
                }
            }
        }

        public void OnDrawGizmos()
        {
            this.OnDrawGizmos(false);
        }

        public void OnDrawGizmos(bool selected)
        {
            this.gizmoDrawing = true;
            Gizmos.color = new Color(0.615f, 1f, 0.06f, !selected ? 0.7f : 1f);
            if (((this.gizmoVerts == null) || this.AreGizmosDirty()) || (this._obstacleMode != this.obstacleMode))
            {
                this._obstacleMode = this.obstacleMode;
                if (this.gizmoVerts == null)
                {
                    this.gizmoVerts = new List<Vector3[]>();
                }
                else
                {
                    this.gizmoVerts.Clear();
                }
                this.CreateObstacles();
            }
            Matrix4x4 matrix = this.GetMatrix();
            for (int i = 0; i < this.gizmoVerts.Count; i++)
            {
                Vector3[] vectorArray = this.gizmoVerts[i];
                int index = 0;
                for (int j = vectorArray.Length - 1; index < vectorArray.Length; j = index++)
                {
                    Vector3 from = matrix.MultiplyPoint3x4(vectorArray[index]);
                    Gizmos.DrawLine(from, matrix.MultiplyPoint3x4(vectorArray[j]));
                }
                if (selected)
                {
                    int num4 = 0;
                    for (int k = vectorArray.Length - 1; num4 < vectorArray.Length; k = num4++)
                    {
                        Gizmos.DrawLine(matrix.MultiplyPoint3x4(vectorArray[num4]) + ((Vector3) (Vector3.up * this.Height)), matrix.MultiplyPoint3x4(vectorArray[k]) + ((Vector3) (Vector3.up * this.Height)));
                        Vector3 introduced16 = matrix.MultiplyPoint3x4(vectorArray[num4]);
                        Gizmos.DrawLine(introduced16, matrix.MultiplyPoint3x4(vectorArray[num4]) + ((Vector3) (Vector3.up * this.Height)));
                    }
                    int num6 = 0;
                    for (int m = vectorArray.Length - 1; num6 < vectorArray.Length; m = num6++)
                    {
                        Vector3 vector = matrix.MultiplyPoint3x4(vectorArray[m]);
                        Vector3 vector2 = matrix.MultiplyPoint3x4(vectorArray[num6]);
                        Vector3 vector3 = (Vector3) ((vector + vector2) * 0.5f);
                        Vector3 vector6 = vector2 - vector;
                        Vector3 normalized = vector6.normalized;
                        if (normalized != Vector3.zero)
                        {
                            Vector3 vector5 = Vector3.Cross(Vector3.up, normalized);
                            Gizmos.DrawLine(vector3, vector3 + vector5);
                            Gizmos.DrawLine(vector3 + vector5, (Vector3) ((vector3 + (vector5 * 0.5f)) + (normalized * 0.5f)));
                            Gizmos.DrawLine(vector3 + vector5, (Vector3) ((vector3 + (vector5 * 0.5f)) - (normalized * 0.5f)));
                        }
                    }
                }
            }
            this.gizmoDrawing = false;
        }

        public void OnDrawGizmosSelected()
        {
            this.OnDrawGizmos(true);
        }

        public void OnEnable()
        {
            if (this.addedObstacles != null)
            {
                if (this.sim == null)
                {
                    throw new Exception("This should not happen! Make sure you are not overriding the OnDisable function");
                }
                for (int i = 0; i < this.addedObstacles.Count; i++)
                {
                    ObstacleVertex next = this.addedObstacles[i];
                    ObstacleVertex vertex2 = next;
                    do
                    {
                        next.layer = this.layer;
                        next = next.next;
                    }
                    while (next != vertex2);
                    this.sim.AddObstacle(this.addedObstacles[i]);
                }
            }
        }

        public void Start()
        {
            this.addedObstacles = new List<ObstacleVertex>();
            this.sourceObstacles = new List<Vector3[]>();
            this.prevUpdateMatrix = this.GetMatrix();
            this.CreateObstacles();
        }

        public void Update()
        {
            Matrix4x4 matrix = this.GetMatrix();
            if (matrix != this.prevUpdateMatrix)
            {
                for (int i = 0; i < this.addedObstacles.Count; i++)
                {
                    this.sim.UpdateObstacle(this.addedObstacles[i], this.sourceObstacles[i], matrix);
                }
                this.prevUpdateMatrix = matrix;
            }
        }

        private void WindCorrectly(Vector3[] vertices)
        {
            int index = 0;
            float positiveInfinity = float.PositiveInfinity;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].x < positiveInfinity)
                {
                    index = i;
                    positiveInfinity = vertices[i].x;
                }
            }
            if (VectorMath.IsClockwiseXZ(vertices[((index - 1) + vertices.Length) % vertices.Length], vertices[index], vertices[(index + 1) % vertices.Length]))
            {
                if (this.obstacleMode == ObstacleVertexWinding.KeepOut)
                {
                    Array.Reverse(vertices);
                }
            }
            else if (this.obstacleMode == ObstacleVertexWinding.KeepIn)
            {
                Array.Reverse(vertices);
            }
        }

        protected abstract bool ExecuteInEditor { get; }

        protected abstract float Height { get; }

        protected abstract bool LocalCoordinates { get; }

        protected abstract bool StaticObstacle { get; }

        public enum ObstacleVertexWinding
        {
            KeepOut,
            KeepIn
        }
    }
}

