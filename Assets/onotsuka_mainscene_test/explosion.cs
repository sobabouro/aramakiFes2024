using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour {
	[SerializeField, Tooltip("���e")]
	private GameObject bomb;

	public void SetBomb() {
		Instantiate(bomb, transform.position, transform.rotation);
	}
}
