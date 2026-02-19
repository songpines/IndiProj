using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct TargetResetSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
      foreach(RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if(target.ValueRO.targetEntity != Entity.Null)
            {
                if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity))
                {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
        }  
    }

}
