using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using MIConvexHull;

public class Visuals
{
	public static List<Vector3> SliceVertices(IEnumerable<Vector4> vertices)
	{
		float wDistance = 5f;
		return vertices
			.Select(v => new Vector3(v.X / (wDistance - v.W), v.Y / (wDistance - v.W), v.Z / (wDistance - v.W)))
			.ToList();
	}
}
