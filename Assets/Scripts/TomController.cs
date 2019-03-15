﻿using UnityEngine;
using System.Collections;

public class TomController : MonoBehaviour {

	private float verticalVelocity;
	private float horizontalVelocity;
	private Animator myAnimator;
private CameraFollowPlayersScript followScript;
	public float tomMaxSpeed=12f;
	public AudioClip JumpSound = null;
	public AudioClip HitSound = null;
	public AudioClip CoinSound = null;

	private Rigidbody mRigidBody = null;
	private AudioSource mAudioSource = null;
	private bool mFloorTouched = false;

	public float tomSpeedScale=100f;
	private Vector3 lookRot;
	private Quaternion lookTo;
	public float timeToEatRat=4f;
	private float time;
	private bool isEating;
	private ParticleSystem myParticle;
	private ParticleSystem dashParticle;
	private float timeToCatchJerry=0.2f;
	private float timeCatch;
	private bool isJumping;
	private float jumpTime;
	void Start () {
		followScript=Camera.main.GetComponent<CameraFollowPlayersScript>();
		dashParticle=GameObject.Find("CatDashParticle").GetComponent<ParticleSystem>();
		mRigidBody = GetComponent<Rigidbody> ();
		mAudioSource = GetComponent<AudioSource> ();
		myAnimator=transform.GetChild(0).GetComponent<Animator>();
		myParticle=GameObject.Find("CatCatchRatParticle").GetComponent<ParticleSystem>();
	}

	void FixedUpdate () {
		
		verticalVelocity=Mathf.Abs(mRigidBody.velocity.z);
		horizontalVelocity=Mathf.Abs(mRigidBody.velocity.x);
		if (mRigidBody != null && !isEating ) {
			if (Input.GetAxis ("VerticalTom")!=0 && verticalVelocity<tomMaxSpeed) {
				mRigidBody.AddForce(Vector3.forward * Input.GetAxis("VerticalTom")*tomSpeedScale);
			}
			if (Input.GetAxis ("HorizontalTom")!=0 && horizontalVelocity<tomMaxSpeed) {
				mRigidBody.AddForce(Vector3.right * Input.GetAxis("HorizontalTom")*tomSpeedScale);
			}
			if(Input.GetAxis("VerticalTom")==0){
				mRigidBody.velocity=new Vector3(mRigidBody.velocity.x,mRigidBody.velocity.y,0);
			}
			if(Input.GetAxis("HorizontalTom")==0 ){
				mRigidBody.velocity=new Vector3(0,mRigidBody.velocity.y,mRigidBody.velocity.z);
			}
			if(Input.GetButtonDown("Jump") && !isJumping){
				mRigidBody.velocity=Vector3.zero;
				myAnimator.SetBool("isJumping", true);
				dashParticle.emissionRate=30;
				isJumping=true;
			}
			if(mRigidBody.velocity.magnitude<0.1f){
				myAnimator.SetBool("isWalk", false);
			}else{
				myAnimator.SetBool("isWalk", true);
			}
			RotateCharacter();
		}
		Jumping();
		IsEatingRatAction();
	}

	void IsEatingRatAction(){
		if(isEating){
			time+=Time.deltaTime;
			if(time>=timeToEatRat){
				//tomMaxSpeed+=1f;
				myAnimator.SetBool("isAttack", false);
				time=0;
				myParticle.emissionRate=0;
				isEating=false;
			}
		}
	}

	void Jumping(){
		if(isJumping){
			jumpTime+=Time.deltaTime;
			mRigidBody.AddForce(transform.forward*250f);
			if(jumpTime>=0.4f){
				dashParticle.emissionRate=0;
				myAnimator.SetBool("isJumping", false);
				jumpTime=0;
				mRigidBody.velocity=Vector3.zero;
				isJumping=false;
			}
		}
	}

	void OnCollisionStay(Collision coll){
		// if (coll.gameObject.tag.Equals ("Floor")) {
		// 	mFloorTouched = true;
		// 	if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
		// 		//mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
		// 	}
		// } else {
		// 	if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f) {
		// 		//mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
		// 	}
		// } 
		if (coll.gameObject.CompareTag ("Jerry") && !isEating && isJumping) {
			if(mAudioSource != null && CoinSound != null){
				mAudioSource.PlayOneShot(CoinSound);
			}
			isJumping=false;
			dashParticle.emissionRate=0;
			myAnimator.SetBool("isAttack", true);
			myAnimator.SetBool("isJumping", false);
			isEating=true;
			mRigidBody.velocity=Vector3.zero;
			myParticle.emissionRate=10;
			followScript.RemoveObjectFromList(coll.gameObject);
			//Destroy(coll.gameObject);	
			coll.gameObject.SetActive(false);		
		}
	}

	void RotateCharacter(){
		//if(!isJumping){
		if(Input.GetAxis("HorizontalTom")!=0 || Input.GetAxis("VerticalTom")!=0){
	lookRot=new Vector3( Input.GetAxis("HorizontalTom"), 0,Input.GetAxis("VerticalTom"));
	lookTo=Quaternion.LookRotation(lookRot);
	transform.rotation = Quaternion.Slerp(transform.rotation, lookTo, 10 * Time.deltaTime);
	//}	
		}
	
	

}

	void OnCollisionExit(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = false;
		}
	}

	void OnCollision(Collision other) {
		if (other.collider.CompareTag ("Jerry")) {
			if(mAudioSource != null && CoinSound != null){
				mAudioSource.PlayOneShot(CoinSound);
			}
			Destroy(other.gameObject);
		}
	}
}
