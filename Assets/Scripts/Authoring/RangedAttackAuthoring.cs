using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class RangedAttackAuthoring : MonoBehaviour
{
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public Transform bulletSpawnPositionTransform;
    public class Baker : Baker<RangedAttackAuthoring>
    {

        public override void Bake(RangedAttackAuthoring authoring)
        {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RangedAttack
            {
                timerMax = authoring.timerMax,
                damageAmount = authoring.damageAmount,
                attackDistance = authoring.attackDistance,
                bulletSpawnPosition = authoring.bulletSpawnPositionTransform.localPosition
            });
    }
}
    
}

public struct RangedAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public int damageAmount;
    public float attackDistance;
    public float3 bulletSpawnPosition;
}
