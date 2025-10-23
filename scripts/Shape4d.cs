using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Shape4d : Node3D
{
	protected Vector4[] vertices;
	protected int[,] edges;
	
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
	
	protected abstract void InitVertices();
	protected abstract void InitEdges();

	protected virtual void UpdateMesh()
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
