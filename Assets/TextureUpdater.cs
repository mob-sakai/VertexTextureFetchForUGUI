using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextureUpdater : MonoBehaviour
{
	public abstract Texture texture { get; protected set;}

	protected const int Width = 1024;
	protected const int Height = 16;

	public abstract void SetRandom ();

	public abstract void SetRgb ();
	
	public abstract void Stop ();
}
