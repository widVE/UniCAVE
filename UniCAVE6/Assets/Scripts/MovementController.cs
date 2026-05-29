using UniCAVE;
using Unity.Netcode;
using UnityEngine;

public class MovementController : NetworkBehaviour
{
    /// <summary>
    /// The speed at which the character moves
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// Host Machine Name is needed to only run the script on the host machine
    /// </summary>
    [SerializeField] private MachineName HeadMachineAsset;
    public string HeadMachine => MachineName.GetMachineName(HeadMachineAsset);

    /// <summary>
    /// Should the vertical navigation be disabled?
    /// </summary>
    public bool DisableVerticalNavigation;

    /// <summary>
    /// Transform of a cave so the location can be send to IGs todo: make it sync with netcode
    /// </summary>
    private Transform _caveGameObject;

    /// <summary>
    /// Find the CAVE object
    /// </summary>
    private void Start()
    {
        _caveGameObject = GameObject.Find("CAVE").GetComponent<Transform>();
    }

    public float rotationSpeed = 100f;

    /// <summary>
    /// Get input from controller and upate the position in the cave
    /// </summary>
    private void Update()
    {
        if (Util.GetMachineName() != HeadMachine)
        {
            return;
        }

        float horizontal_joystick = Input.GetAxis("Horizontal");
        float vertical_joystick = Input.GetAxis("Vertical");

        float vertical_movement = 0;
        if (!DisableVerticalNavigation)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetButton("joystick button 3"))
            {
                vertical_movement = 1;
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetButton("joystick button 2"))
            {
                vertical_movement = -1;
            }
        }

        Vector3 forwardMovement = _caveGameObject.forward * vertical_joystick;

        // Combine the forward, right, and vertical movement components
        Vector3 direction = forwardMovement + new Vector3(0, vertical_movement, 0);

        // Normalize the direction vector to ensure consistent movement speed
        direction.Normalize();
        _caveGameObject.position += moveSpeed * Time.deltaTime * direction;

        // Apply rotation to the character
        _caveGameObject.Rotate(0, horizontal_joystick * rotationSpeed * Time.deltaTime, 0);

        UpdateIgPositionClientRpc(_caveGameObject.position.x, _caveGameObject.position.y, _caveGameObject.position.z, _caveGameObject.rotation.eulerAngles.x, _caveGameObject.rotation.eulerAngles.y, _caveGameObject.rotation.eulerAngles.z);
    }

    [ClientRpc]
    private void UpdateIgPositionClientRpc(float _x, float _y, float _z, float _rotX, float _rotY, float _rotZ)
    {
        //_caveGameObject.SetPositionAndRotation(new Vector3(_x, _y, _z), Quaternion.Euler(_rotX, _rotY, _rotZ));
        _caveGameObject.position = new Vector3(_x, _y, _z);

        _caveGameObject.eulerAngles = new Vector3(_rotX, _rotY, _rotZ);
    }

    private void OnDestroy()
    {
        UpdateIgPositionClientRpc(0, 0, 0, 0, 0, 0);
    }
}