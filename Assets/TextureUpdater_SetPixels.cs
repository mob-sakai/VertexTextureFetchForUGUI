using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TextureUpdater_SetPixels32 : TextureUpdater
{
	public override Texture2D texture { get; protected set; }

	Color32[] colors32rgb = new Color32[Width * Height];
	Color32[][] colors32rnd = new Color32[10][];

	void Start()
	{
		texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false, false);
		texture.filterMode = FilterMode.Point;
		texture.mipMapBias = 0;
		texture.wrapMode = TextureWrapMode.Clamp;
	}


	public override void SetRandom()
	{
		int i = Time.frameCount % 10;
		SetColorsToRandom(ref colors32rnd[i]);
		UpdateTexture(texture, colors32rnd[i]);
	}

	public override void SetRgb()
	{
		SetColorsToRGB(ref colors32rgb);
		UpdateTexture(texture, colors32rgb);
	}

	static void UpdateTexture(Texture2D tex, Color32[] cols)
	{
		Profiler.BeginSample("TEST: SetPixels32");
		tex.SetPixels32(cols);
		Profiler.EndSample();

		Profiler.BeginSample("TEST: Apply");
		tex.Apply();
		Profiler.EndSample();
	}


	static void SetColorsToRGB(ref Color32[] cols)
	{
		if (cols != null)
			return;
		cols = new Color32[Width * Height];
			
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
		
		cols = new Color32[Width * Height];

		for (int i = 0; i < cols.Length; i++)
		{
			cols[i] = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
		}
	}
}