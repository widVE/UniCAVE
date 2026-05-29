#pragma strict

function Awake()//OPTIMIZED
{	
	GetComponent.<Animation>()["" + GetComponent.<Animation>().clip.name].normalizedTime = Random.Range(0.0, 1.0);
}