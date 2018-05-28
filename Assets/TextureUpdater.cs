using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextureUpdater : MonoBehaviour
{
	public abstract Texture2D texture { get; protected set;}

	protected const int Width = 1024;
	protected const int Height = 1024;

	public abstract void SetRandom ();

	public abstract void SetRgb ();
}
