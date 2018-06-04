using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TextureUpdater_LoadRawTextureData : TextureUpdater
{

	public override Texture texture { get; protected set; }

//	byte[] colorBytes = new byte[Width * Height * 32];
	byte[][] colorsrnd = new byte[RandomCache][];
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
		int i = Time.frameCount % RandomCache;
		SetColorsToRandom(ref colorsrnd[i]);
		UpdateTexture(texture as Texture2D, colorsrnd[i]);
	}

	public override void SetRgb()
	{
		SetColorsToRGB(ref colorsrgb);
		UpdateTexture(texture as Texture2D, colorsrgb);
	}
	
	public override void Stop()
	{
	}

	static void UpdateTexture(Texture2D tex, byte[] cols)
	{
		Profiler.BeginSample("TEST: LoadRawTextureData");
		tex.LoadRawTextureData(cols);
		Profiler.EndSample();

		Profiler.BeginSample("TEST: Apply");
		tex.Apply(false, false);
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

		Profiler.BeginSample("TEST: SetColorsToRandom");
		byte c;
		for (int i = 0; i < cols.Length; i += 4)
		{
			c = (byte)(i * 255 / cols.Length);
			cols[i + 0] = 255;
			cols[i + 1] = c;
			cols[i + 2] = c;
			cols[i + 3] = c;
		}
		Profiler.EndSample();
	}

}