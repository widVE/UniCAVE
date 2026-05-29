using UnityEngine;

namespace UniCAVE
{
    /// <summary>
    /// Add this script to a GameObject that has a UCNetwork component
    /// The referenced GameObject toEnable will be set to enabled once the UCNetwork reports itself as initialized
    /// </summary>
    public class EnableOnCaveInitialized : MonoBehaviour
    {
        public GameObject toEnable;

        private void Start()
        {
            if (toEnable == null || gameObject.GetComponent<UCNetwork>() == null)
            {
                Object.Destroy(this);
            }
        }

        void Update()
        {
            if (!gameObject.GetComponent<UCNetwork>().Initialized)
            {
                return;
            }
            toEnable.SetActive(true);
            Debug.Log("SetActive " + toEnable);
            Object.Destroy(this);
        }
    }
}