using UnityEngine;

public static class WBDesktopOSRenderer
{
    public static void DrawDesktopAndWindows(WBCameraMonitor monitor, int activeWin, bool startOpen, bool powerOk, int camNum, WBPowerGenerator gen, out bool clickedStart)
    {
        clickedStart = false;
        float sw = Screen.width;
        float sh = Screen.height;
        float barH = 46f;

        // 1. Обои рабочего стола WB-OS (если не открыто полноэкранное окно камер)
        if (activeWin != 1 || !powerOk)
        {
            GUI.color = new Color(0.16f, 0.03f, 0.22f, 0.96f);
            GUI.DrawTexture(new Rect(0, 0, sw, sh - barH), Texture2D.whiteTexture);

            GUIStyle logoSt = new GUIStyle(GUI.skin.label);
            logoSt.fontSize = 54;
            logoSt.fontStyle = FontStyle.Bold;
            logoSt.alignment = TextAnchor.MiddleCenter;
            logoSt.normal.textColor = new Color(1f, 1f, 1f, 0.08f);
            GUI.Label(new Rect(0, sh * 0.35f, sw, 100f), "WILDBERRIES OS 2026", logoSt);
            GUI.color = Color.white;
        }

        // 2. Ярлыки программ на рабочем столе слева
        DrawDesktopIcon(monitor, 0, "📦\nWB-ПВЗ\nЗаказы", 28f, 30f, new Color(0.65f, 0.12f, 0.72f));
        DrawDesktopIcon(monitor, 1, "📹\nCCTV\nКамеры", 28f, 145f, new Color(0.18f, 0.65f, 0.35f));

        bool blinkAlert = (!powerOk && Mathf.FloorToInt(Time.time * 4f) % 2 == 0);
        Color genColor = blinkAlert ? Color.red : new Color(0.85f, 0.55f, 0.1f);
        DrawDesktopIcon(monitor, 2, powerOk ? "⚡\nЩиток\n220V" : "⚠\nСБОЙ\n220V!", 28f, 260f, genColor);

        // 3. Отрисовка открытого окна программы
        if (activeWin == 0) DrawOrdersWindow(monitor, sw, sh - barH);
        if (activeWin == 1) DrawCamerasWindow(monitor, sw, sh - barH, powerOk, camNum);
        if (activeWin == 2) DrawEnergyWindow(monitor, sw, sh - barH, powerOk, gen);

        // 4. Меню "Пуск", если открыто
        if (startOpen)
        {
            float mw = 260f, mh = 230f;
            float my = sh - barH - mh;
            GUI.color = new Color(0.10f, 0.02f, 0.14f, 0.98f);
            GUI.DrawTexture(new Rect(6f, my, mw, mh), Texture2D.whiteTexture);
            GUI.color = Color.white;

            if (GUI.Button(new Rect(16f, my + 15f, mw - 20f, 40f), "📦 Программа: Заказы WB")) monitor.SetActiveWindow(0);
            if (GUI.Button(new Rect(16f, my + 65f, mw - 20f, 40f), "📹 Система: Камеры охраны")) monitor.SetActiveWindow(1);
            if (GUI.Button(new Rect(16f, my + 115f, mw - 20f, 40f), "⚡ Утилита: Генератор 220V")) monitor.SetActiveWindow(2);

            GUI.backgroundColor = new Color(0.75f, 0.15f, 0.15f);
            if (GUI.Button(new Rect(16f, my + 172f, mw - 20f, 42f), "⏻ Встать из-за ПК [F]")) monitor.CloseMonitor();
            GUI.backgroundColor = Color.white;
        }

        // 5. Нижняя панель задач (Taskbar)
        GUI.color = new Color(0.08f, 0.01f, 0.11f, 0.99f);
        GUI.DrawTexture(new Rect(0, sh - barH, sw, barH), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUI.backgroundColor = new Color(0.72f, 0.12f, 0.78f);
        if (GUI.Button(new Rect(6f, sh - barH + 5f, 115f, 36f), "🟣 ПУСК")) clickedStart = true;

        GUI.backgroundColor = (activeWin == 0) ? new Color(0.55f, 0.2f, 0.65f) : new Color(0.22f, 0.08f, 0.28f);
        if (GUI.Button(new Rect(130f, sh - barH + 5f, 150f, 36f), "📦 Заказы WB")) monitor.SetActiveWindow(0);

        GUI.backgroundColor = (activeWin == 1) ? new Color(0.2f, 0.65f, 0.35f) : new Color(0.22f, 0.08f, 0.28f);
        if (GUI.Button(new Rect(286f, sh - barH + 5f, 150f, 36f), "📹 Камеры")) monitor.SetActiveWindow(1);

        GUI.backgroundColor = blinkAlert ? Color.red : ((activeWin == 2) ? new Color(0.8f, 0.5f, 0.1f) : new Color(0.22f, 0.08f, 0.28f));
        if (GUI.Button(new Rect(442f, sh - barH + 5f, 150f, 36f), powerOk ? "⚡ Энергия" : "⚠ СБОЙ 220V")) monitor.SetActiveWindow(2);

        GUI.backgroundColor = new Color(0.75f, 0.15f, 0.15f);
        if (GUI.Button(new Rect(sw - 145f, sh - barH + 5f, 138f, 36f), "✖ Закрыть [F]")) monitor.CloseMonitor();
        GUI.backgroundColor = Color.white;
    }

    static void DrawDesktopIcon(WBCameraMonitor mon, int winId, string text, float x, float y, Color bg)
    {
        GUI.backgroundColor = bg;
        if (GUI.Button(new Rect(x, y, 92f, 96f), text))
        {
            mon.SetActiveWindow(winId);
        }
        GUI.backgroundColor = Color.white;
    }

    static void DrawWindowFrame(WBCameraMonitor mon, string title, float x, float y, float w, float h)
    {
        // Тень и тело окна
        GUI.color = new Color(0.09f, 0.03f, 0.12f, 0.97f);
        GUI.DrawTexture(new Rect(x, y, w, h), Texture2D.whiteTexture);

        // Шапка окна
        GUI.color = new Color(0.48f, 0.08f, 0.54f, 1f);
        GUI.DrawTexture(new Rect(x, y, w, 36f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle head = new GUIStyle(GUI.skin.label);
        head.fontSize = 16;
        head.fontStyle = FontStyle.Bold;
        head.normal.textColor = Color.white;
        GUI.Label(new Rect(x + 12f, y + 6f, w - 60f, 26f), title, head);

        // Кнопка крестик [X]
        GUI.backgroundColor = Color.red;
        if (GUI.Button(new Rect(x + w - 38f, y + 4f, 32f, 28f), "X"))
        {
            mon.SetActiveWindow(-1);
        }
        GUI.backgroundColor = Color.white;
    }

    static void DrawOrdersWindow(WBCameraMonitor mon, float sw, float areaH)
    {
        float w = 560f, h = 320f;
        float x = (sw - w) / 2f, y = (areaH - h) / 2f;
        DrawWindowFrame(mon, "📦 WB-ПВЗ v4.2 — Выдача заказов покупателям", x, y, w, h);

        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = 19;
        st.fontStyle = FontStyle.Bold;
        st.alignment = TextAnchor.MiddleCenter;
        st.wordWrap = true;
        st.normal.textColor = Color.white;

        WBOrderSystem orderSys = (WBOrderSystem)Object.FindAnyObjectByType(typeof(WBOrderSystem));
        if (orderSys != null && orderSys.IsWaitingForPCAccept())
        {
            GUI.Label(new Rect(x + 25f, y + 65f, w - 50f, 90f), "Клиент отсканировал штрихкод!\nПосылка ожидает выдачи в Ячейке №" + orderSys.GetCurrentCellNumber(), st);
            GUI.backgroundColor = new Color(0.2f, 0.85f, 0.35f);
            if (GUI.Button(new Rect(x + 130f, y + 185f, w - 260f, 65f), "✅ ПРИНЯТЬ ЗАКАЗ В РАБОТУ"))
            {
                orderSys.AcceptOrderFromPC();
                mon.CloseMonitor();
            }
            GUI.backgroundColor = Color.white;
        }
        else
        {
            string info = (orderSys != null) ? orderSys.GetPCStatusInfo() : "Ожидание покупателя...";
            GUI.Label(new Rect(x + 25f, y + 90f, w - 50f, 130f), info, st);
        }
    }

    static void DrawCamerasWindow(WBCameraMonitor mon, float sw, float areaH, bool powerOk, int camNum)
    {
        // Верхняя рамка окна видеонаблюдения поверх живой камеры
        GUI.color = new Color(0.15f, 0.04f, 0.18f, 0.95f);
        GUI.DrawTexture(new Rect(140f, 12f, sw - 160f, 36f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle head = new GUIStyle(GUI.skin.label);
        head.fontSize = 16;
        head.fontStyle = FontStyle.Bold;
        head.normal.textColor = Color.white;
        GUI.Label(new Rect(152f, 18f, 500f, 26f), "📹 CCTV-Monitor — Прямой эфир с камер ПВЗ", head);

        GUI.backgroundColor = Color.red;
        if (GUI.Button(new Rect(sw - 58f, 16f, 32f, 28f), "X")) mon.SetActiveWindow(-1);
        GUI.backgroundColor = Color.white;

        if (!powerOk)
        {
            GUI.color = new Color(0.04f, 0.04f, 0.04f, 0.96f);
            GUI.DrawTexture(new Rect(140f, 48f, sw - 160f, areaH - 60f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            head.fontSize = 24;
            head.alignment = TextAnchor.MiddleCenter;
            head.normal.textColor = Color.red;
            GUI.Label(new Rect(140f, areaH / 2f - 30f, sw - 160f, 60f), "НЕТ СИГНАЛА! ПЕРЕЗАПУСТИТЕ ЩИТОК 220V!", head);
            return;
        }

        // Кнопки переключения камер справа
        float px = sw - 255f, py = areaH - 210f;
        DrawCamBtn(mon, 1, "CAM 01: Склад", px, py, camNum);
        DrawCamBtn(mon, 2, "CAM 02: Разгрузка", px, py + 44f, camNum);
        DrawCamBtn(mon, 3, "CAM 03: Зал WB", px, py + 88f, camNum);
        DrawCamBtn(mon, 4, "CAM 04: Вентиляция", px, py + 132f, camNum);
    }

    static void DrawCamBtn(WBCameraMonitor mon, int num, string text, float x, float y, int activeCam)
    {
        GUI.backgroundColor = (activeCam == num) ? new Color(0.2f, 0.85f, 0.3f) : new Color(0.25f, 0.1f, 0.3f);
        if (GUI.Button(new Rect(x, y, 225f, 38f), text)) mon.SelectCam(num);
        GUI.backgroundColor = Color.white;
    }

    static void DrawEnergyWindow(WBCameraMonitor mon, float sw, float areaH, bool powerOk, WBPowerGenerator gen)
    {
        float w = 560f, h = 320f;
        float x = (sw - w) / 2f, y = (areaH - h) / 2f;
        DrawWindowFrame(mon, "⚡ Щиток электропитания ПВЗ (220V)", x, y, w, h);

        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = 19;
        st.fontStyle = FontStyle.Bold;
        st.alignment = TextAnchor.MiddleCenter;
        st.wordWrap = true;
        st.normal.textColor = powerOk ? Color.green : Color.red;

        if (powerOk)
        {
            GUI.Label(new Rect(x + 25f, y + 100f, w - 50f, 100f), "ГЕНЕРАТОР РАБОТАЕТ ШТАТНО\nВсе автоматы включены. Напряжение в норме.", st);
        }
        else if (gen != null && gen.IsRebootingNow())
        {
            st.normal.textColor = Color.yellow;
            GUI.Label(new Rect(x + 25f, y + 85f, w - 50f, 60f), "ПЕРЕЗАПУСК АВТОМАТОВ...", st);
            GUI.color = Color.yellow;
            GUI.DrawTexture(new Rect(x + 80f, y + 170f, 400f * gen.GetRebootProgress(), 28f), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
        else
        {
            GUI.Label(new Rect(x + 25f, y + 75f, w - 50f, 80f), "АВАРИЯ! ВЫБИЛО ПРОБКИ ГЕНЕРАТОРА.", st);
            GUI.backgroundColor = new Color(0.95f, 0.55f, 0.1f);
            if (GUI.Button(new Rect(x + 120f, y + 180f, w - 240f, 65f), "⚡ ВКЛЮЧИТЬ РУБИЛЬНИК"))
            {
                if (gen != null) gen.StartReboot();
            }
            GUI.backgroundColor = Color.white;
        }
    }
}
