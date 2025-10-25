using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

public class Python
{
	private readonly static string PythonExe = @".venv/bin/python";
	private readonly static string PythonScript = @"scipy_mesh.py";
	private readonly static string MeshInput = "mesh_input.json";
	private readonly static string MeshOutput = "mesh_output.json";
	
	public static (List<Vector3> vertices, List<int[]> faces) RunMeshScript(Vector4[] vertices4D)
	{
		var data = new
		{
			vertices = vertices4D
				.Select(v => new float[] {v.X, v.Y, v.Z, v.W})
				.ToList()
		};
		File.WriteAllText(MeshInput, JsonSerializer.Serialize(data));
		ProcessStartInfo psi = new()
		{
			FileName = PythonExe,
			Arguments = $"\"{PythonScript}\"",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true
		};
		using (Process process = new())
		{
			process.StartInfo = psi;
			process.Start();
			
			string output = process.StandardOutput.ReadToEnd();
			string error = process.StandardError.ReadToEnd();
			process.WaitForExit();
			
			if (!string.IsNullOrEmpty(error))
				GD.PrintErr("Python error: " + error);
			else
				GD.Print("Python output: " + output);
		}
		
		if (!File.Exists(MeshOutput))
		{
			GD.PrintErr("Python did not generate output file: " + MeshOutput);
			return (new List<Vector3>(), new List<int[]>());
		}
		
		string json = File.ReadAllText(MeshOutput);
		using JsonDocument doc = JsonDocument.Parse(json);
		var root = doc.RootElement;
		
		List<Vector3> vertices3D = [];
		foreach (var v in root.GetProperty("vertices").EnumerateArray())
		{
			vertices3D.Add(new Vector3(
				v[0].GetSingle(),
				v[1].GetSingle(),
				v[2].GetSingle()
			));
		}
		
		List<int[]> faces = [];
		foreach (var face in root.GetProperty("faces").EnumerateArray())
		{
			faces.Add(
			[
				face[0].GetInt32(),
				face[1].GetInt32(),
				face[2].GetInt32()
			]);
		}
		return (vertices3D, faces);
	}
}
