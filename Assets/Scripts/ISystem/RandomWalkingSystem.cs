using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RandomWalking>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridConfig = SystemAPI.GetSingleton<GridConfig>();

        foreach((RefRW<RandomWalking> randomWalking, 
            RefRW<UnitMover> unitMover,
            RefRO<LocalTransform> localTransform) in
            SystemAPI.Query<
                RefRW<RandomWalking>,
                RefRW<UnitMover>,
                RefRO<LocalTransform>>())
        {
            if(math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) < UnitMoverSystem.REACHED_DISTANCESQ)
            {
                //도착
                
                Random random = randomWalking.ValueRO.random;

                float3 randomDirection = new float3(random.NextFloat(-1f, +1f), 0, random.NextFloat(-1f, +1f));
                randomDirection = math.normalize(randomDirection);

                randomWalking.ValueRW.targetPosition =
                    randomWalking.ValueRO.originPosition +
                    randomDirection * random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);

                if(randomWalking.ValueRO.targetPosition.x > gridConfig.Width || 
                    randomWalking.ValueRO.targetPosition.z > gridConfig.Height || 
                    randomWalking.ValueRO.targetPosition.x < 0 ||
                    randomWalking.ValueRO.targetPosition.z < 0)
                {
                    randomWalking.ValueRW.targetPosition = localTransform.ValueRO.Position;
                }
                randomWalking.ValueRW.random = random;
            }
            else
            {
                //더 가야함.
                unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
            }
        }
    }
}
