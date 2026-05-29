using UnityEngine;

namespace UniCAVE
{
    /// <summary>
    /// Moves an object in a circle in local space.
    /// </summary>
    public class Move : MonoBehaviour
    {
        void Update()
        {
            transform.localPosition = new Vector3(Mathf.Sin(Time.time), Mathf.Cos(Time.time), -1);
        }
    }
}