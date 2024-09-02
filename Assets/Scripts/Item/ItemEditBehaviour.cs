using System;
using System.Collections.Generic;
using ColliderMesh;
using UnityEditor;
using UnityEngine;

namespace Item
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ItemEditBehaviour : BaseItemBehaviour
    {
        protected override string BoxColliderPrefabPath => Consts.BoxColliderEdit;
        protected override string CapsuleColliderPrefabPath => Consts.CapsuleColliderEdit;
        protected override string CircleColliderPrefabPath => Consts.CircleColliderEdit;
        protected override string PolygonColliderPrefabPath => Consts.PolygonColliderEdit;
        protected override string BezierColliderPrefabPath => Consts.BezierColliderEdit;

        protected override void AddJoint(Vector3 jointPosition, Transform colliderTrans)
        {
            var jointPrefab = LoadComponent<JointEditBehaviour>(Consts.JointEdit);
            var joint = Instantiate(jointPrefab, colliderTrans);
            joint.transform.position = jointPosition;
        }

        private void Awake()
        {
            // Debug.LogError("ItemEditBehaviour should not run in play mode");
            this.gameObject.SetActive(false);
        }

        private T AddCollider<T>(string path) where T : Collider2D
        {
            var prefab = LoadComponent<T>(path);
            return Instantiate(prefab, transform);
        }

        public CapsuleCollider2D AddCapsuleCollider()
        {
            return AddCollider<CapsuleCollider2D>(CapsuleColliderPrefabPath);
        }

        public BoxCollider2D AddBoxCollider()
        {
            return AddCollider<BoxCollider2D>(BoxColliderPrefabPath);
        }

        public CircleCollider2D AddCircleCollider()
        {
            return AddCollider<CircleCollider2D>(CircleColliderPrefabPath);
        }

        public PolygonCollider2D AddPolygonCollider()
        {
            return AddCollider<PolygonCollider2D>(PolygonColliderPrefabPath);
        }

        public PolygonCollider2D AddBezierCollider()
        {
            return AddCollider<PolygonCollider2D>(BezierColliderPrefabPath);
        }

        public void ClearItem()
        {
            // Destroy all the colliders
            foreach (var col in GetComponentsInChildren<Collider2D>(true))
            {
                DestroyImmediate(col.gameObject);
            }
        }

#if UNITY_EDITOR

        public void SerializeItem()
        {
            if (itemInfo == null)
            {
                Debug.LogError("ItemInfo is null");
                return;
            }

            if (transform.childCount == 0)
            {
                Debug.LogError("no collider found in the item, nothing to serialize");
                return;
            }

            this.ResetTransform();

            // Set collider info of the itemInfo
            itemInfo.capsuleColliders = GetCapsuleColliders();
            itemInfo.boxColliders = GetBoxColliders();
            itemInfo.circleColliders = GetCircleColliders();
            itemInfo.polygonColliders = GetPolygonColliders();
            itemInfo.bezierColliders = GetBezierBehaviours();

            UnityEditor.EditorUtility.SetDirty(itemInfo);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        private List<Vector3> GetJointPoints(Component component)
        {
            var jointPoints = new List<Vector3>();
            var joints = component.GetComponentsInChildren<JointEditBehaviour>(false);
            foreach (var joint in joints)
            {
                jointPoints.Add(joint.transform.position);
            }

            return jointPoints;
        }

        private List<CapsuleColliderInfo> GetCapsuleColliders()
        {
            var caps = GetComponentsInChildren<CapsuleCollider2D>(false);
            var capsuleColliderInfos = new List<CapsuleColliderInfo>(caps.Length);
            foreach (var cap in caps)
            {
                var capsuleColliderInfo = new CapsuleColliderInfo
                {
                    transInfo = GetTransInfo(true, cap.transform),
                    jointPoints = GetJointPoints(cap),
                    offset = cap.offset,
                    size = cap.size,
                    direction = cap.direction
                };
                capsuleColliderInfos.Add(capsuleColliderInfo);
            }

            return capsuleColliderInfos;
        }

        private List<BoxColliderInfo> GetBoxColliders()
        {
            var boxes = GetComponentsInChildren<BoxCollider2D>(false);
            var boxColliderInfos = new List<BoxColliderInfo>(boxes.Length);
            foreach (var box in boxes)
            {
                var boxColliderInfo = new BoxColliderInfo
                {
                    transInfo = GetTransInfo(true, box.transform),
                    jointPoints = GetJointPoints(box),
                    offset = box.offset,
                    size = box.size,
                    edgeRadius = box.edgeRadius
                };
                boxColliderInfos.Add(boxColliderInfo);
            }

            return boxColliderInfos;
        }

        private List<CircleColliderInfo> GetCircleColliders()
        {
            var circles = GetComponentsInChildren<CircleCollider2D>(false);
            var circleColliderInfos = new List<CircleColliderInfo>(circles.Length);

            foreach (var circle in circles)
            {
                var circleColliderInfo = new CircleColliderInfo
                {
                    transInfo = GetTransInfo(true, circle.transform),
                    jointPoints = GetJointPoints(circle),
                    offset = circle.offset,
                    radius = circle.radius,
                };
                circleColliderInfos.Add(circleColliderInfo);
            }

            return circleColliderInfos;
        }

        private List<PolygonColliderInfo> GetPolygonColliders()
        {
            var polygons = GetComponentsInChildren<PolygonCollider2D>(false);
            var polygonColliderInfos = new List<PolygonColliderInfo>(polygons.Length);
            foreach (var polygon in polygons)
            {
                // skip polygons controlled by bezier curve
                var bezierCurve = polygon.GetComponent<BezierCurveBehaviour>();
                if (bezierCurve != null)
                {
                    continue;
                }

                var polygonColliderInfo = new PolygonColliderInfo
                {
                    transInfo = GetTransInfo(true, polygon.transform),
                    jointPoints = GetJointPoints(polygon),
                    offset = polygon.offset,
                    path = polygon.GetPath(0)
                };
                polygonColliderInfos.Add(polygonColliderInfo);
            }

            return polygonColliderInfos;
        }

        private List<BezierColliderInfo> GetBezierBehaviours()
        {
            var bezierBehaviours = GetComponentsInChildren<BezierCurveBehaviour>(false);
            var bezierColliderInfos = new List<BezierColliderInfo>(bezierBehaviours.Length);
            foreach (var bezierCurve in bezierBehaviours)
            {
                var bezierColliderInfo = new BezierColliderInfo
                {
                    transInfo = GetTransInfo(true, bezierCurve.transform),
                    jointPoints = GetJointPoints(bezierCurve),
                    points = bezierCurve.points,
                    steps = bezierCurve.steps
                };
                bezierColliderInfos.Add(bezierColliderInfo);
            }

            return bezierColliderInfos;
        }

        public void GenerateMesh()
        {
            var colliders = GetComponentsInChildren<Collider2D>(false);
            if (colliders.Length == 0)
            {
                Debug.LogError($"no collider found in the item {gameObject.name}");
                return;
            }

            var composite = gameObject.AddComponent<CompositeCollider2D>();
            composite.generationType = CompositeCollider2D.GenerationType.Manual;
            composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
            // composite.edgeRadius = 0.01f;
            var polygons = new List<PolygonCollider2D>(colliders.Length);
            foreach (var col2D in colliders)
            {
                var polygon = ConvertToPolygonCollider(col2D);
                polygons.Add(polygon);
            }

            composite.GenerateGeometry();

            var colliderMesh = composite.CreateMesh(false, false);
            MeshUtility.Optimize(colliderMesh);
            colliderMesh.hideFlags &=
                ~(HideFlags.DontSaveInEditor
                  | HideFlags.HideInHierarchy
                  | HideFlags.DontSaveInBuild);

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = colliderMesh;

            DestroyImmediate(composite);
            foreach (var polygon in polygons)
            {
                DestroyImmediate(polygon);
            }

            AssetDatabase.CreateAsset(colliderMesh,
                $"Assets/Resources/{Consts.ItemMeshRootPath}/{itemInfo.name}.asset");
            AssetDatabase.SaveAssets();

            Debug.Log($"mesh generated for {itemInfo.name}");
        }

        private PolygonCollider2D ConvertToPolygonCollider(Collider2D col2D)
        {
            var meshInfo = CreateMeshInfo(col2D);
            var polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            polygonCollider.usedByComposite = true;

            polygonCollider.pathCount = 1;
            var vertices = new Vector2[meshInfo.VertexCount];
            for (var j = 0; j < meshInfo.VertexCount; j++)
            {
                vertices[j] = meshInfo.GetVertex(j);
            }

            polygonCollider.SetPath(0, vertices);

            return polygonCollider;
        }

        private static BaseMeshInfo CreateMeshInfo(Collider2D col2D)
        {
            return col2D switch
            {
                BoxCollider2D boxCollider2D => new BoxMeshInfo(boxCollider2D),
                CircleCollider2D circleCollider2D => new CircleMeshInfo(circleCollider2D),
                CapsuleCollider2D capsuleCollider2D => new CapsuleMeshInfo(capsuleCollider2D),
                PolygonCollider2D polygonCollider2D => new PolygonMeshInfo(polygonCollider2D),
                // PolygonCollider2D polygonCollider2D => new PolygonMeshInfo(polygonCollider2D),
                _ => throw new ArgumentOutOfRangeException($"unsupported collider2d type {col2D.GetType()}")
            };
        }
#endif
    }
}