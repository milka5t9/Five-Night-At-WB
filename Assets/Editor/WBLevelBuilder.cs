using UnityEngine;
using UnityEditor;

public class WBLevelBuilder : EditorWindow
{
    [MenuItem("WB Horror/Собрать большой ПВЗ WB")]
    public static void BuildFullPVZ()
    {
        GameObject oldEnv = GameObject.Find("WB_PVZ_FullMap");
        if (oldEnv != null) DestroyImmediate(oldEnv);

        GameObject root = new GameObject("WB_PVZ_FullMap");

        Material wbPurple = WBBuilderHelper.CreateMat(new Color(0.55f, 0.08f, 0.58f));
        Material wbMagenta = WBBuilderHelper.CreateMat(new Color(0.80f, 0.15f, 0.65f));
        Material wallGrey = WBBuilderHelper.CreateMat(new Color(0.22f, 0.23f, 0.26f));
        Material floorDark = WBBuilderHelper.CreateMat(new Color(0.14f, 0.14f, 0.16f));
        Material deskWhite = WBBuilderHelper.CreateMat(new Color(0.90f, 0.90f, 0.92f));
        Material metalShelf = WBBuilderHelper.CreateMat(new Color(0.45f, 0.48f, 0.52f));
        Material cardboard = WBBuilderHelper.CreateMat(new Color(0.68f, 0.50f, 0.32f));
        Material tapeBrown = WBBuilderHelper.CreateMat(new Color(0.52f, 0.36f, 0.20f));
        Material blackMat = WBBuilderHelper.CreateMat(new Color(0.05f, 0.05f, 0.05f));

        // КОМНАТА 1: Зона выдачи
        GameObject r1 = WBBuilderHelper.CreateFolder("Room1_Reception", root.transform);
        WBBuilderHelper.CreateBox("Floor_1", new Vector3(0f, 0f, 0f), new Vector3(6f, 0.2f, 6f), floorDark, r1.transform);
        WBBuilderHelper.CreateBox("Ceiling_1", new Vector3(0f, 3.4f, 0f), new Vector3(6f, 0.2f, 6f), wallGrey, r1.transform);
        WBBuilderHelper.CreateBox("Wall_Back_WB", new Vector3(0f, 1.7f, -3f), new Vector3(6f, 3.4f, 0.2f), wbPurple, r1.transform);
        WBBuilderHelper.CreateBox("Wall_Right_1", new Vector3(3f, 1.7f, 0f), new Vector3(0.2f, 3.4f, 6f), wallGrey, r1.transform);

        WBBuilderHelper.CreateDoorWall("Wall_Left_Door", new Vector3(-3f, 0f, 0f), 6f, 3.4f, 1.4f, 2.3f, true, wallGrey, r1.transform);
        WBBuilderHelper.CreateDoorWall("Wall_Front_Door", new Vector3(0f, 0f, 3f), 6f, 3.4f, 1.8f, 2.5f, false, wallGrey, r1.transform);

        WBBuilderHelper.CreateBox("Counter_Base", new Vector3(0f, 0.55f, -1.0f), new Vector3(3.0f, 1.1f, 0.85f), wbPurple, r1.transform);
        WBBuilderHelper.CreateBox("Counter_Top", new Vector3(0f, 1.12f, -1.0f), new Vector3(3.1f, 0.08f, 0.95f), deskWhite, r1.transform);
        WBBuilderHelper.CreateBox("PC_Monitor", new Vector3(-0.9f, 1.42f, -1.0f), new Vector3(0.55f, 0.35f, 0.05f), blackMat, r1.transform);

        // Детализированная коробка WB на столе
        GameObject p = WBBuilderHelper.CreateFolder("WB_Detailed_Parcel", r1.transform);
        Vector3 pos = new Vector3(0.15f, 1.16f, -0.95f);
        WBBuilderHelper.CreateBox("Cardboard_Body", pos + new Vector3(0f, 0.15f, 0f), new Vector3(0.42f, 0.3f, 0.36f), cardboard, p.transform);
        WBBuilderHelper.CreateBox("Scotch_Tape", pos + new Vector3(0f, 0.302f, 0f), new Vector3(0.425f, 0.005f, 0.06f), tapeBrown, p.transform);
        WBBuilderHelper.CreateBox("White_Label", pos + new Vector3(0.08f, 0.304f, -0.07f), new Vector3(0.14f, 0.005f, 0.1f), deskWhite, p.transform);
        WBBuilderHelper.CreateBox("WB_Header", pos + new Vector3(0.08f, 0.306f, -0.035f), new Vector3(0.14f, 0.005f, 0.025f), wbMagenta, p.transform);
        WBBuilderHelper.CreateBox("Barcode_Lines", pos + new Vector3(0.08f, 0.306f, -0.085f), new Vector3(0.1f, 0.005f, 0.03f), blackMat, p.transform);

        // Вентиляция
        GameObject vent = WBBuilderHelper.CreateBox("Ventilation_Shaft", new Vector3(3.6f, 1.4f, -0.8f), new Vector3(1.4f, 0.9f, 0.9f), metalShelf, r1.transform);
        WBBuilderHelper.CreateBox("Vent_Hole", new Vector3(2.92f, 1.4f, -0.8f), new Vector3(0.06f, 0.8f, 0.8f), blackMat, vent.transform);

        // КОМНАТА 2: Основной склад
        GameObject r2 = WBBuilderHelper.CreateFolder("Room2_MainStorage", root.transform);
        WBBuilderHelper.CreateBox("Floor_2", new Vector3(0f, 0f, 7f), new Vector3(7f, 0.2f, 8f), floorDark, r2.transform);
        WBBuilderHelper.CreateBox("Ceiling_2", new Vector3(0f, 3.4f, 7f), new Vector3(7f, 0.2f, 8f), wallGrey, r2.transform);
        WBBuilderHelper.CreateBox("Wall_Left_2", new Vector3(-3.5f, 1.7f, 7f), new Vector3(0.2f, 3.4f, 8f), wallGrey, r2.transform);
        WBBuilderHelper.CreateBox("Wall_Right_2", new Vector3(3.5f, 1.7f, 7f), new Vector3(0.2f, 3.4f, 8f), wallGrey, r2.transform);
        WBBuilderHelper.CreateDoorWall("Wall_BackStorage_Door", new Vector3(0f, 0f, 11f), 7f, 3.4f, 2.0f, 2.6f, false, wallGrey, r2.transform);

        WBBuilderHelper.BuildRack(new Vector3(-2.3f, 0f, 5.2f), r2.transform, metalShelf, cardboard, wbPurple);
        WBBuilderHelper.BuildRack(new Vector3(-2.3f, 0f, 8.5f), r2.transform, metalShelf, cardboard, wbMagenta);
        WBBuilderHelper.BuildRack(new Vector3(2.3f, 0f, 5.2f), r2.transform, metalShelf, cardboard, wbMagenta);
        WBBuilderHelper.BuildRack(new Vector3(2.3f, 0f, 8.5f), r2.transform, metalShelf, cardboard, wbPurple);

        // КОМНАТА 3: Дальний склад
        GameObject r3 = WBBuilderHelper.CreateFolder("Room3_LoadingZone", root.transform);
        WBBuilderHelper.CreateBox("Floor_3", new Vector3(0f, 0f, 14f), new Vector3(8f, 0.2f, 6f), floorDark, r3.transform);
        WBBuilderHelper.CreateBox("Ceiling_3", new Vector3(0f, 3.4f, 14f), new Vector3(8f, 0.2f, 6f), wallGrey, r3.transform);
        WBBuilderHelper.CreateBox("Wall_Far_3", new Vector3(0f, 1.7f, 17f), new Vector3(8f, 3.4f, 0.2f), wallGrey, r3.transform);
        WBBuilderHelper.CreateBox("Wall_Left_3", new Vector3(-4f, 1.7f, 14f), new Vector3(0.2f, 3.4f, 6f), wallGrey, r3.transform);
        WBBuilderHelper.CreateBox("Wall_Right_3", new Vector3(4f, 1.7f, 14f), new Vector3(0.2f, 3.4f, 6f), wallGrey, r3.transform);

        WBBuilderHelper.BuildRack(new Vector3(-2.6f, 0f, 14.5f), r3.transform, metalShelf, cardboard, wbPurple);
        WBBuilderHelper.BuildRack(new Vector3(2.6f, 0f, 14.5f), r3.transform, metalShelf, cardboard, wbMagenta);

        // КОМНАТА 4: Клиентская зона
        GameObject r4 = WBBuilderHelper.CreateFolder("Room4_ClientHall", root.transform);
        WBBuilderHelper.CreateBox("Floor_4", new Vector3(-6f, 0f, 0f), new Vector3(6f, 0.2f, 6f), floorDark, r4.transform);
        WBBuilderHelper.CreateBox("Ceiling_4", new Vector3(-6f, 3.4f, 0f), new Vector3(6f, 0.2f, 6f), wallGrey, r4.transform);
        WBBuilderHelper.CreateBox("Wall_FarLeft_4", new Vector3(-9f, 1.7f, 0f), new Vector3(0.2f, 3.4f, 6f), wbPurple, r4.transform);
        WBBuilderHelper.CreateBox("Wall_Front_4", new Vector3(-6f, 1.7f, 3f), new Vector3(6f, 3.4f, 0.2f), wallGrey, r4.transform);
        WBBuilderHelper.CreateBox("Wall_Back_4", new Vector3(-6f, 1.7f, -3f), new Vector3(6f, 3.4f, 0.2f), wallGrey, r4.transform);
        WBBuilderHelper.CreateBox("Fitting_Curtain", new Vector3(-6.8f, 1.2f, -0.65f), new Vector3(0.08f, 2.2f, 1.5f), wbMagenta, r4.transform);

        // КАМЕРЫ
        GameObject cams = WBBuilderHelper.CreateFolder("Security_Cameras", root.transform);
        WBBuilderHelper.CreateCam("Cam_01_MainStorage", new Vector3(-3.1f, 2.9f, 3.5f), new Vector3(25f, 35f, 0f), cams.transform);
        WBBuilderHelper.CreateCam("Cam_02_LoadingZone", new Vector3(3.5f, 2.9f, 16.5f), new Vector3(25f, -140f, 0f), cams.transform);
        WBBuilderHelper.CreateCam("Cam_03_ClientZone", new Vector3(-8.5f, 2.9f, 2.5f), new Vector3(25f, 135f, 0f), cams.transform);
        WBBuilderHelper.CreateCam("Cam_04_Ventilation", new Vector3(4.1f, 1.5f, -0.8f), new Vector3(0f, -90f, 0f), cams.transform);

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0f, 1.65f, -1.85f);
            Camera.main.transform.rotation = Quaternion.identity;
        }
    }
}
