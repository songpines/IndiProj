using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ResourceAuthoring : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceAmount;

    public class Baker : Baker<ResourceAuthoring>
    {
        public override void Bake(ResourceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            var collider = GetComponent<BoxCollider>();
            //xz축만
            Vector3 colliderSize = collider.bounds.size;
            float colliderSideSize = math.max(colliderSize.x, colliderSize.z);
            AddComponent(entity, new Resource
            {
                resourceType = authoring.resourceType,
                resourceAmount = authoring.resourceAmount,
                radius = colliderSideSize * 0.5f
            });
        }
    }

}



public struct Resource : IComponentData
{
    public ResourceType resourceType;
    public int resourceAmount;

    //자원 반지름
    public float radius;
}


public enum ResourceType
{
    Coral,
    Stone,
}


