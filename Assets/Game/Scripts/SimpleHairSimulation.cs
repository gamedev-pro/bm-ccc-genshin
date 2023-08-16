using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHairSimulation : MonoBehaviour
{
    [Serializable]
    public class Point
    {
        public Transform Transform;
        public Vector3 Position;
        public Vector3 PrevPosition;
        public float Mass;
    }

    [Serializable]
    public class Constraint
    {
        public Point A;
        public Point B;
        public float Distance;

        public static Constraint New(Point a, Point b, float distance)
        {
            return new Constraint { A = a, B = b, Distance = distance };
        }
    }

    [Header("Physics")] public Transform Root;
    public Vector3 LocalForce = Vector3.zero;
    [Range(0, 1)] public float Drag = 0.1f;
    [Range(0, 0.9f)] public float Elasticity = 0.1f;

    private List<Point> points = new();
    private List<Constraint> constraints = new();

    private void Awake()
    {
        points.Clear();
        constraints.Clear();

        var t = Root;
        while (t != null)
        {
            points.Add(new Point { Transform = t, Position = t.position, PrevPosition = t.position, Mass = 1f });
            t = t.childCount == 1 ? t.GetChild(0) : null;
        }

        for (var i = 0; i < points.Count - 1; i++)
        {
            var a = points[i];
            var b = points[i + 1];
            constraints.Add(Constraint.New(a, b, Vector3.Distance(a.Position, b.Position)));
        }
    }

    private void FixedUpdate()
    {
        Simulate(Time.fixedDeltaTime);

        //sync transforms
        for (var i = 0; i < points.Count; i++)
        {
            points[i].Transform.position = points[i].Position;
        }
    }

    private void Simulate(float dt)
    {
        if (points.Count == 0)
        {
            return;
        }

        var root = points[0];
        root.PrevPosition = root.Position;
        root.Position = root.Transform.position;

        //verlet integration
        for (var i = 1; i < points.Count; i++)
        {
            var p = points[i];
            var force = p.Transform.TransformVector(LocalForce);
            var acc = force / p.Mass;
            var newPos = p.Position + (p.Position - p.PrevPosition) * (1f - Drag) + acc * dt * dt;
            p.PrevPosition = p.Position;
            p.Position = newPos;
        }

        //distance constraint
        for (var i = 0; i < constraints.Count; i++)
        {
            var c = constraints[i];

            var delta = c.A.Position - c.B.Position;
            var deltaLength = delta.magnitude;
            var correction = (c.Distance - deltaLength) / deltaLength;

            var damping = 1.0f - Elasticity;
            damping = Mathf.Max(0.1f, damping);
            c.B.Position -= delta * correction * damping;
        }
    }
}