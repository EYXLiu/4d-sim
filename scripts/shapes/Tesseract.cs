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
			vertices[i++] = new Vector4(x * scale, y * scale, z * scale, w * scale);
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
	protected override void InitFaces()
	{
		(List<Vector4> vertices4D, List<int[]> faces) = Python.RunMeshScript(vertices);
	}
	
	//protected override void Outline() {}
	
	protected override void UpdateMesh()
	{
		List<Vector3> verts3d = Visuals.SliceVertices(vertices);
		
		Vector3 center = Vector3.Zero;
	}
}
