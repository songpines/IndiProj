using Unity.Burst;
using Unity.Entities;

partial class WinSystem : SystemBase
{
    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<Enemy>();
    }

    protected override void OnUpdate()
    {
        foreach(RefRO<Enemy> enemy in SystemAPI.Query<RefRO<Enemy>>())
        {

        }
    }



    protected override void OnStopRunning()
    {
        GameManager.Instance.Win();
    }

    
}
