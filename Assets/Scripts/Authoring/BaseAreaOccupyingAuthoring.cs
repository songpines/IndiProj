using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BaseAreaOccupyingAuthoring : MonoBehaviour
{
    public int2 baseSize;
    public class Baker : Baker<BaseAreaOccupyingAuthoring>
    {
        public override void Bake(BaseAreaOccupyingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            var collider = GetComponent<BoxCollider>();
            Vector3 colliderSize = collider.bounds.size;
            float colliderSideSize = math.max(colliderSize.x, colliderSize.z);
            AddComponent(entity, new BaseAreaOccupying
            {
                baseSize = authoring.baseSize,
                radius = colliderSideSize * 0.5f,
                isOccupying = false,
            });
        }
    }
}

public struct BaseAreaOccupying : IComponentData, IEnableableComponent
{
    public bool isOccupying;
    public int2 baseSize;
    public float radius;
}
