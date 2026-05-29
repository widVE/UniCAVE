using UnityEngine;

public class S3DScreen : MonoBehaviour
{
    public Stereo3D S3DScript;
    public RenderTexture renderTexture_left;
    public RenderTexture renderTexture_right;
    public Material material;
    public Camera S3DCamera;
    //public Transform S3DScreenCenter;
    public bool moveScreen;
    public bool moveVertical;
    public float moveSpeed = 1;
    public float moveLimit = 1;
    public int setRenderTexturesDelayedFrames = 1;

    bool S3DEnabled;
    float localPositionDelta;
    Vector3 localPositionStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    material = GetComponent<Renderer>().material;

        if (S3DScript)
	    {
      ////      //float formula =  25.4f / 960000;
      ////      float formula =  25.4f / S3DScript.PPI / 10000;
      ////      transform.localScale = new Vector3(Screen.width * formula, 1, Screen.height * formula);
		    //renderTexture_left = S3DScript.renderTexture_left;
		    //renderTexture_right = S3DScript.renderTexture_right;
	     //   material.SetTexture("_Texture2D_left", renderTexture_left);
	     //   material.SetTexture("_Texture2D_right", renderTexture_right);

            S3DCamera = S3DScript.GetComponent<Camera>();
            //transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
            //S3DScreenCenter = S3DCamera.GetComponentInParent<Transform>();
            //S3DScreenCenter = S3DScript.trackingAnchor;
            S3DEnabled = S3DScript.S3DEnabled;
            //S3DCamera.enabled = false;

            SetRenderTextures();
	    }

        //SetRenderTextures();

        //localPositionStart = transform.localPosition;
    }

    //bool done;
    bool moveDirectionPositive;
    bool S3DScriptWasDisabledInLastFrame;
    bool setScreenHeightOnce;

    // Update is called once per frame
    void Update()
    {
        //if (Time.time > 5)
        //    S3DScript.tracking = true;

        if (!setScreenHeightOnce)
        {
            //Invoke("SetScreenPosition", Time.deltaTime);
            SetScreenPosition();
        }
        else
            if (moveScreen)
            {
                if (moveDirectionPositive)
                    localPositionDelta += moveSpeed * Time.deltaTime;
                else
                    localPositionDelta -= moveSpeed * Time.deltaTime;

                if (moveDirectionPositive && localPositionDelta >= moveLimit || !moveDirectionPositive && localPositionDelta <= -moveLimit)
                    moveDirectionPositive = !moveDirectionPositive;

                if (moveVertical)
                    transform.localPosition = new Vector3(transform.localPosition.x, localPositionStart.y + localPositionDelta, transform.localPosition.z);
                else
                    transform.localPosition = new Vector3(localPositionStart.x + localPositionDelta, transform.localPosition.y, transform.localPosition.z);
            }

        //if (!done && Time.time > 5)
        if (S3DScript)
        {
            if (S3DScript.isActiveAndEnabled)
            {
                //done = true;
                //SetRenderTextures();
                float formula =  25.4f / S3DScript.PPI / 10000;
                transform.localScale = new Vector3(Screen.width * formula, 1, Screen.height * formula);
                //S3DScreenCenter.position = transform.position;

                if (S3DScript.tracking)
                {
                    S3DCamera.transform.position = Camera.main.transform.position;
                    //S3DScript.TrackingRotationHelper_SetRotation(Camera.main.transform.localRotation);
                    S3DScript.TrackingRotationHelper_SetRotation(Quaternion.Inverse(S3DScript.transform.localRotation) * Camera.main.transform.localRotation);
                }


                float roundedStereoSeparationInMillimeters = Mathf.Round(S3DCamera.stereoSeparation * 1000);

                if (roundedStereoSeparationInMillimeters != S3DScript.userIPD)
                {
                    Debug.Log($"roundedStereoSeparationInMillimeters {roundedStereoSeparationInMillimeters} S3DScript.userIPD {S3DScript.userIPD}");
                    S3DScript.userIPD = roundedStereoSeparationInMillimeters;
                }

                if (S3DScriptWasDisabledInLastFrame)
                {
                    S3DScriptWasDisabledInLastFrame = false;
                    Invoke("SetRenderTexturesDelayed", Time.deltaTime * setRenderTexturesDelayedFrames);
                }
                else
                    if (S3DEnabled != S3DScript.S3DEnabled)
                    {
                        Debug.Log("S3DEnabled != S3DScript.S3DEnabled");
                        //S3DEnabled = S3DScript.S3DEnabled;
                        SetRenderTextures();
                    }

                //S3DScript.TrackingData_Set(Camera.main.transform.localPosition, Camera.main.transform.localRotation);
                //S3DScript.TrackingData_Set(Camera.main.transform.position, Camera.main.transform.rotation);
            }
            else
            {
                //Debug.Log("!S3DScript.isActiveAndEnabled");
                S3DScriptWasDisabledInLastFrame = true;
            }

            S3DEnabled = S3DScript.S3DEnabled;
            //Debug.Log("S3DEnabled " + S3DEnabled);
        }
    }

    //void LateUpdate()
    //{
    //    if (!setScreenHeightOnce)
    //    {
    //        //Invoke("SetScreenPosition", Time.deltaTime);
    //        SetScreenPosition();
    //    }
    //}

    void SetScreenPosition()
    {
        if (Camera.main.transform.localPosition.y != 0)
        //if (S3DCamera && S3DCamera.transform.localPosition.y != 0)
        {
            setScreenHeightOnce = true;
            //transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = new Vector3(transform.localPosition.x, Camera.main.transform.localPosition.y, transform.localPosition.z);
            localPositionStart = transform.localPosition;
            //Debug.Log("transform.localPosition.y " + transform.localPosition.y);

            if (S3DCamera)
                //S3DCamera.transform.localPosition = Camera.main.transform.localPosition;
                S3DCamera.transform.localPosition = new Vector3(S3DCamera.transform.localPosition.x, Camera.main.transform.localPosition.y, S3DCamera.transform.localPosition.z);

            //S3DScript.tracking = true;
            Invoke("EnableTracking", Time.deltaTime); //let BackupCamPosition() update with changed S3DCamera.transform.localPosition here before EnableTracking
        }
    }

    void EnableTracking()
    { 
        if (S3DScript)
            S3DScript.tracking = true;
    }

    void SetRenderTextures()
    {
        S3DEnabled = S3DScript.S3DEnabled;

        if (S3DEnabled)
        {
		    renderTexture_left = S3DScript.renderTexture_left;
		    renderTexture_right = S3DScript.renderTexture_right;
            //S3DCamera.enabled = false;
        }
        else
        {
		    renderTexture_left = S3DScript.renderTexture;
		    renderTexture_right = S3DScript.renderTexture;
        }

	    material.SetTexture("_Texture2D_left", renderTexture_left);
	    material.SetTexture("_Texture2D_right", renderTexture_right);
    }

    void SetRenderTexturesDelayed()
    {
        Debug.Log("SetRenderTexturesDelayed");

        if (S3DScript.isActiveAndEnabled)
            SetRenderTextures();
    }

    //void OnPostRenderSetTexture()
    //{
    //    SetRenderTextures();
    //}

    //void SetRenderTextures()
    //{
    //    if (S3DScript)
	   // {
    //        //float formula =  25.4f / 960000;
    //        float formula =  25.4f / S3DScript.PPI / 10000;
    //        transform.localScale = new Vector3(Screen.width * formula, 1, Screen.height * formula);
    //        S3DScreenCenter.position = transform.position;
    //        S3DCamera.transform.position = Camera.main.transform.position;
		  //  //renderTexture_left = S3DScript.renderTexture_left;
		  //  //renderTexture_right = S3DScript.renderTexture_right;
	   //  //   material.SetTexture("_Texture2D_left", renderTexture_left);
	   //  //   material.SetTexture("_Texture2D_right", renderTexture_right);
	   //     //material.SetTexture("_Texture2D_left", S3DScript.S3DMaterial.GetTexture("_LeftTex"));
	   //     //material.SetTexture("_Texture2D_right", S3DScript.S3DMaterial.GetTexture("_RightTex"));
	   // }
    //}
}
