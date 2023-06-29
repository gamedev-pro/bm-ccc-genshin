using System.Linq;
using UnityEngine;

public class ClothSimulation : MonoBehaviour
{
    //
    public class Point
    {
        public Vector2 Position;
        public Vector2 PrevPosition;
        public float Mass;
    }

    public struct Constraint
    {
        public Point A;
        public Point B;
        public float Distance;

        public static Constraint New(Point a, Point b, float distance)
        {
            return new Constraint { A = a, B = b, Distance = distance };
        }
    }

    [Header("Physics")] public float Distance = 1;
    public float Mass = 1;
    public Vector2 Force = Vector2.down;
    [Range(0, 1)] public float Drag = 0.1f;
    [Range(0, 0.9f)] public float Elasticity = 0.1f;

    public float GroundY;


    [Header("Graphics")] public Color Color = Color.red;

    public float Radius = 0.1f;

    private Point[] Points;
    private Constraint[] Constraints;

    private void Awake()
    {
        var pos = (Vector2)transform.position;
        var positions = new[]
        {
            pos + new Vector2(-1, 1) * Distance * 0.5f, //top left
            pos + new Vector2(1, 1) * Distance * 0.5f, //top right
            pos + new Vector2(-1, -1) * Distance * 0.5f, //bottom left
            pos + new Vector2(1, -1) * Distance * 0.5f //bottom right
        };
        Points = positions.Select(p => new Point { Position = p, PrevPosition = p, Mass = Mass }).ToArray();

        Constraints = new[]
        {
            Constraint.New(Points[0], Points[1], Distance),
            Constraint.New(Points[0], Points[2], Distance),
            Constraint.New(Points[1], Points[3], Distance),
            Constraint.New(Points[2], Points[3], Distance),
        };
    }

    private void FixedUpdate()
    {
        Simulate(Time.fixedDeltaTime);
    }

    private void Simulate(float dt)
    {
        //verlet integration
        for (var i = 0; i < Points.Length; i++)
        {
            var p = Points[i];
            var acc = Force / p.Mass;

            var newPos = p.Position + (p.Position - p.PrevPosition) * (1f - Drag) + acc * dt * dt;
            p.PrevPosition = p.Position;
            p.Position = newPos;
        }

        //fake ground collision
        for (var i = 0; i < Points.Length; i++)
        {
            var p = Points[i];
            if (p.Position.y < GroundY)
            {
                p.Position.y = GroundY;
            }
        }

        //keep distances
        for (int i = 0; i < Constraints.Length; i++)
        {
            var c = Constraints[i];

            var delta = c.A.Position - c.B.Position;
            var deltaLength = delta.magnitude;
            var correction = 0.5f * (c.Distance - deltaLength) / deltaLength;

            var damping = 1.0f - Elasticity;
            damping = Mathf.Max(0.1f, damping);
            c.A.Position += delta * correction * damping;
            c.B.Position -= delta * correction * damping;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(Vector3.up * GroundY - Vector3.right * 1000, Vector3.up * GroundY + Vector3.right * 1000);
        if (Points != null && Points.Length > 0)
        {
            Gizmos.color = Color;
            for (int i = 0; i < Points.Length; i++)
            {
                var p = Points[i];
                Gizmos.DrawSphere(p.Position, Radius);
            }

            for (int i = 0; i < Constraints.Length; i++)
            {
                var c = Constraints[i];
                Gizmos.DrawLine(c.A.Position, c.B.Position);
            }
        }
    }
}