using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class StereoBlit : MonoBehaviour {
    public Camera lcam;
    public Camera rcam;

    private Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
        cam.depth = 100;

        lcam.targetTexture = new RenderTexture(lcam.pixelWidth, lcam.pixelHeight, 1);
        rcam.targetTexture = new RenderTexture(rcam.pixelWidth, rcam.pixelHeight, 1);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if(cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left) {
            Graphics.Blit(lcam.targetTexture, destination);
        } else {
            Graphics.Blit(rcam.targetTexture, destination);
        }
    }
}
