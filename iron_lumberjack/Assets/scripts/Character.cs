using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Character : NetworkBehaviour {
	public static string[] characterNames = new string[] { "girl", "woodman", "Ninzya" };

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
	private Texture[] moveAnimationTextures;

	private bool inAttackingState = false;

	private bool isMoving = false;

	[SyncVar]
	private string charName;

	private string nonSyncedCharName;

	// Use this for initialization
	void Start () {
		this.charName = characterNames [Random.Range (0, characterNames.Length)];

		attackAnimationTextures = loadTextures (charName + "Attack");
		moveAnimationTextures = loadTextures (charName + "Walk");

		var clonedMaterial = new Material (Shader.Find("Standard"));
		clonedMaterial.CopyPropertiesFromMaterial (characterMaterial);
		characterMaterial = clonedMaterial;
		transform.GetChild(1).GetComponent<MeshRenderer> ().material = clonedMaterial;
		characterMaterial.SetTexture ("_MainTex", attackAnimationTextures [0]);

		characters.Add (this);
		Spawn ();
	}


	void UpdateTextures() {
		attackAnimationTextures = loadTextures (charName + "Attack");
		moveAnimationTextures = loadTextures (charName + "Walk");

		var clonedMaterial = new Material (Shader.Find("Standard"));
		clonedMaterial.CopyPropertiesFromMaterial (characterMaterial);
		characterMaterial = clonedMaterial;
		transform.GetChild(1).GetComponent<MeshRenderer> ().material = clonedMaterial;
		characterMaterial.SetTexture ("_MainTex", attackAnimationTextures [0]);
	}

	float lastLogTime = 0;

	Texture[] loadTextures(string path) {
		Object[] texturesAsObj = Resources.LoadAll (path);
		Texture[] textures = new Texture[texturesAsObj.GetLength(0)];
		for (int i = 0; i < texturesAsObj.GetLength(0); i++) {
			textures [i] = (Texture)texturesAsObj [i];
		}
		return textures;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time - lastLogTime > 5) {
			DebugConsole.Log ("Character netId: " + netId + " coords: " + transform.position + " mat: " + characterMaterial.GetInstanceID() + " health: " + health);
			lastLogTime = Time.time;
		}

		if (!isLocalPlayer) {
			return;
		}

		bool wasMoving = isMoving;
		isMoving = false;
		var v2d = new Vector2 (0, 0);
		if (Input.GetKey (KeyCode.W)) {
			v2d += Vector2.up;
			isMoving = true;
		}
		if (Input.GetKey (KeyCode.S)) {
			v2d += Vector2.down;
			isMoving = true;
		}
		if (Input.GetKey (KeyCode.A)) {
			v2d += Vector2.left;
			isMoving = true;
		}
		if (Input.GetKey (KeyCode.D)) {
			v2d += Vector2.right;
			isMoving = true;
		}
		SetVelocity (v2d);
		transform.position = new Vector3 (transform.position.x, 0.5f, transform.position.z);

		if (!inAttackingState) {
			if (isMoving && !wasMoving) {
				CmdStartWalkAnimation (netId);
			} else if (wasMoving && !isMoving) {
				CmdStopWalkAnimation (netId);
			}
		}
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
		if (nonSyncedCharName == null || !nonSyncedCharName.Equals (charName)) {
			nonSyncedCharName = charName;
			UpdateTextures ();
		}

		if (!isLocalPlayer) {
			transform.GetChild (0).GetComponent<Light> ().intensity = 0;
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

	void OnTriggerEnter(Collider other) {
		Debug.Log ("OnTriggerEnter: tag: " + other.tag);
		if (other.tag.Equals("Spikes")) {
			Die();
		}

		if (other.tag.Equals("Weapon") && other.GetComponentInParent<Character>().inAttackingState) {
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
			}),
			-1f,
			false
		);
	}

	[Command]
	void CmdStartWalkAnimation(NetworkInstanceId netId) {
		Debug.Log ("CmdStartWalkAnimation netId: " + netId);
		foreach (Character character in characters) {
			if (character.netId.Equals(netId)) {
				character.StartWalkAnimation ();
			}
		}
		RpcStartWalkAnimation (netId);
	}

	[ClientRpc]
	void RpcStartWalkAnimation(NetworkInstanceId netId) {
		Debug.Log ("RpcStartWalkAnimation netId: " + netId);
		foreach (Character character in characters) {
			if (character.netId.Equals(netId)) {
				character.StartWalkAnimation ();
			}
		}
	}

	void StartWalkAnimation() {
		GetComponent<AnimationPlayer> ().StartAnimation (
			attackAnimationTextures,
			characterMaterial,
			100000f,
			new System.Action (delegate {
			}),
			1.0f / 24f,
			true
		);
	}

	[Command]
	void CmdStopWalkAnimation(NetworkInstanceId netId) {
		Debug.Log ("CmdStopWalkAnimation netId: " + netId);
		foreach (Character character in characters) {
			if (character.netId.Equals(netId)) {
				character.StopWalkAnimation ();
			}
		}
		RpcStopWalkAnimation (netId);
	}

	[ClientRpc]
	void RpcStopWalkAnimation(NetworkInstanceId netId) {
		Debug.Log ("RpcStopWalkAnimation netId: " + netId);
		foreach (Character character in characters) {
			if (character.netId.Equals(netId)) {
				character.StopWalkAnimation ();
			}
		}
	}

	void StopWalkAnimation() {
		GetComponent<AnimationPlayer> ().StopAnimation ();
	}
}
