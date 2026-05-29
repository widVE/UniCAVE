#pragma strict
#pragma implicit
#pragma downcast
 
var scrollSpeedX = 0.5;
var scrollSpeedY = 0.5;
var materialID = 0;
var updateRate = 0.05;

enum TextureType { _MainTex, _BumpMap, _LightMap, _Cube, _ParticleTexture };
var Type = TextureType._MainTex;

private var materials : Material[]; 
private var offsetX : float;
private var offsetY : float;
private var rate = 0.05;
private var nextUpdate : float;

private var currentTime : float;

function Awake()
{
	enabled = false;
}

function Update()
{
	if(Time.time > nextUpdate)
	{		
		currentTime = Time.time;		
		nextUpdate = currentTime + updateRate;
		
		offsetX = Mathf.Repeat(currentTime * scrollSpeedX, 1);		
		offsetY = Mathf.Repeat(currentTime * scrollSpeedY, 1);	
		
		GetComponent.<Renderer>().materials[materialID].SetTextureOffset("" + Type, Vector2(offsetX, offsetY));
	}
}

function OnBecameVisible ()
{
	enabled = true;
}

function OnBecameInvisible ()
{
	enabled = false;
}