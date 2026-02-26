using Unity.Burst;
using Unity.Entities;

//리셋되기전
[UpdateBefore(typeof(ResetEventSystem))]
[UpdateAfter(typeof(MeleeAttack))]
[UpdateAfter(typeof(RangedAttackSystem))]
partial struct ResourceGatheringCancelSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceGathering>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var(enabledResourceGathering, health) in SystemAPI.Query<EnabledRefRW<ResourceGathering>, RefRO<Health>>())
        {
            if (health.ValueRO.onHealthChanged)
            {
                enabledResourceGathering.ValueRW = false;
            }
        }
    }

}
