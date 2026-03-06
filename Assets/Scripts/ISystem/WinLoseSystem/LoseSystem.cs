using Unity.Burst;
using Unity.Entities;

partial class LoseSystem : SystemBase
{

    protected override void OnCreate()
    {
        RequireForUpdate<Friendly>();
    }

    protected override void OnUpdate()
    {
        foreach (RefRO<Friendly> friendly in SystemAPI.Query<RefRO<Friendly>>())
        {

        }
    }



    protected override void OnStopRunning()
    {
        GameManager.Instance.Lost();
    }
}
