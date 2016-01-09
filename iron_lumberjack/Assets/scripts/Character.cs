using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Character : NetworkBehaviour {
	private static ArrayList characters = new ArrayList();
//	public int curTextureIndex = 0;
//	public float lastTextureChangeTime = 0;
//	public float changeTextureFreq = 24;
	public Texture idleTexture;

//	public Texture[] textures;
	public float speed;
	public int health;
	public string attackAnimationDir;

	public Material characterMaterial;
	private Texture[] attackAnimationTextures;

	private bool inAttackingState = false;

	[SyncVar]
	private int playerId;

	// Use this for initialization
	void Start () {
		Object[] texturesAsObj = Resources.LoadAll (attackAnimationDir);
		Debug.Log ("texturesAsObj size: " + texturesAsObj.GetLength (0));
		attackAnimationTextures = new Texture[texturesAsObj.GetLength(0)];
		for (int i = 0; i < texturesAsObj.GetLength(0); i++) {
			attackAnimationTextures [i] = (Texture)texturesAsObj [i];
		}

		var clonedMaterial = new Material (Shader.Find("Standard"));
		clonedMaterial.CopyPropertiesFromMaterial (characterMaterial);
		characterMaterial = clonedMaterial;
		transform.GetChild(1).GetComponent<MeshRenderer> ().material = clonedMaterial;

		characters.Add (this);
		Spawn ();
	}

	float lastLogTime = 0;

	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time - lastLogTime > 5) {
			DebugConsole.Log ("Character netId: " + netId + " coords: " + transform.position + " mat: " + characterMaterial.GetInstanceID());
			lastLogTime = Time.time;
		}

		if (!isLocalPlayer) {
			return;
		}

		var v2d = new Vector2 (0, 0);
		if (Input.GetKey (KeyCode.W)) {
			v2d += Vector2.up;
		}
		if (Input.GetKey (KeyCode.S)) {
			v2d += Vector2.down;
		}
		if (Input.GetKey (KeyCode.A)) {
			v2d += Vector2.left;
		}
		if (Input.GetKey (KeyCode.D)) {
			v2d += Vector2.right;
		}
		SetVelocity (v2d);
		transform.position = new Vector3 (transform.position.x, 0.5f, transform.position.z);
	}

	void SetVelocity(Vector2 v2d) {
		GetComponent<Rigidbody>().velocity = new Vector3 (v2d.x, 0, v2d.y) * speed * Time.deltaTime;
	}
//
//	void SetTexture(Texture texture) {
//		characterMaterial.SetTexture ("_MainTex", texture);
//		characterMaterial.SetTexture ("_BumpMap", texture);
//	}

	void Update() {
		if (!isLocalPlayer) {
			return;
		}

		var mousePosition = Input.mousePosition;
		mousePosition.z = 10.0f;
		var mousePositionWorldCoords = Camera.main.ScreenToWorldPoint (mousePosition);
		mousePositionWorldCoords.y = transform.position.y;
		var characterDirection = transform.position - mousePositionWorldCoords;
		transform.rotation = Quaternion.LookRotation(characterDirection);

		Camera.main.transform.position = new Vector3(
			transform.position.x,
			Camera.main.transform.position.y,
			transform.position.z
		);

		if (Input.GetMouseButtonDown (0) && !inAttackingState) {
			Debug.Log ("Calling CmdAttackWithWeapon");
			CmdAttackWithWeapon (this.netId);
		}
			
//
//		if (Time.time - lastTextureChangeTime > 1.0 / changeTextureFreq) {
//			SetTexture (textures [curTextureIndex]);
//			curTextureIndex = (curTextureIndex + 1) % textures.GetLength(0);
//			lastTextureChangeTime = Time.time;
//		}
	}

	void Die() {
		Spawn ();
		GameObject.Find ("MusicManager").GetComponent<MusicController> ().playDeathSound();
	}



	void Spawn() {
		GameObject[] respawnPoints = GameObject.FindGameObjectsWithTag ("Respawn");
		transform.position = respawnPoints[Random.Range(0, respawnPoints.Length)].transform.position;
	}

	void Hit(int damage) {
		health -= damage;
		if (health <= 0) {
			Die ();
		}
	}

	IEnumerator ShutdownFavn(Transform favn)
	{
		yield return new WaitForSeconds (2f);

		for (int i = 0; i < favn.childCount; ++i) {
			favn.GetChild (i).gameObject.SetActive (false);

		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("OnTriggerEnter: tag: " + other.tag);
		if (other.tag.Equals("Spikes")) {
			Die();
		}

		if (other.tag.Equals ("Favn")) {
			for (int i = 0; i < other.transform.childCount; ++i) {
				other.transform.GetChild (i).gameObject.SetActive (true);

			}
			StartCoroutine (ShutdownFavn (other.transform));

			Invoke ("Die", 0.6f /*timeToDeath*/);
		}

		if (other.tag.Equals("Weapon")) {
			Hit (other.GetComponent<IWeapon> ().GetDamage ());
		}
	}

	[Command]
	void CmdAttackWithWeapon(NetworkInstanceId netId) {
		Debug.Log ("CmdAttackWithWeapon netId: " + netId);
		foreach (Character character in characters) {
			if (character.netId.Equals(netId)) {
				character.AttackWithWeapon ();
			}
		}
		RpcAttackWithWeapon (netId);
	}

	[ClientRpc]
	void RpcAttackWithWeapon(NetworkInstanceId netId) {
		Debug.Log ("RpcAttackWithWeapon netId: " + netId);
		foreach (Character character in characters) {
			if (character.netId.Equals(netId)) {
				character.AttackWithWeapon ();
			}
		}
	}

	void AttackWithWeapon() {
		Debug.Log ("AttackWithWeapon netId: " + netId);
		inAttackingState = true;
		GetComponent<AnimationPlayer> ().StartAnimation (
			attackAnimationTextures,
			characterMaterial,
			0.5f,
			new System.Action (delegate {
				inAttackingState = false;
			})
		);
	}
}
