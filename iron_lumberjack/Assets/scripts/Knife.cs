using UnityEngine;
using System.Collections;

public class Knife : MonoBehaviour, IWeapon {
	public int damage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int GetDamage() {
		return damage;
	}
}
