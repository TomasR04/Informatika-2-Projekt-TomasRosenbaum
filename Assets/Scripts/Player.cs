using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public Transform cameraTarget;
    public GameObject cameraObject;

    CinemachineCamera camera;
    CinemachineOrbitalFollow cameraFollow;

    public float moveSpeed = 20f;
    public float lookSensitivity = 1f;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 scrollInput;

    bool middleClicked = false;
    bool shiftPressed = false;

    GameObject selectedObject;

    private void Start()
    {
        camera = cameraObject.GetComponent<CinemachineCamera>();
        cameraFollow = camera.GetComponent<CinemachineOrbitalFollow>();
    }
    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        UpdateMovement(deltaTime);
        UpdateOrbit(deltaTime);

    }
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void OnScrollWheel(InputValue value)
    {
        scrollInput = value.Get<Vector2>();
    }

    void OnMiddleClick(InputValue value)
    {
        middleClicked = value.isPressed;
    }

    void OnSprint(InputValue value)
    {
        shiftPressed = value.isPressed;      
    }

    void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider != null)
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    if (hitObject.CompareTag("Playable"))
                    {
                        selectedObject = hitObject;
                        //Debug.Log("Selected object: " + selectedObject.name);
                        selectedObject.GetComponent<PlayableControler>().Selected();
                    }
                    else
                    {
                        if (selectedObject != null)
                        {
                            selectedObject.GetComponent<PlayableControler>().Deselected();
                        }
                        selectedObject = null;

                        //Debug.Log("No playable object selected.");
                    }
                }
            }
        }

    }

    

    void OnRightClick(InputValue value)
    {
        if (value.isPressed && selectedObject != null)
        {

            if (selectedObject.CompareTag("Playable"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Vector3 targetPosition = hitInfo.point;
                    //Debug.Log("Moving " + selectedObject.name + " to " + targetPosition);
                    selectedObject.GetComponent<PlayableControler>().MoveTo(targetPosition);

                }
            }
        }
    }

    void UpdateMovement(float deltaTime)
    {
        moveSpeed = shiftPressed ? 40f : 20f;
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();
        Vector3 desiredMove = (forward * move.z + right * move.x).normalized;

        cameraTarget.position += desiredMove * moveSpeed * deltaTime;
        // Handle camera rotation
        float yaw = lookInput.x * lookSensitivity;
        cameraTarget.Rotate(0, yaw, 0);
        // Handle camera zoom
        float zoomChange = scrollInput.y * lookSensitivity;
        if (cameraFollow.Radius - zoomChange > 2f)
            cameraFollow.Radius -= zoomChange;

    }

    private void UpdateOrbit(float deltaTime)
    {
        
        Vector2 orbitInput = lookInput * (middleClicked ? 1f : 0f);

        orbitInput *= lookSensitivity;
        InputAxis horizontalAxis = cameraFollow.HorizontalAxis;
        InputAxis verticalAxis = cameraFollow.VerticalAxis;

        horizontalAxis.Value += orbitInput.x;
        verticalAxis.Value -= orbitInput.y;

        //horizontalAxis.Value = Mathf.Clamp(horizontalAxis.Value, horizontalAxis.Range.x, horizontalAxis.Range.y);
        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, 5, verticalAxis.Range.y);

        cameraFollow.HorizontalAxis = horizontalAxis;
        cameraFollow.VerticalAxis = verticalAxis;


    }
}




