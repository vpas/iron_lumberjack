using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Character : NetworkBehaviour {
	public int curTextureIndex = 0;
	public float lastTextureChangeTime = 0;
	public float changeTextureFreq = 24;
	public float speed = 5;
	public Material characterMaterial;
	public Texture[] textures;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetKey (KeyCode.W)) {
			gameObject.transform.position = gameObject.transform.position + new Vector3 (0, 0, 1) * speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.S)) {
			gameObject.transform.position = gameObject.transform.position + new Vector3 (0, 0, -1) * speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.A)) {
			gameObject.transform.position = gameObject.transform.position + new Vector3 (-1, 0, 0) * speed * Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			gameObject.transform.position = gameObject.transform.position + new Vector3 (1, 0, 0) * speed * Time.deltaTime;
		}


	}

	void SetTexture(Texture texture) {
		characterMaterial.SetTexture ("_MainTex", texture);
		characterMaterial.SetTexture ("_BumpMap", texture);
	}

	void Update() {
		if (!isLocalPlayer) {
			return;
		}


		if (Time.time - lastTextureChangeTime > 1.0 / changeTextureFreq) {
			SetTexture (textures [curTextureIndex]);
			curTextureIndex = (curTextureIndex + 1) % textures.GetLength(0);
			lastTextureChangeTime = Time.time;
		}
	}
}
