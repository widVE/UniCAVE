#pragma strict

private var OldPosition : Vector3;
var Rate = 0.35;
var VisualMesh : GameObject;
private var NextHeading : float;
private var Heading : Vector3;
private var LookAtPos : Vector3;

private var NextWingFlap : float; 
private var WingPos = 1;

function Start ()
{
	OldPosition = transform.position;
	NextHeading = Time.time;
}

function Update ()
{
	if(Time.time > NextHeading)
	{
		NextHeading = Time.time + Rate;
		Heading = transform.position - OldPosition;		
		
		OldPosition = transform.position;
	}
		
	Debug.DrawRay(transform.position, Heading * 10, Color.green);
	
	LookAtPos = Vector3.Lerp(LookAtPos, transform.position + (Heading * 10), Time.deltaTime * 5);
	
	transform.LookAt(LookAtPos);  
	
	/*
	if(Time.time > NextWingFlap)
	{
		NextWingFlap = Time.time + Random.Range(0.01, 0.05);
		WingPos *= -1;	
		
		if(WingPos > 0)		
			VisualMesh.animation.Play("WingUp", PlayMode.StopAll);
		else		
			VisualMesh.animation.Play("WingDown", PlayMode.StopAll);
	}	
	*/
}