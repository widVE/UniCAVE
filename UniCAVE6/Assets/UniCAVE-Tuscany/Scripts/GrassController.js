#pragma strict

private var target : Transform;


function Start()
{
	target = GameObject.Find("OVRCameraController").transform;
}

function Update()
{	
	if(target != null)
	{
		for (child in transform)
		{
			var typedChild : Transform = child as Transform;
			typedChild.LookAt(target);
		}
	}
}