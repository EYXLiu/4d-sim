using Godot;
using System;

public struct Transformation4
{
	public float [,] m;
	
	public Transformation4(bool identity = false)
	{
		m = new float[5,5];
		for (int i = 0; i < 5; i++) m[i, i] = identity ? 1f : 0f;
	}
	public static Transformation4 Identity => new Transformation4(true);

	public static Transformation4 Translation(float x, float y, float z, float w)
	{
		Transformation4 m = Transformation4.Identity;
		m.m[0,4] = x;
		m.m[1,4] = y;
		m.m[2,4] = z;
		m.m[3,4] = w;
		return m;
	}

	public static Transformation4 Scale(float sx, float sy, float sz, float sw)
	{
		Transformation4 m = Transformation4.Identity;
		m.m[0,0] = sx;
		m.m[1,1] = sy;
		m.m[2,2] = sz;
		m.m[3,3] = sw;
		return m;
	}

	public static Transformation4 RotateXZ(float angle)
	{
		Transformation4 t = Transformation4.Identity;
		float c = Mathf.Cos(angle);
		float s = Mathf.Sin(angle);
		t.m[0,0] = c;
		t.m[0,2] = -s;
		t.m[2,0] = s;
		t.m[2,2] = c;
		return t;
	}
	public static Transformation4 RotateYZ(float angle)
	{
		Transformation4 t = Transformation4.Identity;
		float c = Mathf.Cos(angle);
		float s = Mathf.Sin(angle);
		t.m[1,1] = c;
		t.m[1,2] = -s;
		t.m[2,1] = s;
		t.m[2,2] = c;
		return t;
	}
	public static Transformation4 RotateXW(float angle)
	{
		Transformation4 t = Transformation4.Identity;
		float c = Mathf.Cos(angle);
		float s = Mathf.Sin(angle);
		t.m[0,0] = c;
		t.m[0,3] = -s;
		t.m[3,0] = s;
		t.m[3,3] = c;
		return t;
	}
	public static Transformation4 RotateZW(float angle)
	{
		Transformation4 t = Transformation4.Identity;
		float c = Mathf.Cos(angle);
		float s = Mathf.Sin(angle);
		t.m[2,2] = c;
		t.m[2,3] = -s;
		t.m[3,2] = s;
		t.m[3,3] = c;
		return t;
	}
	
	public static Transformation4 Invert(Transformation4 m)
	{
		Transformation4 inv = Transformation4.Identity;

		 float[,] Rinv = Transformation.Invert(m);

		for (int i = 0; i < 4; i++)
			for (int j = 0; j < 4; j++)
				inv.m[i,j] = Rinv[i,j];

		for (int i = 0; i < 4; i++)
		{
			float t = 0;
			for (int j = 0; j < 4; j++)
				t += -inv.m[i,j] * m.m[j,4];
			inv.m[i,4] = t;
		}
		return inv;
	}

	public static Transformation4 operator *(Transformation4 a, Transformation4 b)
	{
		Transformation4 t = Transformation4.Identity;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				t.m[i, j] = 0f;
				for (int k = 0; k < 5; k++)
				{
					t.m[i, j] += a.m[i, k] * b.m[k, j];
				}
			}
		}
		return t;
	}
}
