using UnityEngine;

public class Move : MonoBehaviour
{
    public bool moveVertical;
    public float moveSpeed = 1;
    public float moveLimit = 1;

    float localPositionDelta;
    //float localPositionStart;
    Vector3 localPositionStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        localPositionStart = transform.localPosition;
    }

    bool moveDirectionPositive;

    // Update is called once per frame
    void Update()
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
}
