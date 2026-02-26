using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

partial struct UnitMoverSystem : ISystem
{
    public const float REACHED_DISTANCESQ = 0.2f;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitMover>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob { deltaTime = SystemAPI.Time.DeltaTime };

        unitMoverJob.ScheduleParallel();

        //foreach ((RefRW<LocalTransform> localTransform, RefRO<UnitMover> unitMover, RefRW<PhysicsVelocity> physicsVelocity) 
        //    in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
        //{



        //    localTransform.ValueRW.Position += new float3(unitMover.ValueRO.speed, 0, 0);
        //}
    }
}




[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;

    
    public void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        float reachedTargetDistanceSq = UnitMoverSystem.REACHED_DISTANCESQ;



        if(math.lengthsq(moveDirection) <= reachedTargetDistanceSq)
        {
            //reached the target position
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

        moveDirection = math.normalize(moveDirection);
        localTransform.Rotation =
            math.slerp(localTransform.Rotation,
            quaternion.LookRotation(moveDirection, math.up()),
            deltaTime * unitMover.rotationSpeed);

        physicsVelocity.Linear = moveDirection * unitMover.speed;
        physicsVelocity.Angular = float3.zero;
    }


}
