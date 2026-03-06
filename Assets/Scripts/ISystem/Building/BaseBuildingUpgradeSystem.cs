using Unity.Burst;
using Unity.Entities;

partial struct BaseBuildingUpgradeSystem : ISystem
{
    float timer;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BaseBuildingUnitUpgrade>();
        timer = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (RefRO<BaseBuildingUnitUpgrade> unitProduction in SystemAPI.Query<RefRO<BaseBuildingUnitUpgrade>>())
        {

        }

    }
}
