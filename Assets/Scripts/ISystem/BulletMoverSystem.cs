using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct BulletMoverSystem : ISystem
{


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach((RefRW<LocalTransform> localTransform, 
            RefRO<Bullet> Bullet,
            RefRO<Target> target,
            Entity entity)
            in SystemAPI.Query<RefRW<LocalTransform>, 
            RefRO<Bullet>, RefRO<Target>>().WithEntityAccess())
        {
            if(target.ValueRO.targetEntity == Entity.Null || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);

            float3 moveDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * Bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);

            if(distanceAfterSq > distanceBeforeSq)
            {
                //overshot
                localTransform.ValueRW.Position = targetLocalTransform.Position;
            }

            float destroyDistanceSq = 0.0002f;
            if(math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) < destroyDistanceSq)
            {
                //타격판정
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= Bullet.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;

                entityCommandBuffer.DestroyEntity(entity);
            }


        }
    }
}
