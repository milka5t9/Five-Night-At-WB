using UnityEngine;

public class WBGameplayManager : MonoBehaviour
{
    public float mouseSensitivity = 2.0f;
    public float smoothSpeed = 14.0f;
    public Light scannerLight;
    public Camera mainCam;

    private float targetYaw = 0f;
    private float targetPitch = 0f;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (scannerLight != null) scannerLight.enabled = false;
    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (mainCam != null)
        {
            Vector3 angles = mainCam.transform.localEulerAngles;
            targetYaw = angles.y;
            currentYaw = angles.y;
        }
    }

    void OnDisable()
    {
        if (scannerLight != null) scannerLight.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        HandleSmoothLook();
        HandleScannerLight();
    }

    void HandleSmoothLook()
    {
        targetYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        targetPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        targetPitch = Mathf.Clamp(targetPitch, -65f, 65f);

        currentYaw = Mathf.Lerp(currentYaw, targetYaw, Time.deltaTime * smoothSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);

        if (mainCam != null)
        {
            mainCam.transform.localEulerAngles = new Vector3(currentPitch, currentYaw, 0f);
        }
    }

    void HandleScannerLight()
    {
        if (scannerLight == null) return;
        scannerLight.enabled = Input.GetMouseButton(0);
    }
}
