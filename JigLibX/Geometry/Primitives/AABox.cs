#region Using Statements

using Microsoft.Xna.Framework;
using JigLibX.Math;

#endregion

namespace JigLibX.Geometry.Primitives
{

    /// <summary>
    /// An axis aligned Box.
    /// </summary>
    public class AABox : Primitive
    {

        private Vector3 minPos = new Vector3(float.MaxValue);
        private Vector3 maxPos = new Vector3(float.MinValue);

        private static AABox hugeBox = new AABox(
            new Vector3(float.MinValue),new Vector3(float.MaxValue));

        /// <summary>
        /// Position based on one corner. sideLengths are the full side
        /// lengths (each element must be >= 0)
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        public AABox(Vector3 minPos, Vector3 maxPos) :
            base((int)PrimitiveType.AABox)
        {
            this.minPos = minPos;
            this.maxPos = maxPos;
        }

        public AABox() : 
            base((int)PrimitiveType.AABox)
        {
           Clear();
        }

        private Vector3 offset = Vector3.Zero;
        public override Transform Transform
        {
            get
            {
                return new Transform(offset, Matrix.Identity);
            }
            set
            {
                maxPos = maxPos - offset + value.Position;
                minPos = minPos - offset + value.Position;
                offset = value.Position;
            }
        }

        // adding points etc.
        public void Clear()
        {
            minPos.X = minPos.Y = minPos.Z = float.MaxValue;
            maxPos.X = maxPos.Y = maxPos.Z = float.MinValue;
        }

        /// <summary>
        /// Adds a point to the AABB.
        /// </summary>
        /// <remarks>
        /// This function is heavily used to calculate the axis
        /// aligned bounding boxes arround any object. Calling 
        /// by reference is unusal but makes it faster.
        /// </remarks> 
        /// <param name="pos"></param>
        public void AddPoint(ref Vector3 pos)
        {
            if (pos.X < minPos.X) minPos.X = pos.X - JiggleMath.Epsilon;
            if (pos.X > maxPos.X) maxPos.X = pos.X + JiggleMath.Epsilon;

            if (pos.Y < minPos.Y) minPos.Y = pos.Y - JiggleMath.Epsilon;
            if (pos.Y > maxPos.Y) maxPos.Y = pos.Y + JiggleMath.Epsilon;

            if (pos.Z < minPos.Z) minPos.Z = pos.Z - JiggleMath.Epsilon;
            if (pos.Z > maxPos.Z) maxPos.Z = pos.Z + JiggleMath.Epsilon;
        }

        public void AddPoint(Vector3 pos)
        {
            if (pos.X < minPos.X) minPos.X = pos.X - JiggleMath.Epsilon;
            if (pos.X > maxPos.X) maxPos.X = pos.X + JiggleMath.Epsilon;

            if (pos.Y < minPos.Y) minPos.Y = pos.Y - JiggleMath.Epsilon;
            if (pos.Y > maxPos.Y) maxPos.Y = pos.Y + JiggleMath.Epsilon;

            if (pos.Z < minPos.Z) minPos.Z = pos.Z - JiggleMath.Epsilon;
            if (pos.Z > maxPos.Z) maxPos.Z = pos.Z + JiggleMath.Epsilon;
        }

        public void AddBox(Box box)
        {
            //var pts = new Vector3[8];
            Vector3[] pts;
            box.GetCornerPoints(out pts);

            AddPoint(ref pts[0]);
            AddPoint(ref pts[1]);
            AddPoint(ref pts[2]);
            AddPoint(ref pts[3]);
            AddPoint(ref pts[4]);
            AddPoint(ref pts[5]);
            AddPoint(ref pts[6]);
            AddPoint(ref pts[7]);
        }

        public void AddSegment(Segment seg)
        {
            AddPoint(seg.Origin);
            AddPoint(seg.GetEnd());
        }

        public void AddAABox(AABox aabox)
        {
            AddPoint(aabox.MaxPos);
            AddPoint(aabox.MinPos);
        }

        public void AddSphere(Sphere sphere)
        {
            if ((sphere.Position.X - sphere.Radius) < minPos.X)
                minPos.X = (sphere.Position.X - sphere.Radius) - JiggleMath.Epsilon;
            if ((sphere.Position.X + sphere.Radius) > maxPos.X)
                maxPos.X = (sphere.Position.X + sphere.Radius) + JiggleMath.Epsilon;

            if ((sphere.Position.Y - sphere.Radius) < minPos.Y)
                minPos.Y = (sphere.Position.Y - sphere.Radius) - JiggleMath.Epsilon;
            if ((sphere.Position.Y + sphere.Radius) > maxPos.Y)
                maxPos.Y = (sphere.Position.Y + sphere.Radius) + JiggleMath.Epsilon;

            if ((sphere.Position.Z - sphere.Radius) < minPos.Z)
                minPos.Z = (sphere.Position.Z - sphere.Radius) - JiggleMath.Epsilon;
            if ((sphere.Position.Z + sphere.Radius) > maxPos.Z)
                maxPos.Z = (sphere.Position.Z + sphere.Radius) + JiggleMath.Epsilon;
        }

        public void AddCapsule(Capsule capsule)
        {
            AddSphere(new Sphere(capsule.Position, capsule.Radius));
            AddSphere(new Sphere(capsule.Position + capsule.Length * capsule.Orientation.Backward, capsule.Radius));
        }

        public void AddPrimitive(Primitive prim)
        {
            switch ((PrimitiveType)prim.Type)
            {
                case PrimitiveType.Box:
                    AddBox((Box)prim);
                    break;
                case PrimitiveType.Sphere:
                    AddSphere((Sphere)prim);
                    break;
                case PrimitiveType.Capsule:
                    AddCapsule((Capsule)prim);
                    break;
                default:
                    AddAABox(prim.GetBoundingBox());
                    break;
            }
        }

        public void Move(Vector3 delta)
        {
            minPos += delta;
            maxPos += delta;
        }

        public bool IsPointInside(Vector3 pos)
        {
            return ((pos.X >= minPos.X) &&
                (pos.X <= maxPos.X) &&
                (pos.Y >= minPos.Y) &&
                (pos.Y <= maxPos.Y) &&
                (pos.Z >= minPos.Z) &&
                (pos.Z <= maxPos.Z));
        }

        public static bool OverlapTest(AABox box0, AABox box1)
        {
            return ((box0.minPos.Z >= box1.maxPos.Z) ||
                (box0.maxPos.Z <= box1.minPos.Z) ||
                (box0.minPos.Y >= box1.maxPos.Y) ||
                (box0.maxPos.Y <= box1.minPos.Y) ||
                (box0.minPos.X >= box1.maxPos.X) ||
                (box0.maxPos.X <= box1.minPos.X)) ? false : true;
        }

        public static bool OverlapTest(AABox box0, AABox box1, float tol)
        {
            return ((box0.minPos.Z >= box1.maxPos.Z + tol) ||
                (box0.maxPos.Z <= box1.minPos.Z - tol) ||
                (box0.minPos.Y >= box1.maxPos.Y + tol) ||
                (box0.maxPos.Y <= box1.minPos.Y - tol) ||
                (box0.minPos.X >= box1.maxPos.X + tol) ||
                (box0.maxPos.X <= box1.minPos.X - tol)) ? false : true;
        }

        public Vector3 GetCentre()
        {
            return 0.5f * (minPos + maxPos);
        }

        public Vector3 MinPos
        {
            get { return minPos; }
            set { minPos = value; }
        }

        public Vector3 MaxPos
        {
            get { return maxPos; }
            set { maxPos = value; }
        }

        public Vector3 GetSideLengths()
        {
            return maxPos - minPos;
        }

        public float GetRadiusAboutCentre()
        {
            return 0.5f * (maxPos - minPos).Length();
        }

        public float GetRadiusSqAboutCentre()
        {
            float result = GetRadiusAboutCentre();
            return result * result;
        }

        public static AABox HugeBox
        {
            get { return hugeBox; }
        }

        public override Primitive Clone()
        {
            return new AABox(minPos, maxPos);
        }

        public override bool SegmentIntersect(out float frac, out Vector3 pos, out Vector3 normal, Segment seg)
        {
            // todo implement
            throw new JigLibXException("Not implemented!");
        }

        public override float GetVolume()
        {
            return (maxPos - minPos).LengthSquared();
        }

        public override float GetSurfaceArea()
        {
            Vector3 sl = maxPos - minPos;
            return 2.0f * (sl.X * sl.Y + sl.X * sl.Z + sl.Y * sl.Z);
        }

        public override void GetMassProperties(PrimitiveProperties primitiveProperties, out float mass, out Vector3 centerOfMass, out Matrix inertiaTensor)
        {
            mass = 0.0f;
            centerOfMass = Vector3.Zero;
            inertiaTensor = Matrix.Identity;
        }
    }
}
