using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial struct HealthDeadTestSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Health>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        EntityCommandBuffer entityCommandBuffer = 
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRO<Health> health, Entity entity) in SystemAPI.Query<
           RefRO<Health>>().WithEntityAccess())
        {
            if(health.ValueRO.healthAmount <= 0)
            {
                //unit dead
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
