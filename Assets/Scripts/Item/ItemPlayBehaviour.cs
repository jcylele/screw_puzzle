using System.Collections.Generic;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;

namespace Item
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ItemPlayBehaviour : BaseItemBehaviour
    {
        public float alpha = 0.8f;

        protected override string BoxColliderPrefabPath => Consts.BoxColliderPlay;
        protected override string CapsuleColliderPrefabPath => Consts.CapsuleColliderPlay;
        protected override string CircleColliderPrefabPath => Consts.CircleColliderPlay;

        private readonly List<JointPlayBehaviour> jointPlayList = new List<JointPlayBehaviour>();

        /// <summary>
        /// used multiple times, so cache it
        /// </summary>
        private JointPlayBehaviour jointPlayPrefab;

        private MaterialPropertyBlock props;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private void Start()
        {
            // colliders are generated in ExpandItem, so generate mesh here
            GenerateMesh();
        }

        protected override void AddJoint(Vector3 jointPosition, Transform colliderTrans)
        {
            var joint2D = gameObject.AddComponent<HingeJoint2D>();
            if (joint2D == null) return;

            joint2D.anchor = jointPosition;

            this.jointPlayPrefab ??= LoadComponent<JointPlayBehaviour>(Consts.JointPlay);

            var jointPlay = Instantiate(this.jointPlayPrefab, transform);
            jointPlay.transform.localPosition = jointPosition;
            jointPlay.joint = joint2D;

            jointPlay.ScrewColor = Game.Instance.GetNextColor();

            jointPlayList.Add(jointPlay);
        }

        public JointPlayBehaviour GetJointByHit(Vector2 hitWorldPos)
        {
            // TODO maybe should be cached, but deletion will be more complex
            var joints = GetComponents<HingeJoint2D>();
            foreach (var jointPlay in jointPlayList)
            {
                if (jointPlay.ScrewColor == ScrewColor.None)
                {
                    continue;
                }

                var distance = Vector2.Distance(jointPlay.WorldPosition, hitWorldPos);
                if (distance <= Consts.JointRadius)
                {
                    return jointPlay;
                }
            }

            return null;
        }

        public void OnFallToGround()
        {
            this.BelongLayerBehaviour.OnItemFallToGround(this);
            Destroy(gameObject);
        }

        public override string ToString()
        {
            return $"{itemInfo.name} of layer {LayerIndex}";
        }

        private void GenerateMesh()
        {
            var mesh = new Mesh
            {
                name = "Procedural Mesh"
            };
            GetComponent<MeshFilter>().mesh = mesh;

            var meshDataArray = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArray[0];
            var generator = new ColliderMeshGenerator();
            var stream = new MultiStream();
            Bounds meshBounds = new Bounds();

            var colliders = this.GetComponentsInChildren<Collider2D>();
            var colliderInfos = new List<ProceduralMeshes.ColliderInfo.BaseColliderInfo>(colliders.Length);
            int totalVertexCount = 0, totalIndexCount = 0;

            foreach (var col in colliders)
            {
                var info = ColliderMeshGenerator.CreateColliderInfo(col);
                colliderInfos.Add(info);
                totalVertexCount += info.VertexCount;
                totalIndexCount += info.TriangleCount * 3;
                meshBounds.Encapsulate(info.Bounds);
                // Debug.Log($"{col}, vertex {info.VertexCount} triangle {info.TriangleCount}");
            }

            stream.Setup(
                meshData,
                colliders.Length,
                totalVertexCount,
                totalIndexCount
            );

            int vertexCount = 0, triangleCount = 0;
            for (int i = 0; i < colliderInfos.Count; i++)
            {
                var info = colliderInfos[i];
                generator.SetColliderInfo(info);
                stream.SetupSubMesh(meshData, i, info, vertexCount, triangleCount);
                generator.GenerateTo(stream);

                vertexCount += info.VertexCount;
                triangleCount += info.TriangleCount;
            }

            mesh.bounds = meshBounds;
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);


            // Set materials
            var meshRenderer = GetComponent<MeshRenderer>();
            var mat = meshRenderer.sharedMaterials[0];
            var mats = new Material[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                mats[i] = mat;
            }

            meshRenderer.materials = mats;

            props ??= new MaterialPropertyBlock();
            var color = Game.Instance.GetLayerColor(LayerIndex);
            color.a = alpha;
            props.SetColor(ColorID, color);
            meshRenderer.SetPropertyBlock(props);
        }
    }
}