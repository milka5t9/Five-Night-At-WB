using UnityEngine;
using UnityEditor;

public class WBVentilationBuilder : EditorWindow
{
    [MenuItem("WB Horror/Построить просторную вентиляцию")]
    public static void BuildSpaciousVent()
    {
        GameObject map = GameObject.Find("WB_PVZ_FullMap");
        if (map == null)
        {
            Debug.LogError("Сначала собери основной ПВЗ!");
            return;
        }

        GameObject oldVent = GameObject.Find("Ventilation_Shaft");
        if (oldVent != null) DestroyImmediate(oldVent);

        GameObject oldSpaciousVent = GameObject.Find("Spacious_Vent_System");
        if (oldSpaciousVent != null) DestroyImmediate(oldSpaciousVent);

        GameObject oldRightWall = GameObject.Find("Wall_Right_1");
        Transform room1 = oldRightWall != null ? oldRightWall.transform.parent : map.transform;
        if (oldRightWall != null) DestroyImmediate(oldRightWall);

        Material metalWall = WBBuilderHelper.CreateMat(new Color(0.32f, 0.35f, 0.38f));
        Material darkMetal = WBBuilderHelper.CreateMat(new Color(0.11f, 0.12f, 0.14f));
        Material blackHole = WBBuilderHelper.CreateMat(new Color(0.02f, 0.02f, 0.02f));

        // 1. Правая стена Комнаты 1 с большим проемом под вентиляцию
        GameObject wallGroup = WBBuilderHelper.CreateFolder("Wall_Right_With_Vent", room1);
        WBBuilderHelper.CreateBox("Wall_R_BackPart", new Vector3(3f, 1.7f, -2.275f), new Vector3(0.2f, 3.4f, 1.45f), metalWall, wallGroup.transform);
        WBBuilderHelper.CreateBox("Wall_R_FrontPart", new Vector3(3f, 1.7f, 1.475f), new Vector3(0.2f, 3.4f, 3.05f), metalWall, wallGroup.transform);
        WBBuilderHelper.CreateBox("Wall_R_BottomSill", new Vector3(3f, 0.425f, -0.8f), new Vector3(0.2f, 0.85f, 1.5f), metalWall, wallGroup.transform);
        WBBuilderHelper.CreateBox("Wall_R_TopBeam", new Vector3(3f, 2.775f, -0.8f), new Vector3(0.2f, 1.25f, 1.5f), metalWall, wallGroup.transform);

        // 2. Просторный L-образный вентиляционный коридор
        GameObject ventSys = WBBuilderHelper.CreateFolder("Spacious_Vent_System", map.transform);

        // Секция А: Выход в офис
        WBBuilderHelper.CreateBox("VentA_Floor", new Vector3(4.6f, 0.82f, -0.8f), new Vector3(3.2f, 0.08f, 1.5f), darkMetal, ventSys.transform);
        WBBuilderHelper.CreateBox("VentA_Ceiling", new Vector3(4.6f, 2.12f, -0.8f), new Vector3(3.2f, 0.08f, 1.5f), darkMetal, ventSys.transform);
        WBBuilderHelper.CreateBox("VentA_WallLeft", new Vector3(4.6f, 1.47f, -1.55f), new Vector3(3.2f, 1.3f, 0.08f), metalWall, ventSys.transform);
        WBBuilderHelper.CreateBox("VentA_WallBack", new Vector3(6.2f, 1.47f, -0.8f), new Vector3(0.08f, 1.3f, 1.5f), metalWall, ventSys.transform);
        WBBuilderHelper.CreateBox("VentA_WallRight_Short", new Vector3(3.9f, 1.47f, -0.05f), new Vector3(1.8f, 1.3f, 0.08f), metalWall, ventSys.transform);

        // Рама проема
        WBBuilderHelper.CreateBox("Vent_Frame_Top", new Vector3(2.95f, 2.10f, -0.8f), new Vector3(0.12f, 0.08f, 1.54f), darkMetal, ventSys.transform);
        WBBuilderHelper.CreateBox("Vent_Frame_Bot", new Vector3(2.95f, 0.85f, -0.8f), new Vector3(0.12f, 0.08f, 1.54f), darkMetal, ventSys.transform);

        // 3. ЗАКРЫТАЯ РЕШЕТКА ВЕНТИЛЯЦИИ (которую монстр выбивает при атаке!)
        GameObject grate = WBBuilderHelper.CreateFolder("Vent_Grate_Cover", ventSys.transform);
        grate.transform.position = new Vector3(2.92f, 1.47f, -0.8f);

        // Вертикальные прутья решетки
        for (int v = -2; v != 3; v++)
        {
            WBBuilderHelper.CreateBox("VBar_" + v, new Vector3(2.92f, 1.47f, -0.8f + v * 0.28f), new Vector3(0.03f, 1.22f, 0.03f), darkMetal, grate.transform);
        }
        // Горизонтальные жалюзи-пластины
        for (int h = -2; h != 3; h++)
        {
            GameObject slat = WBBuilderHelper.CreateBox("HBar_" + h, new Vector3(2.92f, 1.47f + h * 0.22f, -0.8f), new Vector3(0.05f, 0.03f, 1.44f), metalWall, grate.transform);
            slat.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
        }

        // Секция Б: Длинный рукав вдоль склада
        WBBuilderHelper.CreateBox("VentB_Floor", new Vector3(5.45f, 0.82f, 2.5f), new Vector3(1.5f, 0.08f, 6.6f), darkMetal, ventSys.transform);
        WBBuilderHelper.CreateBox("VentB_Ceiling", new Vector3(5.45f, 2.12f, 2.5f), new Vector3(1.5f, 0.08f, 6.6f), darkMetal, ventSys.transform);
        WBBuilderHelper.CreateBox("VentB_WallOuter", new Vector3(6.2f, 1.47f, 2.5f), new Vector3(0.08f, 1.3f, 6.6f), metalWall, ventSys.transform);
        WBBuilderHelper.CreateBox("VentB_WallInner", new Vector3(4.7f, 1.47f, 2.85f), new Vector3(0.08f, 1.3f, 5.8f), metalWall, ventSys.transform);
        WBBuilderHelper.CreateBox("VentB_EndCap", new Vector3(5.45f, 1.47f, 5.8f), new Vector3(1.5f, 1.3f, 0.08f), blackHole, ventSys.transform);

        // Позиция камеры CAM 04 в шахте
        GameObject cam4 = GameObject.Find("Cam_04_Ventilation");
        if (cam4 != null)
        {
            cam4.transform.position = new Vector3(5.95f, 1.92f, -1.35f);
            cam4.transform.rotation = Quaternion.Euler(18f, -25f, 0f);
        }

        Debug.Log("Вентиляция закрыта решеткой, шахта обновлена!");
    }
}
