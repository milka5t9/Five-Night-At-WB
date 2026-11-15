using UnityEngine;

public class WBWorkstationAndQR : MonoBehaviour
{
    private Camera mainCam;
    private WBCameraMonitor camMonitor;
    private GameObject pcScreenObj;
    private bool isLookingAtPC = false;

    void Start()
    {
        mainCam = Camera.main;
        camMonitor = (WBCameraMonitor)FindAnyObjectByType(typeof(WBCameraMonitor));
        BuildDeskComputer();
    }

    void BuildDeskComputer()
    {
        GameObject oldMon = GameObject.Find("PC_Monitor");
        if (oldMon != null) oldMon.SetActive(false);
        GameObject oldStand = GameObject.Find("PC_Stand");
        if (oldStand != null) oldStand.SetActive(false);

        GameObject oldPC = GameObject.Find("WB_Desk_Computer");
        if (oldPC != null) Destroy(oldPC);

        GameObject pcRoot = new GameObject("WB_Desk_Computer");
        pcRoot.transform.position = new Vector3(-0.72f, 1.16f, -0.95f);
        pcRoot.transform.rotation = Quaternion.Euler(0f, 22f, 0f);

        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");

        Material darkPlastic = new Material(sh);
        darkPlastic.color = new Color(0.12f, 0.12f, 0.14f);

        CreatePart("Stand_Base", new Vector3(0f, 0.01f, 0f), new Vector3(0.22f, 0.02f, 0.18f), darkPlastic, pcRoot.transform);
        CreatePart("Stand_Neck", new Vector3(0f, 0.12f, 0.04f), new Vector3(0.06f, 0.22f, 0.05f), darkPlastic, pcRoot.transform);
        CreatePart("Monitor_Frame", new Vector3(0f, 0.32f, 0.02f), new Vector3(0.58f, 0.36f, 0.04f), darkPlastic, pcRoot.transform);

        pcScreenObj = CreatePart("Monitor_Screen_Click", new Vector3(0f, 0.32f, -0.005f), new Vector3(0.53f, 0.31f, 0.01f), darkPlastic, pcRoot.transform);
        Material screenMat = new Material(sh);
        screenMat.color = new Color(0.45f, 0.1f, 0.55f);
        screenMat.EnableKeyword("_EMISSION");
        screenMat.SetColor("_EmissionColor", new Color(0.5f, 0.15f, 0.6f));
        ((Renderer)pcScreenObj.GetComponent(typeof(Renderer))).material = screenMat;

        CreatePart("Keyboard", new Vector3(0f, 0.01f, -0.22f), new Vector3(0.42f, 0.015f, 0.14f), darkPlastic, pcRoot.transform);
    }

    GameObject CreatePart(string name, Vector3 localPos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = scale;
        ((Renderer)obj.GetComponent(typeof(Renderer))).material = mat;
        return obj;
    }

    void Update()
    {
        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null && !gm.enabled)
        {
            isLookingAtPC = false;
            return;
        }

        if (camMonitor == null) camMonitor = (WBCameraMonitor)FindAnyObjectByType(typeof(WBCameraMonitor));

        // Открытие компьютера с 3 вкладками на клавишу F или Tab
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Tab)) && camMonitor != null)
        {
            camMonitor.OpenMonitor();
            return;
        }

        // Открытие при наведении прицела на монитор (ЛКМ или E)
        isLookingAtPC = false;
        if (mainCam != null && pcScreenObj != null)
        {
            Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 3.0f))
            {
                if (hit.transform.gameObject == pcScreenObj || hit.transform.parent == pcScreenObj.transform.parent)
                {
                    isLookingAtPC = true;
                    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && camMonitor != null)
                    {
                        camMonitor.OpenMonitor();
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        if (!isLookingAtPC) return;

        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = 18;
        st.fontStyle = FontStyle.Bold;
        st.alignment = TextAnchor.MiddleCenter;
        st.normal.textColor = Color.yellow;
        GUI.Label(new Rect(Screen.width / 2f - 260f, Screen.height / 2f + 25f, 520f, 35f), "[ЛКМ / E / F] Открыть терминал WB (Заказы / Камеры / Энергия)", st);
    }
}
