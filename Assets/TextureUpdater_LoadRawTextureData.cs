using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TextureUpdater_LoadRawTextureData : TextureUpdater
{

	public override Texture2D texture { get; protected set; }

//	byte[] colorBytes = new byte[Width * Height * 32];
	byte[][] colorsrnd = new byte[10][];
	byte[] colorsrgb;

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

	static void UpdateTexture(Texture2D tex, byte[] cols)
	{
		Profiler.BeginSample("TEST: LoadRawTextureData");
		tex.LoadRawTextureData(cols);
		Profiler.EndSample();

		Profiler.BeginSample("TEST: Apply");
		tex.Apply();
		Profiler.EndSample();
	}


	static void SetColorsToRGB(ref byte[] cols)
	{
		if (cols != null)
			return;
		cols = new byte[Width * Height * 4];

		int rgb = 0;
		for (int i = 0; i < cols.Length; i += 4)
		{
			cols[i + 0] = 255;
			cols[i + 1] = (rgb % 3 == 0) ? byte.MaxValue : byte.MinValue;
			cols[i + 2] = (rgb % 3 == 1) ? byte.MaxValue : byte.MinValue;
			cols[i + 3] = (rgb % 3 == 2) ? byte.MaxValue : byte.MinValue;
			rgb++;
		}
	}

	static void SetColorsToRandom(ref byte[] cols)
	{
		if (cols != null)
			return;
		cols = new byte[Width * Height * 4];

		for (int i = 0; i < cols.Length; i += 4)
		{
			cols[i + 0] = 255;
			cols[i + 1] = (byte)Random.Range(0, 256);
			cols[i + 2] = (byte)Random.Range(0, 256);
			cols[i + 3] = (byte)Random.Range(0, 256);
		}
	}

}