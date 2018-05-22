using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.IO;
using Random = UnityEngine.Random;

public class ByteArrayToTexture : MonoBehaviour
{

	//################################
	// Serialize Members.
	//################################
	[SerializeField] Dropdown dropDownWidth;
	[SerializeField] Dropdown dropDownHeight;
	[SerializeField] Toggle toggleEveryFrame;
	[SerializeField] RawImage target;
	[SerializeField] RenderTexture rt;
	[SerializeField] Texture2D texture;
	[SerializeField] Material material;

	private void Update()
	{
		if(toggleEveryFrame.isOn)
		{
			SetTextureColorsToRandom();
		}
	}

	public void SetTextureColorsToRandom ()
	{
		if (texture && colors != null) {
			Profiler.BeginSample ("TEST: Random");
			SetColorsToRandom (colors);
			Profiler.EndSample ();
			SetBySetPixels ();
		} else if (rt && colors32 != null) {
			Profiler.BeginSample ("TEST: Random");
			SetColorsToRandom (colors32);
			Profiler.EndSample ();
			StartCoroutine (CoSetByCommandBuffer ());
		}
	}

	public void SetTextureColorsToRGB ()
	{
		if (texture && colors != null) {
			Profiler.BeginSample ("TEST: RGB");
			SetColorsToRGB (colors);
			Profiler.EndSample ();
			SetBySetPixels ();
		} else if (rt && colors32 != null) {
			Profiler.BeginSample ("TEST: RGB");
			SetColorsToRGB (colors32);
			Profiler.EndSample ();
			StartCoroutine (CoSetByCommandBuffer ());
		}
	}


	public void UpdateTextureBySetPixels ()
	{
		if (rt) {
			rt.Release ();
			Destroy (rt);
			rt = null;
		}

		// 解像度.
		int w = int.Parse (dropDownWidth.options [dropDownWidth.value].text);
		int h = int.Parse (dropDownHeight.options [dropDownHeight.value].text);
		Debug.LogFormat ("UpdateTextureBySetPixels {0}x{1}", w, h);

		// 解像度が違う場合、リサイズ.
		if (texture && (texture.width != w || texture.width != h)) {
			texture.Resize (w, h, TextureFormat.ARGB32, false);
		}

		// テクスチャを生成.
		if (texture == null) {
			texture = new Texture2D (w, h, TextureFormat.ARGB32, false, false);
			texture.filterMode = FilterMode.Point;
			texture.mipMapBias = 0;
			texture.wrapMode = TextureWrapMode.Clamp;
		}
		// レンダリングのため、RawImageで表示.
		target.texture = texture;

		colors = new Color[w * h];

		SetTextureColorsToRGB ();
	}

	void SetBySetPixels ()
	{
		Profiler.BeginSample ("TEST: SetPixels");
		texture.SetPixels (colors);
		Profiler.EndSample ();

		Profiler.BeginSample ("TEST: Apply");
		texture.Apply ();
		Profiler.EndSample ();
	}

	public void UpdateTextureByCommandBuffer (bool tri)
	{
		if (texture) {
			Destroy (texture);
			texture = null;
		}

		// 現在の解像度.
		int w = int.Parse (dropDownWidth.options [dropDownWidth.value].text);
		int h = int.Parse (dropDownHeight.options [dropDownHeight.value].text);
		Debug.LogFormat ("UpdateTextureByCommandBuffer {0}x{1} tri:{2} vertex:{3}", w, h, tri, tri ? w * h * 3 : w * h * 4);

		// 解像度が違う場合、古いRTを削除.
		if (rt && (rt.width != w || rt.width != h)) {
			rt.Release ();
			rt = null;
		}

		// RT生成.
		if (rt == null) {
			rt = new RenderTexture (w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			rt.filterMode = FilterMode.Point;
			rt.useMipMap = false;
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.hideFlags = HideFlags.HideAndDontSave;
		}

		// レンダリングのため、RawImageで表示.
		target.texture = rt;

		Profiler.BeginSample ("TEST: GenerateMesh");
		colors32 = tri
			? GenerateMeshTri (out mesh, w, h)
			: GenerateMeshQuad (out mesh, w, h);
		Profiler.EndSample ();

		SetTextureColorsToRGB ();
	}

	IEnumerator CoSetByCommandBuffer ()
	{
		
		Profiler.BeginSample ("TEST: Set Mesh.colors32");
		mesh.colors32 = colors32;
		Profiler.EndSample ();

		if (cb != null) {
			yield break;
		}

		Profiler.BeginSample ("TEST: Create CB");
		cb = new CommandBuffer ();
		cb.SetRenderTarget (new RenderTargetIdentifier (rt));
		Profiler.EndSample ();

		Profiler.BeginSample ("TEST: DrawMesh");
		cb.DrawMesh (mesh, Matrix4x4.identity, material);
		Profiler.EndSample ();

		Profiler.BeginSample ("TEST: AddCommandBuffer");
		renderCamera.AddCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		Profiler.EndSample ();
		
		yield return new WaitForEndOfFrame ();

		renderCamera.RemoveCommandBuffer (CameraEvent.BeforeForwardOpaque, cb);
		cb.Dispose ();
		cb = null;
	}


	Camera renderCamera { get { return target.canvas.worldCamera ?? Camera.main; } }


	CommandBuffer cb;

	Mesh mesh;


	Color32[] colors32;
	Color[] colors;


	public static void SetColorsToRGB (Color32[] cols)
	{
		int rgb = 0;
		var r = new Color32 (255, 0, 0, 255);
		var g = new Color32 (0, 255, 0, 255);
		var b = new Color32 (0, 0, 255, 255);
		int dim = cols.Length % 3 == 0 ? 3 : 4; 
		for (int i = 0; i < cols.Length; i += dim) {
			var c = rgb % 3 == 0 ? r : rgb % 3 == 1 ? g : b;
			for (int j = 0; j < dim; j++) {
				cols [i + j] = c;
			}
			rgb++;
		}
	}

	public static void SetColorsToRGB (Color[] cols)
	{
		int rgb = 0;
		var r = Color.red;
		var g = Color.green;
		var b = Color.blue;
		for (int i = 0; i < cols.Length; i++) {
			var c = rgb % 3 == 0 ? r : rgb % 3 == 1 ? g : b;
			cols [i] = c;
			rgb++;
		}
	}

	public static void SetColorsToRandom (Color[] cols)
	{
		for (int i = 0; i < cols.Length; i++) {
			var c = new Color (UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
			cols [i] = c;
		}
	}

	public static void SetColorsToRandom (Color32[] cols)
	{
		
		int dim = cols.Length % 3 == 0 ? 3 : 4; 

		for (int i = 0; i < cols.Length; i += dim) {
			var c = new Color32 ((byte)UnityEngine.Random.Range (0, 256), (byte)UnityEngine.Random.Range (0, 256), (byte)UnityEngine.Random.Range (0, 256), 255);
			for (int j = 0; j < dim; j++) {
				cols [i + j] = c;
			}
		}
	}

	public static Color32[] GenerateMeshQuad (out Mesh mesh, int width, int height)
	{
		var size = width * height;

		mesh = new Mesh ();

		var verts = new Vector3[size * 4];
		var tris = new int[size * 6];
		var cols = new Color32[size * 4];

		for (int y = 0; y < height; y++) {

			float yMin = (y + 0) / (float)height * 2 - 1;
			float yMax = (y + 1) / (float)height * 2 - 1;

			for (int x = 0; x < width; x++) {
				float xMin = (x + 0) / (float)width * 2 - 1;
				float xMax = (x + 1) / (float)width * 2 - 1;
				int indexVert = (y * width + x) * 4;
				int indexTris = (y * width + x) * 6;

				verts [indexVert + 0] = new Vector3 (xMin, yMin);
				verts [indexVert + 1] = new Vector3 (xMin, yMax);
				verts [indexVert + 2] = new Vector3 (xMax, yMax);
				verts [indexVert + 3] = new Vector3 (xMax, yMin);

				tris [indexTris + 0] = indexVert + 0;
				tris [indexTris + 1] = indexVert + 1;
				tris [indexTris + 2] = indexVert + 2;
				tris [indexTris + 3] = indexVert + 0;
				tris [indexTris + 4] = indexVert + 2;
				tris [indexTris + 5] = indexVert + 3;
			}
		}
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.colors32 = cols;

		mesh.RecalculateNormals();

		return cols;
	}

	public static Color32[] GenerateMeshTri (out Mesh mesh, int width, int height)
	{
		var size = width * height;

		mesh = new Mesh ();
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

		return cols;
	}
}
