using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TextureUpdater_SetPixels : TextureUpdater
{

	public override Texture2D texture { get; protected set; }

	//	Color[] colors = new Color[Width * Height];
	Color[][] colorsrnd = new Color[10][];
	Color[] colorsrgb;

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
		SetColorsToRandom(ref colorsrnd[i]);
		UpdateTexture(texture, colorsrnd[i]);
	}

	public override void SetRgb()
	{
		SetColorsToRGB(ref colorsrgb);
		UpdateTexture(texture, colorsrgb);
	}

	static void UpdateTexture(Texture2D tex, Color[] cols)
	{
		Profiler.BeginSample("TEST: SetPixels");
		tex.SetPixels(cols);
		Profiler.EndSample();

		Profiler.BeginSample("TEST: Apply");
		tex.Apply();
		Profiler.EndSample();
	}

	public static void SetColorsToRGB(ref Color[] cols)
	{
		if (cols != null)
			return;
		cols = new Color[Width * Height];

		int rgb = 0;
		var r = Color.red;
		var g = Color.green;
		var b = Color.blue;
		for (int i = 0; i < cols.Length; i++)
		{
			var c = rgb % 3 == 0 ? r : rgb % 3 == 1 ? g : b;
			cols[i] = c;
			rgb++;
		}
	}

	public static void SetColorsToRandom(ref Color[] cols)
	{
		if (cols != null)
			return;
		cols = new Color[Width * Height];
		
		for (int i = 0; i < cols.Length; i++)
		{
			var c = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1);
			cols[i] = c;
		}
	}
}
