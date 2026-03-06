using Unity.Burst;
using Unity.Entities;

partial struct ResourceDepletedDeleteSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Resource>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer =
             SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRO<Resource> resource, Entity entity) in SystemAPI.Query<RefRO<Resource>>().WithEntityAccess())
        {
            if(resource.ValueRO.resourceAmount <= 0)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }

}
