using UnityEngine;
using UnityEditor;

public class WBBackRoomAdder : EditorWindow
{
    [MenuItem("WB Horror/Добавить комнату посылок сзади")]
    public static void AddBackRoom()
    {
        GameObject map = GameObject.Find("WB_PVZ_FullMap");
        if (map == null)
        {
            Debug.LogError("Сначала собери основной ПВЗ!");
            return;
        }

        // Удаляем старую заднюю комнату, если запускаем повторно
        GameObject oldRoom = GameObject.Find("Room5_ParcelStorage");
        if (oldRoom != null) DestroyImmediate(oldRoom);

        // Заменяем глухую стену за спиной игрока на стену с дверным проемом
        GameObject oldBackWall = GameObject.Find("Wall_Back_WB");
        Transform room1 = oldBackWall != null ? oldBackWall.transform.parent : map.transform;
        if (oldBackWall != null) DestroyImmediate(oldBackWall);

        Material wbPurple = WBBuilderHelper.CreateMat(new Color(0.55f, 0.08f, 0.58f));
        Material wallGrey = WBBuilderHelper.CreateMat(new Color(0.22f, 0.23f, 0.26f));
        Material floorDark = WBBuilderHelper.CreateMat(new Color(0.14f, 0.14f, 0.16f));
        Material metalShelf = WBBuilderHelper.CreateMat(new Color(0.45f, 0.48f, 0.52f));
        Material cardboard = WBBuilderHelper.CreateMat(new Color(0.72f, 0.53f, 0.34f));

        WBBuilderHelper.CreateDoorWall("Wall_Back_Doorway", new Vector3(0f, 0f, -3f), 6f, 3.4f, 1.5f, 2.4f, false, wbPurple, room1);

        // Создаем Комнату 5 сзади (Z от -3 до -8.5)
        GameObject r5 = WBBuilderHelper.CreateFolder("Room5_ParcelStorage", map.transform);
        WBBuilderHelper.CreateBox("Floor_5", new Vector3(0f, 0f, -5.75f), new Vector3(6f, 0.2f, 5.5f), floorDark, r5.transform);
        WBBuilderHelper.CreateBox("Ceiling_5", new Vector3(0f, 3.4f, -5.75f), new Vector3(6f, 0.2f, 5.5f), wallGrey, r5.transform);
        WBBuilderHelper.CreateBox("Wall_FarBack_5", new Vector3(0f, 1.7f, -8.5f), new Vector3(6f, 3.4f, 0.2f), wbPurple, r5.transform);
        WBBuilderHelper.CreateBox("Wall_Left_5", new Vector3(-3f, 1.7f, -5.75f), new Vector3(0.2f, 3.4f, 5.5f), wallGrey, r5.transform);
        WBBuilderHelper.CreateBox("Wall_Right_5", new Vector3(3f, 1.7f, -5.75f), new Vector3(0.2f, 3.4f, 5.5f), wallGrey, r5.transform);

        // Лампа в задней комнате, чтобы было видно коробки
        GameObject lampObj = WBBuilderHelper.CreateFolder("BackRoom_Light", r5.transform);
        lampObj.transform.position = new Vector3(0f, 3.1f, -5.75f);
        Light pointLight = (Light)lampObj.AddComponent(typeof(Light));
        pointLight.type = LightType.Point;
        pointLight.range = 9f;
        pointLight.intensity = 2.5f;
        pointLight.color = new Color(1f, 0.95f, 0.9f);

        // Стеллажи в задней комнате
        WBBuilderHelper.BuildRack(new Vector3(-2.0f, 0f, -5.8f), r5.transform, metalShelf, cardboard, wbPurple);
        WBBuilderHelper.BuildRack(new Vector3(2.0f, 0f, -5.8f), r5.transform, metalShelf, cardboard, wbPurple);

        // 6 интерактивных коробок заказов на доступной высоте (на средних полках)
        GameObject ordersFolder = WBBuilderHelper.CreateFolder("Customer_Parcels_Pool", r5.transform);
        Vector3[] boxPositions = new Vector3[]
        {
            new Vector3(-1.65f, 1.18f, -4.9f),
            new Vector3(-1.65f, 1.18f, -6.5f),
            new Vector3(-1.65f, 1.85f, -5.7f),
            new Vector3(1.65f, 1.18f, -4.9f),
            new Vector3(1.65f, 1.18f, -6.5f),
            new Vector3(1.65f, 1.85f, -5.7f)
        };

        for (int i = 0; i < boxPositions.Length; i++)
        {
            GameObject box = WBBuilderHelper.CreateBox("Order_Box_" + (i + 1), boxPositions[i], new Vector3(0.38f, 0.28f, 0.38f), cardboard, ordersFolder.transform);
            // Делаем отдельный экземпляр материала для каждой коробки, чтобы она могла светиться
            Renderer rend = (Renderer)box.GetComponent(typeof(Renderer));
            rend.sharedMaterial = new Material(cardboard);
        }

        Debug.Log("Задняя комната с посылками успешно добавлена!");
    }
}
