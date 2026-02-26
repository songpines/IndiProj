using System.Diagnostics;
using System.Resources;
using Unity.Burst;
using Unity.Entities;

//deliver된 다음에 update
[UpdateAfter(typeof(ResourceGatheringDeliveringSystem))]
partial class ResourceManagerSystemBase : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<ResourceManage>();
    }

    protected override void OnUpdate()
    {
        foreach(RefRW<ResourceManage> resourceManage in SystemAPI.Query<RefRW<ResourceManage>>().WithChangeFilter<ResourceManage>())
        {
            UnityEngine.Debug.Log("리소스 업데이트");

            ResourceManager.Instance.Stone = resourceManage.ValueRO.StoneResource;
            ResourceManager.Instance.Coral = resourceManage.ValueRO.CoralResource;

        }
    }
}
