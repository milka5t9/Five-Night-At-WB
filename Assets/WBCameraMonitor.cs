using UnityEngine;

public class WBCameraMonitor : MonoBehaviour
{
    public Camera mainCam;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;

    private WBGameplayManager gameplayManager;
    private bool isMonitorOpen = false;
    private int activeWindow = -1; // -1 = чистый рабочий стол, 0 = Заказы, 1 = Камеры, 2 = Генератор
    private bool startMenuOpen = false;
    private int currentCam = 1;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        gameplayManager = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));

        cam1 = FindAndSetupCam("Cam_01_MainStorage");
        cam2 = FindAndSetupCam("Cam_02_LoadingZone");
        cam3 = FindAndSetupCam("Cam_03_ClientZone");
        cam4 = FindAndSetupCam("Cam_04_Ventilation");

        CloseMonitor();
    }

    Camera FindAndSetupCam(string camName)
    {
        GameObject obj = GameObject.Find(camName);
        if (obj == null) return null;

        Camera c = (Camera)obj.GetComponent(typeof(Camera));
        if (c != null)
        {
            c.enabled = false;
            if (obj.GetComponent(typeof(Light)) == null)
            {
                Light nightLight = (Light)obj.AddComponent(typeof(Light));
                nightLight.type = LightType.Spot;
                nightLight.range = 18f;
                nightLight.spotAngle = 85f;
                nightLight.intensity = 2.6f;
                nightLight.color = new Color(0.75f, 1f, 0.8f);
            }
        }
        return c;
    }

    void Update()
    {
        if (!isMonitorOpen) return;

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMonitor();
            return;
        }

        UpdateCameraState();

        if (activeWindow == 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectCam(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectCam(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectCam(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectCam(4);
        }
    }

    public void OpenMonitor()
    {
        isMonitorOpen = true;
        startMenuOpen = false;
        if (gameplayManager != null) gameplayManager.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateCameraState();
    }

    public void CloseMonitor()
    {
        isMonitorOpen = false;
        startMenuOpen = false;
        DisableAllCams();

        if (mainCam != null) mainCam.enabled = true;
        if (gameplayManager != null) gameplayManager.enabled = true;
    }

    void DisableAllCams()
    {
        if (cam1 != null) cam1.enabled = false;
        if (cam2 != null) cam2.enabled = false;
        if (cam3 != null) cam3.enabled = false;
        if (cam4 != null) cam4.enabled = false;
    }

    void UpdateCameraState()
    {
        WBPowerGenerator gen = (WBPowerGenerator)FindAnyObjectByType(typeof(WBPowerGenerator));
        bool powerOk = (gen == null || gen.IsPowerWorking());

        if (activeWindow == 1 && powerOk)
        {
            if (mainCam != null) mainCam.enabled = false;
            SelectCam(currentCam);
        }
        else
        {
            DisableAllCams();
            if (mainCam != null) mainCam.enabled = true;
        }
    }

    public void SelectCam(int num)
    {
        currentCam = num;
        if (cam1 != null) cam1.enabled = (num == 1);
        if (cam2 != null) cam2.enabled = (num == 2);
        if (cam3 != null) cam3.enabled = (num == 3);
        if (cam4 != null) cam4.enabled = (num == 4);
    }

    public void SetActiveWindow(int winId)
    {
        activeWindow = winId;
        startMenuOpen = false;
        UpdateCameraState();
    }

    void OnGUI()
    {
        if (!isMonitorOpen) return;

        WBPowerGenerator gen = (WBPowerGenerator)FindAnyObjectByType(typeof(WBPowerGenerator));
        bool powerOk = (gen == null || gen.IsPowerWorking());

        WBDesktopOSRenderer.DrawDesktopAndWindows(this, activeWindow, startMenuOpen, powerOk, currentCam, gen, out bool toggleStart);
        if (toggleStart) startMenuOpen = !startMenuOpen;
    }
}
