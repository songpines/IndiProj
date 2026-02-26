using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


partial struct RangedAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RangedAttack>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRW<LocalTransform> localTransform, 
            RefRW<UnitMover> unitMover,
            RefRW<RangedAttack> rangedAttack, 
            RefRO<Target> target) 
            in SystemAPI.Query<RefRW<LocalTransform>, RefRW<UnitMover>,RefRW<RangedAttack>, RefRO<Target>>().WithDisabled<MoveOverride>().WithDisabled<ResourceGathering>()){

            if (target.ValueRO.targetEntity == Entity.Null || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
            {
                continue;
            }

            rangedAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (rangedAttack.ValueRO.timer > 0)
            {
                continue;
            }

            


            rangedAttack.ValueRW.timer = rangedAttack.ValueRO.timerMax;

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            //적을 향하게
            float3 aimDirection = math.normalize(targetLocalTransform.Position - localTransform.ValueRO.Position);

            quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());

            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);


            float distanceToTarget =  math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

            //거리가 범위 밖이면 가까이 이동, 안쪽이면 공격
            if(distanceToTarget > rangedAttack.ValueRO.attackDistance)
            {
                //범위 밖
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }



            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            //세팅된 spawnposition에서 쏘게
            float3 bulletSpawnWorldPos = localTransform.ValueRO.TransformPoint(rangedAttack.ValueRO.bulletSpawnPosition);

            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPos));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = rangedAttack.ValueRO.damageAmount;
            

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }


}
