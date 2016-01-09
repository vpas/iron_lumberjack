using System;
using UnityEngine;
using UnityEngine.Networking;

public class AnimationPlayer : MonoBehaviour
{
	Texture[] frames;
	Material material;
	bool playingAnimation = false;

	int currentFrameIndex = -1;

	float lastFrameChangeTime = 0;
	Action onFinish;
	float secPerFrame;
	Texture oldTexture;
	bool loop;

	public AnimationPlayer ()
	{
		
	}

	public void StartAnimation(Texture[] frames, Material material, float durationSec, Action onFinish, float secPerFrame, bool loop) {
		Debug.Log ("StartAnimation");
		this.frames = frames;
		this.material = material;
		if (secPerFrame < 0) {
			this.secPerFrame = durationSec / frames.GetLength (0);
		} else {
			this.secPerFrame = secPerFrame;
		}
		this.loop = loop;
		this.oldTexture = material.GetTexture("_MainTex");
		playingAnimation = true;
		currentFrameIndex = -1;
		lastFrameChangeTime = 0;
		this.onFinish = onFinish;
	}

	public void StopAnimation() {
		loop = false;
		currentFrameIndex = frames.GetLength (0);
	}

	void Update() {
		if (!playingAnimation) {
			return;
		}

		if (Time.time - lastFrameChangeTime > this.secPerFrame) {
			currentFrameIndex++;
			Debug.Log ("currentFrameIndex: " + currentFrameIndex);
			if (currentFrameIndex >= frames.GetLength (0)) {
				if (loop) {
					currentFrameIndex = 0;
				} else {
					playingAnimation = false;
					material.SetTexture ("_MainTex", oldTexture);
					onFinish ();
				}
			} else {
				material.SetTexture ("_MainTex", frames [currentFrameIndex]);
				lastFrameChangeTime = Time.time;
			}
		}

	}
		
}
