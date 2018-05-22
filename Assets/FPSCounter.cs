using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	[SerializeField]
	private Text m_Display;

	private float _accum;
	private int _frames;
	private float _left;

	private void Update()
	{
		_left -= Time.deltaTime;
		_accum += Time.timeScale / Time.deltaTime;
		_frames++;

		if (0 < _left) return;

		m_Display.text = string.Format("FPS: {0:F2}", _accum / _frames);

		_left = 0.5f;
		_accum = 0;
		_frames = 0;
	}
}
