#pragma strict

var Waypoint : Transform[];
var Speed : float;
var TurnSpeed : float;

private var CurrentWaypoint : int;

function Start()
{
	CurrentWaypoint = 0;
}

function FixedUpdate()
{
	var lookDirection : Vector3 = Waypoint[CurrentWaypoint].position - transform.position;
    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection.normalized), Time.time * (TurnSpeed/1000));
    	
	transform.localEulerAngles.x = 270; 
	transform.localEulerAngles.z = 0; 
	
	transform.Translate(-Vector3.up * Time.deltaTime * Speed, Space.Self);
	
	var distance : float = Vector3.Distance(Waypoint[CurrentWaypoint].position, transform.position);	
	
	if(distance < 5)
	{
		CurrentWaypoint++;
		
		if(CurrentWaypoint >= Waypoint.Length)
		CurrentWaypoint = 0;	
	}
}