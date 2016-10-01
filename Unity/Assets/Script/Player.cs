﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	[SerializeField]
	private float Acceleration = 1;
	[SerializeField]
	private float JumpPower = 1;
	[SerializeField]
	private SceneMove SceneMoveScript;
	[SerializeField]
	private float MaxSpeed = 10;
	private bool isOnFloor = true;
	private Coroutine WalkingCoroutine;
	private Vector3 screenPos;
	// Use this for initialization
	void Start() {
		StartCoroutine(Walking());
		StartCoroutine (Jump ());
	}

	void Update(){
		screenPos = Camera.main.WorldToScreenPoint (transform.position);
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.layer == (int)Layer.Floor){
			isOnFloor = true;
		} else if(col.gameObject.layer == (int)Layer.Gaul){
			//SceneMoveScript.enabled = true;
		}
	}

	void OnCollisionExit(Collision col){
		if(col.gameObject.layer == (int)Layer.Floor){
			isOnFloor = false;
		}
	}

	IEnumerator Walking()
	{
		
		while (true) {
			float x = Input.GetAxis ("Horizontal");
			Rigidbody rigidbody = GetComponent<Rigidbody> ();
			if (!(x < 0 && screenPos.x < 20) && isOnFloor) {//画面の左端にいなければ
				rigidbody.AddForce (x * Vector3.right * Acceleration, ForceMode.VelocityChange);//移動
			} else if(!(x < 0 && screenPos.x < 20) && !isOnFloor){
				rigidbody.AddForce (x * Vector3.right * (Acceleration/10), ForceMode.VelocityChange);//移動
			}
			if (screenPos.x < 20) {
				rigidbody.velocity = new Vector3 (0, rigidbody.velocity.y, 0);
			}

			SpeedLimit (rigidbody);
			CameraLockAtMe (x);
			yield return null;
		}
	}

	IEnumerator Jump(){
		while (true) {
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			if (Input.GetKeyDown (KeyCode.Space) && isOnFloor) {
				rigidbody.AddForce (JumpPower * Vector3.up, ForceMode.Force);
			}
			yield return null;
		}
	}

	private void SpeedLimit(Rigidbody rigidbody){
		if(Mathf.Abs(rigidbody.velocity.x) > MaxSpeed){
			rigidbody.velocity = new Vector3(MaxSpeed * (Mathf.Abs (rigidbody.velocity.x) / rigidbody.velocity.x),rigidbody.velocity.y,rigidbody.velocity.z);
		}
	}

	private void CameraLockAtMe(float x){
		if (x >= 0 && screenPos.x > Screen.width / 3) {//画面の左半分半ばに入ればカメラが付いていく。
			StartCoroutine(FindObjectOfType<CameraFocus> ().MoveToFocusTarget ());
		}
	}
}