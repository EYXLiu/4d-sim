using Godot;
using System;
using System.Collections.Generic;

public partial class Tesseract : Node3D
{
	private Vector4[] vertices;
	private int[,] edges;
	
	private float rotationSpeed = 0.01f;
	private Vector2 lastMousePos;
	private bool leftMouseDown = false;
	private bool rightMouseDown = false;
	
	private ImmediateMesh mesh;
	private MeshInstance3D meshInstance;
	
	public override void _Ready()
	{
		InitVertices();
		InitEdges();
		
		lastMousePos = GetViewport().GetMousePosition();
		
		meshInstance = new MeshInstance3D();
		mesh = new ImmediateMesh();
		meshInstance.Mesh = mesh;
		AddChild(meshInstance);
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
				leftMouseDown = mouseButton.Pressed;
			if (mouseButton.ButtonIndex == MouseButton.Right)
				rightMouseDown = mouseButton.Pressed;
		}
	}
	
	public override void _Process(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 deltaMouse = mousePos - lastMousePos;

		float angleX = deltaMouse.X * rotationSpeed * 0.5f;
		float angleY = deltaMouse.Y * rotationSpeed * 0.5f;
	
		Transformation4 rotation = Transformation4.Identity;

		if (rightMouseDown)
		{
			rotation = Transformation4.RotateXW(angleY) * Transformation4.RotateZW(angleX);
		}
		else if (leftMouseDown)
		{
			rotation = Transformation4.RotateXZ(angleX) * Transformation4.RotateYZ(angleY);
		}

		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = rotation * vertices[i];
		}

		lastMousePos = mousePos;
		UpdateMesh();
	}
	
	private void InitVertices()
	{
		vertices = new Vector4[16];
		int i = 0;
		for (int x = -1; x <= 1; x += 2)
		for (int y = -1; y <= 1; y += 2)
		for (int z = -1; z <= 1; z += 2)
		for (int w = -1; w <= 1; w += 2)
			vertices[i++] = new Vector4(x * 2f, y * 2f, z * 2f, w * 2f);
	}
	private void InitEdges()
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
	
	private List<Vector2> ComputeSlice()
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
	
	private void UpdateMesh()
	{
		mesh.ClearSurfaces();
		mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);
		
		mesh.SurfaceSetColor(new Color(0.2f, 0.4f, 1.0f, 0.3f));
		
		float size = 3f;
		Vector3[] planeVerts = {
			new Vector3(-size, 0, -size),
			new Vector3(size, 0, -size),
			new Vector3(size, 0, size),
			new Vector3(-size, 0, size)
		};
		
		mesh.SurfaceAddVertex(planeVerts[0]);
		mesh.SurfaceAddVertex(planeVerts[1]);
		mesh.SurfaceAddVertex(planeVerts[2]);

		mesh.SurfaceAddVertex(planeVerts[2]);
		mesh.SurfaceAddVertex(planeVerts[3]);
		mesh.SurfaceAddVertex(planeVerts[0]);

		mesh.SurfaceEnd();

		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

		for (int i = 0; i < edges.GetLength(0); i++)
		{
			var v1 = vertices[edges[i, 0]];
			var v2 = vertices[edges[i, 1]];

			Vector3 p1 = new Vector3(v1.X + v1.W * 0.5f, v1.Y + v1.W * 0.5f, v1.Z);
			Vector3 p2 = new Vector3(v2.X + v2.W * 0.5f, v2.Y + v2.W * 0.5f, v2.Z);

			mesh.SurfaceAddVertex(p1);
			mesh.SurfaceAddVertex(p2);
		}

		mesh.SurfaceEnd();
	}
}
