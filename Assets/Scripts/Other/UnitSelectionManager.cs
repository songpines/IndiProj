using System;
using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    public event EventHandler UISelectionPanelIsOccupiedOnChanged;

    private Vector2 selectionStartMousePosition;

    public bool isBuildingMode;

    EntityManager entityManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //빌딩모드인지
        BuildingManager.Instance.OnBuildingModeEnter += BuildingManager_OnBuildingModeEnter;
        BuildingManager.Instance.OnBuildingModeExit += BuildingManager_OnBuildingModeExit;
    }

    private void BuildingManager_OnBuildingModeExit(object sender, EventArgs e)
    {
        isBuildingMode = false;
    }

    private void BuildingManager_OnBuildingModeEnter(object sender, EventArgs e)
    {
        isBuildingMode = true;
    }

    private void Update()
    {
        if (!isBuildingMode)
        {
            UnitSelectAndMove();
        }
    }


    
    //Todo entitymanager와 query 캐싱 최적화

    public void UnitSelectAndMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);

            //UISelectionPanelIsOccupiedOnChanged?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Vector2 selectionEndMousePosition = Input.mousePosition;

            //선택된 entity들 deselect
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                Selected selected = selectedArray[i];
                selected.onDeselected = true;
                entityManager.SetComponentData<Selected>(entityArray[i], selected);
            }
            

            //범위 내 유닛 선택
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);

            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float multipleSelectionSizeMin = 40f;
            bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

            if (isMultipleSelection)
            {
                //multiple select
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                for (int i = 0; i < localTransformArray.Length; i++)
                {
                    LocalTransform unitLocalPosition = localTransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalPosition.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        //unit is in selection area
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                        //set onselected as true
                        Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                        selected.onSelected = true;
                        entityManager.SetComponentData(entityArray[i], selected);
                    }
                }
            }
            else
            {
                //single select
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastInput raycastInput = new RaycastInput
                {
                    Start = cameraRay.GetPoint(0f),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNITS_LAYER,
                        GroupIndex = 0
                    }
                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
                {
                    if (entityManager.HasComponent<Unit>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
                    {
                        //hit a unit
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                        selected.onSelected = true;
                        entityManager.SetComponentData(raycastHit.Entity, selected);
                    }
                }
            }

            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);

            //페널 초기화를 위한
            UISelectionPanelIsOccupiedOnChanged?.Invoke(this, EventArgs.Empty);
        }


        if (Input.GetMouseButtonDown(1))
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery entityQuery;
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            //적 유닛이라면 target으로 설정
            EntityQuery physcisEntityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = physcisEntityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(9999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.UNITS_LAYER,
                    GroupIndex = 0
                }
            };
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
            {
                if (entityManager.HasComponent<Enemy>(raycastHit.Entity))
                {
                    //적을 target으로 바로 설정
                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<Target>().Build(entityManager);
                    NativeArray<Target> targetArray = entityQuery.ToComponentDataArray<Target>(Allocator.Temp);
                    for(int i = 0; i < targetArray.Length; i++)
                    {
                        Target target = targetArray[i];
                        target.targetEntity = raycastHit.Entity;
                        targetArray[i] = target;
                    }
                    entityQuery.CopyFromComponentDataArray(targetArray);
                    Debug.Log($"target은 {raycastHit.Entity}다");
                    return;
                }
            }

            //적 유닛이 아니라면 우클릭 장소로 선택된 유닛들 이동
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride>().Build(entityManager);


            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);

            //위치들 생성
            NativeArray<float3> unitPositions = GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);
            for (int i = 0; i < moveOverrideArray.Length; i++)
            {
                MoveOverride moveOverride = moveOverrideArray[i];
                moveOverride.targetPosition = unitPositions[i];
                moveOverrideArray[i] = moveOverride;
                entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);
            }
            entityQuery.CopyFromComponentDataArray(moveOverrideArray);
        }
    }
    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2
        (
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y)
        );

        Vector2 upperRightCorner = new Vector2
        (
            Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y)
        );

        return new Rect
        (
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upperRightCorner.x - lowerLeftCorner.x,
            upperRightCorner.y - lowerLeftCorner.y
        );

    }
    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
        if(positionCount == 0)
        {
            return positionArray;
        }
        positionArray[0] = targetPosition;
        if(positionCount == 1)
        {
            return positionArray;
        }

        float ringSize = 1f;
        int ring = 0;
        int positionIndex = 1;

        while(positionIndex < positionCount)
        {
            int ringPositionCount = 3 + ring * 2;
            float ringAngleUnit = math.PI2 / ringPositionCount;
            float3 offSet = new float3(ringSize * (ring+1), 0, 0);
            for(int i = 0; i < ringPositionCount; i++)
            {
                float angle = i * ringAngleUnit;
                float3 ringVector = math.rotate(quaternion.RotateY(angle), offSet);
                float3 ringPosition = targetPosition + ringVector;

                positionArray[positionIndex] = ringPosition;
                positionIndex++;
                //positioncount보다 크거나 같으면 break
                if (positionIndex >= positionCount) break;
            }
            ring++;
        }
        return positionArray;
    }
    
}
