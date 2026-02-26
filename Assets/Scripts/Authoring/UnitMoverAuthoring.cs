using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public Vector3 targetPosition;
    public class Baker : Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMover
            {
                speed = authoring.speed,
                rotationSpeed = authoring.rotationSpeed,
                targetPosition = authoring.targetPosition,
            });
        }
    }
}

public struct UnitMover : IComponentData
{
    public float speed;
    public float rotationSpeed;
    public float3 targetPosition;

}
