using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MainCameraTransformAuthoring : MonoBehaviour
{
    public class Baker : Baker<MainCameraTransformAuthoring>
    {
        public override void Bake(MainCameraTransformAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new MainCameraTransform());
        }
    }
}

public struct MainCameraTransform : IComponentData
{
    public float3 cameraTransform;

}
