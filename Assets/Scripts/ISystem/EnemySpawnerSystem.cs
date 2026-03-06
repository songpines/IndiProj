using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawner>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRO<LocalTransform> localTransform,
            RefRW<EnemySpawner> enemySpawner, EnabledRefRW<EnemySpawner> enabledEnemySpawner) in
        SystemAPI.Query<RefRO<LocalTransform>, RefRW<EnemySpawner>, EnabledRefRW<EnemySpawner>>()){

            enemySpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if(enemySpawner.ValueRO.timer > 0f)
            {
                continue;
            }
            enemySpawner.ValueRW.timer = enemySpawner.ValueRO.timerMax;

            Entity enemyEntity = state.EntityManager.Instantiate(enemySpawner.ValueRO.enemyToSpawn);
            SystemAPI.SetComponent<LocalTransform>(enemyEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            enemySpawner.ValueRW.currentNumber++;

            entityCommandBuffer.AddComponent(enemyEntity, new RandomWalking
            {
                originPosition = localTransform.ValueRO.Position,
                targetPosition = localTransform.ValueRO.Position,
                distanceMin = enemySpawner.ValueRO.randomWalkingDistanceMin,
                distanceMax = enemySpawner.ValueRO.randomWalkingDistanceMax,
                random = new Unity.Mathematics.Random((uint)enemyEntity.Index)
            });

            if(enemySpawner.ValueRO.currentNumber >= enemySpawner.ValueRO.maxNumber)
            {
                enemySpawner.ValueRW.timerMax = 0.1f;
            }
            if(enemySpawner.ValueRO.currentNumber >= enemySpawner.ValueRO.maxNumber + 100)
            {
                enabledEnemySpawner.ValueRW = false;
            }
        }
    }

}
