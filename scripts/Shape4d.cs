using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Shape4d : Node3D
{
	protected Vector4[] vertices;
	protected int[,] edges;
	protected int[][] faces;
	protected float scale = 2f;
	
	private float rotationSpeed = 0.01f;
	private Vector2 lastMousePos;
	private bool leftMouseDown = false;
	private bool rightMouseDown = false;
	
	protected ImmediateMesh mesh;
	protected MeshInstance3D meshInstance;
	
	public override void _Ready()
	{
		InitVertices();
		InitEdges();
		InitFaces();
		
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
			rotation = Transformation4.RotateXW(angleX) * Transformation4.RotateZW(angleY);
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
		Outline();
		UpdateMesh();
	}
	
	protected abstract void InitVertices();
	protected abstract void InitEdges();
	protected abstract void InitFaces();
	
	protected virtual void Outline()
	{
		mesh.ClearSurfaces();
		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);

		List<Vector3> verts3d = Visuals.SliceVertices(vertices);

		for (int i = 0; i < edges.GetLength(0); i++)
		{
			Vector3 p1 = verts3d[edges[i, 0]];
			Vector3 p2 = verts3d[edges[i, 1]];

			mesh.SurfaceAddVertex(p1);
			mesh.SurfaceAddVertex(p2);
		}

		mesh.SurfaceEnd();
	}
	
	protected virtual void UpdateMesh() {}
}
