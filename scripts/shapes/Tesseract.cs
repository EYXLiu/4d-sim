using Godot;
using System;
using System.Collections.Generic;

public partial class Tesseract : Shape4d
{	
	protected override void InitVertices()
	{
		vertices = new Vector4[16];
		int i = 0;
		for (int x = -1; x <= 1; x += 2)
		for (int y = -1; y <= 1; y += 2)
		for (int z = -1; z <= 1; z += 2)
		for (int w = -1; w <= 1; w += 2)
			vertices[i++] = new Vector4(x * 2f, y * 2f, z * 2f, w * 2f);
	}

	protected override void InitEdges()
	{
		List<int[]> edgeList = new List<int[]>();
		for (int i = 0; i < vertices.Length; i++)
		{
			for (int j = i + 1; j < vertices.Length; j++)
			{
				int diffCount = 0;
				if (vertices[i].X != vertices[j].X) diffCount++;
				if (vertices[i].Y != vertices[j].Y) diffCount++;
				if (vertices[i].Z != vertices[j].Z) diffCount++;
				if (vertices[i].W != vertices[j].W) diffCount++;
				if (diffCount == 1)
				{
					edgeList.Add(new int[]{ i, j });
				}
			}
		}
		edges = new int[edgeList.Count, 2];
		for (int k = 0; k < edgeList.Count; k++)
		{
			edges[k, 0] = edgeList[k][0];
			edges[k, 1] = edgeList[k][1];
		}
	}

	protected List<Vector2> ComputeSlice()
	{
		List<Vector2> slicePoints = new List<Vector2>();

		for (int i = 0; i < edges.GetLength(0); i++)
		{
			Vector4 v1 = vertices[edges[i,0]];
			Vector4 v2 = vertices[edges[i,1]];

			float dy = v2.Y - v1.Y;
			float dw = v2.W - v1.W;

			if (Math.Abs(dy) > 0 && Math.Abs(dw) > 0)
			{
				float tY = (0f - v1.Y) / dy;
				float tW = (0f - v1.W) / dw;

				if (Mathf.Abs(tY - tW) < 0.01f && tY >= 0f && tY <= 1f)
				{
					float t = tY;
					float x = v1.X + t * (v2.X - v1.X);
					float z = v1.Z + t * (v2.Z - v1.Z);
					slicePoints.Add(new Vector2(x, z));
					slicePoints.Add(new Vector2(x, z));
				}
			}
		}
		return slicePoints;
	}
}
