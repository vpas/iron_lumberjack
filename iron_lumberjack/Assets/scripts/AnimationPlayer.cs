using System;
using UnityEngine;
using UnityEngine.Networking;

public class AnimationPlayer : NetworkBehaviour
{
	Texture[] frames;
	Material material;
	bool playingAnimation = false;

	[SyncVar]
	int currentFrameIndex = -1;

	float lastFrameChangeTime = 0;
	Action onFinish;
	float secPerFrame;
	Texture oldTexture;

	public AnimationPlayer ()
	{
		
	}

	public void StartAnimation(Texture[] frames, Material material, float durationSec, Action onFinish) {
		Debug.Log ("StartAnimation");
		this.frames = frames;
		this.material = material;
		this.secPerFrame = durationSec / frames.GetLength (0);
		this.oldTexture = material.GetTexture("_MainTex");
		playingAnimation = true;
		currentFrameIndex = -1;
		lastFrameChangeTime = 0;
		this.onFinish = onFinish;
	}

	void Update() {
		if (!playingAnimation) {
			return;
		}

		if (Time.time - lastFrameChangeTime > this.secPerFrame) {
			currentFrameIndex++;
			Debug.Log ("currentFrameIndex: " + currentFrameIndex);
			if (currentFrameIndex >= frames.GetLength (0)) {
				playingAnimation = false;
				material.SetTexture ("_MainTex", oldTexture);
				onFinish ();
			} else {
				material.SetTexture ("_MainTex", frames [currentFrameIndex]);
				lastFrameChangeTime = Time.time;
			}
		}

	}
}
