using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public class TextureUpdater_CommandBuffer : TextureUpdater
{

	[SerializeField] Material material;
	
	public override Texture texture { get; protected set; }

	
	Color32[] colors32rgb;
	Color32[][] colors32rnd = new Color32[RandomCache][];
	Mesh mesh;

	void Start()
	{
		var rt =  new RenderTexture (Width, Height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		rt.filterMode = FilterMode.Point;
		rt.useMipMap = false;
		rt.wrapMode = TextureWrapMode.Clamp;
		rt.hideFlags = HideFlags.HideAndDontSave;

		texture = rt;

		mesh = GenerateMesh(Width, Height);
	}

	CommandBuffer cb;
	
	IEnumerator CoSetByCommandBuffer ()
	{
		Profiler.BeginSample ("TEST: Set Mesh.colors32");
		mesh.colors32 = colors32rgb;
		Profiler.EndSample ();

		if (cb != null) {
			yield break;
		}

		Profiler.BeginSample ("TEST: Create CB");
		cb = new CommandBuffer ();
		cb.SetRenderTarget (new RenderTargetIdentifier (texture as RenderTexture));
		Profiler.EndSample ();

		Profiler.BeginSample ("TEST: DrawMesh");
		cb.DrawMesh (mesh, Matrix4x4.identity, material);
		Profiler.EndSample ();

		Profiler.BeginSample ("TEST: AddCommandBuffer");
		Camera.main.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		Profiler.EndSample ();
		
		yield return new WaitForEndOfFrame ();

		Camera.main.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		cb.Dispose ();
		cb = null;
	}


	public override void SetRandom()
	{
		int i = Time.frameCount % RandomCache;
		SetColorsToRandom(ref colors32rnd[i]);
		
		StartCb(colors32rnd[i]);
	}

	public override void SetRgb()
	{
		SetColorsToRGB(ref colors32rgb);
		
		StartCb(colors32rgb);
	}
	
	public void StartCb(Color32[] cols)
	{
	
		Profiler.BeginSample ("TEST: Set Mesh.set_colors32");
		for (int i = 0; i < Height;i+=16)
			mesh.colors32 = cols;
		Profiler.EndSample ();
		
		if (cb != null)
			return;

		Profiler.BeginSample ("TEST: Create CB");
		cb = new CommandBuffer ();
		cb.name = "TEST: CommandBuffer rendering";
		cb.SetRenderTarget (new RenderTargetIdentifier (texture as RenderTexture));
		Profiler.EndSample ();
		
		Profiler.BeginSample ("TEST: DrawMesh");
		for (int i = 0; i < Height;i+=16)
		{
			var matrix = Matrix4x4.TRS(new Vector3(0, -(float)i / (float)Height), Quaternion.identity, new Vector3(1, (float)i / (float)Height));
			cb.DrawMesh (mesh, matrix, material);
		}
		Profiler.EndSample();

		Profiler.BeginSample ("TEST: AddCommandBuffer");
		Camera.main.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		Profiler.EndSample ();
	}
	
	public override void Stop()
	{
		if (cb == null)
			return;
			
		Camera.main.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		cb = null;
	}

	static void UpdateTexture(Texture2D tex, Color32[] cols)
	{
		Profiler.BeginSample("TEST: SetPixels32");
		tex.SetPixels32(cols);
		Profiler.EndSample();

		Profiler.BeginSample("TEST: Apply");
		tex.Apply(false, false);
		Profiler.EndSample();
	}


	static void SetColorsToRGB(ref Color32[] cols)
	{
		if (cols != null)
			return;
		cols = new Color32[Width * Height * 3];
			
		int rgb = 0;
		var r = new Color32(255, 0, 0, 255);
		var g = new Color32(0, 255, 0, 255);
		var b = new Color32(0, 0, 255, 255);
		int dim = cols.Length % 3 == 0 ? 3 : 4; 
		for (int i = 0; i < cols.Length; i += dim)
		{
			var c = rgb % 3 == 0 ? r : rgb % 3 == 1 ? g : b;
			for (int j = 0; j < dim; j++)
			{
				cols[i + j] = c;
			}
			rgb++;
		}
	}

	static void SetColorsToRandom(ref Color32[] cols)
	{
		if (cols != null)
			return;
		
		cols = new Color32[Width * Height * 3];

		for (int i = 0; i < cols.Length; i++)
		{
			cols[i] = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
		}
	}
	
	
	public static Mesh GenerateMesh (int width, int height)
	{
		var size = width * height;

		var mesh = new Mesh ();
		var verts = new Vector3[size * 3];
		var tris = new int[size * 3];
		var cols = new Color32[size * 3];

		float diff = 0.5f / width;
		for (int y = 0; y < height; y++) {

			float yMin = (y + 0) / (float)height * 2 - 1;
			float yMax = (y + 1) / (float)height * 2 - 1;

			for (int x = 0; x < width; x++) {
				float xMin = (x + 0) / (float)width * 2 - 1 + diff;
				float xMax = (x + 1) / (float)width * 2 - 1 + diff;
				int indexVert = (y * width + x) * 3;

				verts [indexVert + 0] = new Vector3 (xMin, yMin);
				verts [indexVert + 1] = new Vector3 (xMin, yMax);
				verts [indexVert + 2] = new Vector3 (xMax, yMax);

				tris [indexVert + 0] = indexVert + 0;
				tris [indexVert + 1] = indexVert + 1;
				tris [indexVert + 2] = indexVert + 2;
			}
		}
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.colors32 = cols;

		//return cols;
		return mesh;
	}
}
