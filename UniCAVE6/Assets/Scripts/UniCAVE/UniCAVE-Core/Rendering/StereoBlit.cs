using UnityEngine;

namespace UniCAVE
{
    /// <summary>
    /// Copy non-stereo camera output to stereo camera output
    /// Essentially allows two cameras to render to Unity's passive stereo pipeline
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class StereoBlit : MonoBehaviour
    {
        public Camera lcam;
        public Camera rcam;

        private Camera _cam;

        /// <summary>
        /// Assign my camera and create target camera render textures
        /// </summary>
        private void Start()
        {
            _cam = GetComponent<Camera>();
            _cam.depth = 100;

            if(lcam.targetTexture == null)
            {
                lcam.targetTexture = new RenderTexture(lcam.pixelWidth, lcam.pixelHeight, 1)
                {
                    name = Util.ObjectFullName(gameObject) + "_LeftTexture"
                };
            }
            if(rcam.targetTexture == null)
            {
                rcam.targetTexture = new RenderTexture(rcam.pixelWidth, rcam.pixelHeight, 1)
                {
                    name = Util.ObjectFullName(gameObject) + "_RightTexture"
                };
            }
        }

        /// <summary>
        /// Copy non-stereo camera texture to stereo camera left/right texture
        /// </summary>
        /// <param name="source">Unused parameter (ordinarily used for post processing)</param>
        /// <param name="destination">Texture associated with either left or right stereo eye, depending on pipeline phase</param>
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(
                _cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ? lcam.targetTexture : rcam.targetTexture,
                destination);
        }
    }
}