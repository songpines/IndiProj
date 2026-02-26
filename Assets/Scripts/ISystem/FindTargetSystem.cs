using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

partial struct FindTargetSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FindTarget>();
    }



    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        CollisionFilter targetFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << GameAssets.UNITS_LAYER,
            GroupIndex = 0
        };

        foreach ((RefRO<LocalTransform> localTransform, 
            RefRW<FindTarget> findTarget,
            RefRW<Target> target
            ) in SystemAPI.Query<RefRO<LocalTransform>, 
            RefRW<FindTarget>, RefRW<Target>>().WithDisabled<ResourceGathering>()) //자원 채취 시에는 적을 찾지 않음
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(findTarget.ValueRO.timer > 0f)
            {
                //time not elapsed
                continue;
            }
            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;


            distanceHitList.Clear();
            if(collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range, ref distanceHitList, targetFilter))
            {
                foreach(DistanceHit distanceHit in distanceHitList)
                {
                    if (!SystemAPI.HasComponent<Unit>(distanceHit.Entity))
                    {
                        continue;
                    }
                    Unit targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);

                    if(findTarget.ValueRO.targetFaction == targetUnit.faction)
                    {
                        //valid target
                        target.ValueRW.targetEntity = distanceHit.Entity;
                        break;
                    }
                }
            }


        }
    }
}
