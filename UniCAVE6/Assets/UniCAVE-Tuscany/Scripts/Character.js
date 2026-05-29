#pragma strict

var Controller : CharacterController;
var FollowTarget : Transform;


function Update()
{
	transform.rotation =  Quaternion.Slerp(transform.rotation, FollowTarget.rotation, Time.deltaTime * 10);
		
	if(Controller.velocity.magnitude > 0.2)
	{
		GetComponent.<Animation>().CrossFade("Walk", 0.2);
	}
	else
	{
		GetComponent.<Animation>().CrossFade("Idle", 0.2);	
	}
}