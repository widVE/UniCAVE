#pragma strict

private var yRot : float;
private var xRot : float;

function Update ()
{
	xRot += Input.GetAxis("Vertical1") * 1.5;
	yRot += Input.GetAxis("Horizontal1") * 1.5;
	
	transform.localEulerAngles.x = xRot;
	transform.localEulerAngles.y = yRot;
	
	//print(Input.GetAxis("Horizontal1"));	
}