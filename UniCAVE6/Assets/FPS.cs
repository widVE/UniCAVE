using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
//using static UnityEngine.Rendering.DebugUI;

public class FPS : MonoBehaviour
{
    Text FPSText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FPSText = transform.Find("Text (Legacy)_FPS").GetComponent<Text>();
    }

    float averageTime;
    int frames;
    int averageFPS;

    // Update is called once per frame
    void Update()
    {
        int FPS = (int)(1 / Time.deltaTime);
        frames += 1;
        averageTime += Time.deltaTime;

        if (averageTime > 1) //calculate average FPS per 1 second
        {
            averageFPS = (int)(frames / averageTime);
            averageTime = frames = 0;
        }

        FPSText.text = averageFPS.ToString() + " FPS";
    }
}
