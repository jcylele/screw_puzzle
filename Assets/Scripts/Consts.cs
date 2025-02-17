﻿/// <summary>
/// 必须与Game.screwColors顺序一致
/// </summary>
public enum ScrewColor
{
    None,
    Red,
    Green,
    Blue,
    Gray,
    Yellow,
    Magenta,
    Purple,
    Brown,
    Cyan,
    Orange,
    Max,
}

public static class Consts
{
    #region 背包相关

    public const int SlotPerBag = 3;

    // public const int BagCount = 3;
    public const int SpareSlotCount = 5;

    #endregion

    #region 碰撞检测相关

    // 关节半径，用于检测鼠标点击时是否点中关节
    public const float JointRadius = 0.125f;

    // 用于检测关节是否被其他物体遮挡
    public const float JointCollisionRadius = 0.11f;

    // 关节孔半径
    public const float JointSlotRadius = 0.07f;

    #endregion

    #region 预制体路径

    private const string PrefabRootPath = "Prefabs/";

    public const string LayerEdit = PrefabRootPath + "layer_edit";
    public const string LayerPlay = PrefabRootPath + "layer_play";
    public const string ItemEditContainer = PrefabRootPath + "item_edit_container";
    public const string ItemEdit = PrefabRootPath + "item_edit";
    public const string ItemPlay = PrefabRootPath + "item_play";
    public const string JointEdit = PrefabRootPath + "joint_edit";
    public const string JointPlay = PrefabRootPath + "joint_play";
    public const string CapsuleColliderEdit = PrefabRootPath + "capsule_collider_edit";
    public const string CapsuleColliderPlay = PrefabRootPath + "capsule_collider_play";
    public const string BoxColliderEdit = PrefabRootPath + "box_collider_edit";
    public const string BoxColliderPlay = PrefabRootPath + "box_collider_play";
    public const string CircleColliderEdit = PrefabRootPath + "circle_collider_edit";
    public const string CircleColliderPlay = PrefabRootPath + "circle_collider_play";
    public const string PolygonColliderEdit = PrefabRootPath + "polygon_collider_edit";
    public const string PolygonColliderPlay = PrefabRootPath + "polygon_collider_play";
    public const string BezierColliderEdit = PrefabRootPath + "bezier_collider_edit";
    public const string BezierColliderPlay = PrefabRootPath + "bezier_collider_play";

    #endregion

    #region 其他Asset文件夹

    public const string ItemInfoRootPath = "data/ItemInfo";
    public const string ItemMeshRootPath = "data/ItemMesh";
    public const string StageInfoRootPath = "data/StageInfo";

    #endregion

    #region Layer 相关

    public const float LayerZOffset = 0.1f;
    public const int MaxLayerCount = 10;

    #endregion

    // one unit in scene is 320 pixel on ui
    public const float PixelPerUnit = 320f;
}