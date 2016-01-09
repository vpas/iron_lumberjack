using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Character : NetworkBehaviour {
//	public int curTextureIndex = 0;
//	public float lastTextureChangeTime = 0;
//	public float changeTextureFreq = 24;
	public Material characterMaterial;
//	public Texture[] textures;
	public float speed;
	public int health;
	public string attackAnimationDir;
	private Texture[] attackAnimationTextures;

	private bool inAttackingState = false;

	// Use this for initialization
	void Start () {
		Object[] texturesAsObj = Resources.LoadAll (attackAnimationDir);
		Debug.Log ("texturesAsObj size: " + texturesAsObj.GetLength (0));
		attackAnimationTextures = new Texture[texturesAsObj.GetLength(0)];
		for (int i = 0; i < texturesAsObj.GetLength(0); i++) {
			attackAnimationTextures [i] = (Texture)texturesAsObj [i];
		}
		Spawn ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
			AttackWithWeapon ();
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
		GameObject respawnPoint = GameObject.FindGameObjectWithTag ("Respawn");
		transform.position = respawnPoint.transform.position;
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

		if (other.tag.Equals("Weapon")) {
			Hit (other.GetComponent<IWeapon> ().GetDamage ());
		}
	}

	void AttackWithWeapon() {
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
