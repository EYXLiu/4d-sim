using Godot;
using System;

public struct Vector4
{
	public float X, Y, Z, W;

	public Vector4(float x, float y, float z, float w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	public static Vector4 operator +(Vector4 a, Vector4 b)
		=> new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

	public static Vector4 operator -(Vector4 a, Vector4 b)
		=> new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

	public static Vector4 operator *(Vector4 a, float s)
		=> new Vector4(a.X * s, a.Y * s, a.Z * s, a.W * s);

	public float Dot(Vector4 b)
		=> X * b.X + Y * b.Y + Z * b.Z + W * b.W;

	public float Length()
		=> Mathf.Sqrt(X * X + Y * Y + Z * Z + W * W);

	public Vector4 Normalized()
	{
		float len = Length();
		return len > 0 ? this * (1f / len) : this;
	}

	public static Vector4 Multiply(Transformation4 m, Vector4 v)
	{
		return new Vector4(
			m.m[0,0]*v.X + m.m[0,1]*v.Y + m.m[0,2]*v.Z + m.m[0,3]*v.W + m.m[0,4],
			m.m[1,0]*v.X + m.m[1,1]*v.Y + m.m[1,2]*v.Z + m.m[1,3]*v.W + m.m[1,4],
			m.m[2,0]*v.X + m.m[2,1]*v.Y + m.m[2,2]*v.Z + m.m[2,3]*v.W + m.m[2,4],
			m.m[3,0]*v.X + m.m[3,1]*v.Y + m.m[3,2]*v.Z + m.m[3,3]*v.W + m.m[3,4]
		);
	}

	public static Vector4 operator *(Transformation4 m, Vector4 v) => Multiply(m, v);
}
