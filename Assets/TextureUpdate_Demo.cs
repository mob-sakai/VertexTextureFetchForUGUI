using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureUpdate_Demo : MonoBehaviour
{
	[SerializeField] RawImage target;
	[SerializeField] Toggle toggleEveryFrame;
	[SerializeField] Dropdown dropdownLoad;


	TextureUpdater updater { get; set; }

	// Update is called once per frame
	void Update()
	{
		if (toggleEveryFrame.isOn)
		{
			SetRandom();
		}
		else
		{
			Stop();
		}
	}

	public void SetTextureUpdater(TextureUpdater updater)
	{
		Stop();
	
		this.updater = updater;
		updater.SetRgb();
		target.texture = updater.texture;
	}

	public void SetRandom()
	{
		if (updater)
			updater.SetRandom();
	}

	public void SetRgb()
	{
		if (updater)
			updater.SetRgb();
	}

	public void Stop()
	{
		if (updater)
			updater.Stop();
	}

	IEnumerator Start()
	{
		while (true)
		{
			int size = int.Parse(dropdownLoad.options[dropdownLoad.value].text);
			for (int i = 0; i < size * size; i++)
			{
				new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
			}
			yield return null;
		}
	}
}
