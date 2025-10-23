using Godot;
using System;

public struct Transformation
{
	public static float[,] Invert(Transformation4 m)
	{
		int n = 4;
		float[,] a = new float[n,n];
		float[,] inv = Identity4x4();

		for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
				a[i,j] = m.m[i,j];

		for (int i = 0; i < n; i++)
		{
			int pivot = i;
			for (int j = i+1; j < n; j++)
				if (Math.Abs(a[j,i]) > Math.Abs(a[pivot,i]))
					pivot = j;

			if (pivot != i)
			{
				for (int k = 0; k < n; k++)
				{
					float tmp = a[i,k]; a[i,k] = a[pivot,k]; a[pivot,k] = tmp;
					tmp = inv[i,k]; inv[i,k] = inv[pivot,k]; inv[pivot,k] = tmp;
				}
			}

			float scale = a[i,i];
			if (scale == 0f) throw new Exception("Matrix not invertible");
			for (int k = 0; k < n; k++)
			{
				a[i,k] /= scale;
				inv[i,k] /= scale;
			}

			for (int j = 0; j < n; j++)
			{
				if (j == i) continue;
				float factor = a[j,i];
				for (int k = 0; k < n; k++)
				{
					a[j,k] -= factor * a[i,k];
					inv[j,k] -= factor * inv[i,k];
				}
			}
		}
		return inv;
	}
	public static float[,] Identity4x4()
	{
		float[,] id = new float[4,4];
		for (int i = 0; i < 4; i++)
			id[i,i] = 1f;
		return id;
	}
}
