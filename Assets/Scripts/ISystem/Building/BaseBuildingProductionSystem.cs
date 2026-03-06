using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BaseBuildingProductionSystem : ISystem
{
    float timer;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BaseBuildingUnitProduction>();
        timer = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((EnabledRefRW< BaseBuildingUnitProduction> enabledUnitProduction, RefRW < BaseBuildingUnitProduction> unitProduction, RefRO< LocalTransform > localTransform) 
            in SystemAPI.Query< EnabledRefRW < BaseBuildingUnitProduction > ,RefRW <BaseBuildingUnitProduction>, RefRO<LocalTransform>>())
        {
            timer += SystemAPI.Time.DeltaTime;

            if (timer > unitProduction.ValueRO.timerMax)
            {
                Entity summonEntity = state.EntityManager.Instantiate(entitiesReferences.GetUnit(unitProduction.ValueRO.productionType));
                SystemAPI.SetComponent<LocalTransform>(summonEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
                SystemAPI.SetComponent<MoveOverride>(summonEntity, new MoveOverride { targetPosition = localTransform.ValueRO.Position + new float3(4, 0, -1) });
                SystemAPI.SetComponentEnabled<MoveOverride>(summonEntity, true);
                timer = 0f;
                enabledUnitProduction.ValueRW = false;
            }
        }

    }

}
