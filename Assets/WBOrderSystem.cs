using UnityEngine;

public class WBOrderSystem : MonoBehaviour
{
    public float walkSpeed = 3.2f;
    public Light scannerLight;

    private Camera mainCam;
    private GameObject customerObj;
    private WBProceduralAnimator customerAnim;
    private GameObject phoneScreenObj;
    private GameObject[] orderBoxes;
    private Light boxHighlightLight;

    private int state = 0;
    private GameObject targetBox;
    private int targetCellNum = 1;
    private Vector3 targetBoxHomePos;
    private float scanTimer = 0f;
    private float waitTimer = 1.2f;
    private int completedOrders = 0;
    private string statusMessage = "";

    private Vector3 counterDropPos = new Vector3(0.15f, 1.31f, -0.95f);
    private Vector3 counterClientPos = new Vector3(0f, 0f, -0.32f);
    private Vector3 clientSpawnPos = new Vector3(-5.5f, 0f, 0f);

    void Start()
    {
        mainCam = Camera.main;

        GameObject oldParcel = GameObject.Find("WB_Detailed_Parcel");
        if (oldParcel != null) oldParcel.SetActive(false);

        if (mainCam != null)
        {
            Transform tr = mainCam.transform.Find("Scanner_Laser");
            if (tr != null) scannerLight = (Light)tr.GetComponent(typeof(Light));
            WBCharacterModels.BuildScannerInHand(mainCam.transform);
        }

        SetupHorrorLighting();

        GameObject hlObj = new GameObject("Box_Soft_Highlight");
        boxHighlightLight = (Light)hlObj.AddComponent(typeof(Light));
        boxHighlightLight.type = LightType.Point;
        boxHighlightLight.range = 1.5f;
        boxHighlightLight.intensity = 2.6f;
        boxHighlightLight.color = new Color(1f, 0.85f, 0.4f);
        boxHighlightLight.enabled = false;

        orderBoxes = new GameObject[6];
        for (int i = 0; i != 6; i++)
        {
            orderBoxes[i] = GameObject.Find("Order_Box_" + (i + 1));
        }

        SpawnNewCustomer();
    }

    void SetupHorrorLighting()
    {
        RenderSettings.ambientLight = new Color(0.03f, 0.03f, 0.04f);
        Light[] allLights = (Light[])FindObjectsByType(typeof(Light), FindObjectsSortMode.None);
        foreach (Light l in allLights)
        {
            if (l.type == LightType.Directional) l.intensity = 0.05f;
        }

        if (GameObject.Find("Reception_Work_Lamp") == null)
        {
            GameObject lamp = new GameObject("Reception_Work_Lamp");
            lamp.transform.position = new Vector3(0f, 3.0f, -0.8f);
            Light l = (Light)lamp.AddComponent(typeof(Light));
            l.type = LightType.Point;
            l.range = 6.5f;
            l.intensity = 2.4f;
            l.color = new Color(1f, 0.94f, 0.88f);
        }
    }

    void SpawnNewCustomer()
    {
        GameObject old1 = GameObject.Find("WB_Customer_NPC");
        if (old1 != null) Destroy(old1);
        GameObject old2 = GameObject.Find("WB_Customer_Detailed");
        if (old2 != null) Destroy(old2);

        Texture2D phoneQR = BuildPhoneQRTexture(Random.Range(10, 999));
        customerObj = WBCharacterModels.CreateCustomerWithPhone(clientSpawnPos, phoneQR);
        customerAnim = (WBProceduralAnimator)customerObj.GetComponent(typeof(WBProceduralAnimator));

        Transform scr = customerObj.transform.Find("Rig/ArmR_Pivot/Customer_Phone/Phone_QR_Screen");
        if (scr != null) phoneScreenObj = scr.gameObject;
    }

    Texture2D BuildPhoneQRTexture(int seed)
    {
        int size = 28;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        for (int y = 0; y != size; y++)
        {
            for (int x = 0; x != size; x++)
            {
                bool isWhiteFrame = (x == 0 || y == 0 || x == size - 1 || y == size - 1);
                bool isCorner = (Mathf.Max(x, 6) == 6 && Mathf.Max(y, 6) == 6) || (Mathf.Min(x, size - 7) == size - 7 && Mathf.Max(y, 6) == 6);
                bool dark = ((x * 13 + y * 29 + seed) % 3 == 0);
                tex.SetPixel(x, y, (!isWhiteFrame && (isCorner || dark)) ? Color.black : Color.white);
            }
        }
        tex.Apply();
        return tex;
    }

    void Update()
    {
        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null && !gm.enabled) return;

        WBMonsterSystem ms = (WBMonsterSystem)FindAnyObjectByType(typeof(WBMonsterSystem));
        if (ms != null && ms.IsPlayerDead()) return;

        HandleMovement();
        HandleOrderFlow();
    }

    void HandleMovement()
    {
        if (mainCam == null) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = mainCam.transform.forward;
        Vector3 right = mainCam.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 nextPos = mainCam.transform.position + (forward * v + right * h) * walkSpeed * Time.deltaTime;
        nextPos.x = Mathf.Clamp(nextPos.x, -2.3f, 2.3f);
        nextPos.z = Mathf.Clamp(nextPos.z, -7.8f, -1.65f);
        nextPos.y = 1.65f;

        mainCam.transform.position = nextPos;
    }
    void HandleOrderFlow()
    {
        boxHighlightLight.enabled = false;

        if (state == 0)
        {
            if (customerAnim != null)
            {
                customerAnim.isWalking = false;
                customerAnim.isShowingPhone = false;
            }

            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                statusMessage = "Ожидание следующего покупателя...";
                return;
            }

            if (customerAnim != null) customerAnim.isWalking = true;
            Vector3 moveDir = (counterClientPos - customerObj.transform.position).normalized;
            if (moveDir != Vector3.zero)
            {
                customerObj.transform.rotation = Quaternion.Slerp(customerObj.transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 8f);
            }

            customerObj.transform.position = Vector3.MoveTowards(customerObj.transform.position, counterClientPos, 2.0f * Time.deltaTime);
            statusMessage = "К стойке подходит покупатель...";

            if (Vector3.Distance(customerObj.transform.position, counterClientPos) == 0f)
            {
                scanTimer = 0f;
                state = 1;
            }
        }
        else if (state == 1)
        {
            customerObj.transform.rotation = Quaternion.Slerp(customerObj.transform.rotation, Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * 8f);
            if (customerAnim != null)
            {
                customerAnim.isWalking = false;
                customerAnim.isShowingPhone = true;
            }

            statusMessage = "Покупатель показывает QR-код в телефоне! Наведите сканер на экран телефона и ЗАЖМИТЕ ЛКМ на 2 сек";

            if (Input.GetMouseButton(0) && phoneScreenObj != null)
            {
                Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 3.2f))
                {
                    if (hit.transform.gameObject == phoneScreenObj || hit.transform.parent == phoneScreenObj.transform.parent)
                    {
                        scanTimer += Time.deltaTime;
                        if (Mathf.Max(scanTimer, 2.0f) == scanTimer)
                        {
                            scanTimer = 0f;
                            state = 2;
                        }
                        return;
                    }
                }
            }
            scanTimer = 0f;
        }
        else if (state == 2)
        {
            if (customerAnim != null) customerAnim.isShowingPhone = false;
            statusMessage = "QR-код считан! Откройте компьютер на столе ([ЛКМ / E / F]) и в программе WB-ПВЗ нажмите «Принять»";
        }
        else if (state == 3)
        {
            statusMessage = "Заказ принят! Идите назад на склад хранения, найдите подсвеченную коробку (Ячейка №" + targetCellNum + ") и нажмите [E]";

            if (targetBox != null)
            {
                boxHighlightLight.enabled = true;
                boxHighlightLight.transform.position = targetBox.transform.position + new Vector3(0f, 0.35f, 0f);

                float dist = Vector3.Distance(mainCam.transform.position, targetBox.transform.position);
                if (Input.GetKeyDown(KeyCode.E) && Mathf.Min(dist, 2.8f) == dist)
                {
                    boxHighlightLight.enabled = false;
                    targetBox.transform.SetParent(mainCam.transform);
                    targetBox.transform.localPosition = new Vector3(-0.30f, -0.26f, 0.62f);
                    targetBox.transform.localRotation = Quaternion.identity;
                    state = 4;
                }
            }
        }
        else if (state == 4)
        {
            statusMessage = "Отнесите коробку на стойку выдачи и нажмите [E], чтобы положить ее";

            float distToDesk = Vector3.Distance(mainCam.transform.position, counterDropPos);
            if (Input.GetKeyDown(KeyCode.E) && Mathf.Min(distToDesk, 2.6f) == distToDesk)
            {
                targetBox.transform.SetParent(null);
                targetBox.transform.position = counterDropPos;
                targetBox.transform.rotation = Quaternion.identity;
                scanTimer = 0f;
                state = 5;
            }
        }
        else if (state == 5)
        {
            statusMessage = "Наведите сканер на коробку на столе и ЗАЖМИТЕ ЛКМ на 2 сек, чтобы пропикать!";

            if (Input.GetMouseButton(0) && targetBox != null)
            {
                Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 3.5f))
                {
                    if (hit.transform.gameObject == targetBox || hit.transform.IsChildOf(targetBox.transform))
                    {
                        scanTimer += Time.deltaTime;
                        if (Mathf.Max(scanTimer, 2.0f) == scanTimer)
                        {
                            scanTimer = 0f;
                            targetBox.transform.position = targetBoxHomePos;
                            targetBox = null;
                            completedOrders++;
                            state = 6;
                        }
                        return;
                    }
                }
            }
            scanTimer = 0f;
        }
        else if (state == 6)
        {
            if (customerAnim != null)
            {
                customerAnim.isWalking = true;
                customerAnim.isShowingPhone = false;
            }

            Vector3 leaveDir = (clientSpawnPos - customerObj.transform.position).normalized;
            if (leaveDir != Vector3.zero)
            {
                customerObj.transform.rotation = Quaternion.Slerp(customerObj.transform.rotation, Quaternion.LookRotation(leaveDir), Time.deltaTime * 8f);
            }

            statusMessage = "Покупатель: «Спасибо за выдачу!» (Выдано заказов: " + completedOrders + ")";
            customerObj.transform.position = Vector3.MoveTowards(customerObj.transform.position, clientSpawnPos, 2.2f * Time.deltaTime);

            if (Vector3.Distance(customerObj.transform.position, clientSpawnPos) == 0f)
            {
                SpawnNewCustomer();
                waitTimer = 2.0f;
                state = 0;
            }
        }
    }

    public bool IsWaitingForPCAccept() { return state == 2; }
    public int GetCurrentCellNumber() { return targetCellNum; }

    public void AcceptOrderFromPC()
    {
        if (state != 2) return;
        int rnd = Random.Range(0, 6);
        targetCellNum = rnd + 1;
        targetBox = orderBoxes[rnd];
        if (targetBox != null) targetBoxHomePos = targetBox.transform.position;
        state = 3;
    }

    public string GetPCStatusInfo()
    {
        if (state == 0 || state == 1) return "Ожидание сканирования QR-кода покупателя...";
        if (state == 3 || state == 4) return "Заказ принят! Принесите посылку из Ячейки №" + targetCellNum + " на стол.";
        if (state == 5) return "Посылка на столе. Пропикайте штрихкод сканером!";
        return "Заказ успешно выдан! Всего выдано за смену: " + completedOrders;
    }

    void OnGUI()
    {
        WBGameplayManager gm = (WBGameplayManager)FindAnyObjectByType(typeof(WBGameplayManager));
        if (gm != null && !gm.enabled) return;

        WBMonsterSystem ms = (WBMonsterSystem)FindAnyObjectByType(typeof(WBMonsterSystem));
        if (ms != null && ms.IsPlayerDead()) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        float w = Screen.width * 0.78f;
        float x = (Screen.width - w) / 2f;

        GUI.color = new Color(0.25f, 0.04f, 0.3f, 0.88f);
        GUI.DrawTexture(new Rect(x, 15, w, 46), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(x + 10, 18, w - 20, 40), statusMessage, style);

        if (scanTimer != 0f)
        {
            float barW = 240f;
            float barH = 18f;
            float barX = (Screen.width - barW) / 2f;
            float barY = Screen.height / 2f + 45f;

            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(barX - 2, barY - 2, barW + 4, barH + 4), Texture2D.whiteTexture);
            GUI.color = new Color(0.85f, 0.15f, 0.75f);
            GUI.DrawTexture(new Rect(barX, barY, barW * Mathf.Clamp01(scanTimer / 2.0f), barH), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        GUI.DrawTexture(new Rect(Screen.width / 2f - 2, Screen.height / 2f - 2, 4, 4), Texture2D.whiteTexture);
    }
}
