#pragma strict

var CustomGrass : GameObject;

private var Grass = false;
private var DensityLevel = 1.0;

function Start ()
{
	Grass = false;
	
	var qualityLevel = QualitySettings.GetQualityLevel();
		
	switch (qualityLevel)
	{
		case 0:
			DensityLevel = 0.0;
			break;
			
		case 1:
			DensityLevel = 0.2;
			break;
		
		case 2:
			DensityLevel = 0.4;
			break;
			
		case 3:
			DensityLevel = 0.6;
			break;
			
		case 3:
			DensityLevel = 0.8;
			break;
			
		case 4:
			DensityLevel = 1.0;
			break;
			
		case 5:
			DensityLevel = 1.0;
			break;
	}
	
	//turn off grass by default
	Terrain.activeTerrain.detailObjectDensity = 0.0;
	CustomGrass.SetActive(false);	
}	

function Update ()
{
	if(Input.GetKeyDown("g"))
	{	
		if(Grass)
		{
			Grass = false;
			Terrain.activeTerrain.detailObjectDensity = 0.0;
			CustomGrass.SetActive(false);
		}
		else
		{
			Grass = true;
			Terrain.activeTerrain.detailObjectDensity = DensityLevel;
			CustomGrass.SetActive(true);
		}
	}
}