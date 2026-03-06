using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial class MainCameraTransformSystemBase : SystemBase
{

    protected override void OnCreate()
    {
        RequireForUpdate<MainCameraTransform>();
    }



    protected override void OnUpdate()
    {
        foreach(RefRW<MainCameraTransform> mc in SystemAPI.Query<RefRW<MainCameraTransform>>()){
            if (Camera.main != null)
            {
                mc.ValueRW.cameraTransform = Camera.main.transform.forward;
            }
        }
    }
}
