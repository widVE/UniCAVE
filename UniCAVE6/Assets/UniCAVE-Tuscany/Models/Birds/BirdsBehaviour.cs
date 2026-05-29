using UnityEngine;
using System.Collections;

public class BirdsBehaviour : MonoBehaviour
{
	public Transform birdsPrefab;
	float birdTimer;
	//ParticleAnimator animator;
	//ParticleEmitter emitter;
	
	void Start()
	{
		//if(QualitySettings.currentLevel < QualityLevel.Good)
		//{
		//	this.enabled = false;
		//	return;
		//}
		birdTimer = Random.Range(2,5);
	}
	
	void Update()
	{
		if(birdTimer < Time.time)
		{
			StartBirds();
		}
	}
	
	void StartBirds()
	{
		transform.position = new Vector3( ((Random.Range(0,2)*2)-1) * Random.Range(65f, 80f), transform.position.y, Random.Range(-30f, -20f));
		transform.LookAt(new Vector3(0, transform.position.y, Random.Range(-80f, -50f)) );
		
		/*Transform birds = (Transform)Instantiate(birdsPrefab, transform.position, transform.rotation);
		animator = birds.GetComponentInChildren(typeof(ParticleAnimator)) as ParticleAnimator;
		animator.force = new Vector3(0, Random.Range(-0.3f, 0.3f), 0);
		emitter = birds.GetComponentInChildren(typeof(ParticleEmitter)) as ParticleEmitter;
		emitter.emit = true;
		
		birdTimer = Time.time + Random.Range(5,20);*/
	}
}
