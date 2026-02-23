using Unity.Cinemachine;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform[] mapEdges; //기본 4개


    void Start()
    {
        //맵 중심으로 세팅
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GridConfig gridconfig = entityManager.CreateEntityQuery(typeof(GridConfig)).GetSingleton<GridConfig>();

        transform.position += new Vector3(gridconfig.Width/2, 0f, gridconfig.Height/2);

        //맵 꼭지점들이 모두 카메라에 들어오게 미니맵 카메라 세팅
        if(mapEdges != null)
        {
            mapEdges[0].transform.position = new Vector3(0,0,0);
            mapEdges[1].transform.position = new Vector3(0, 0, gridconfig.Height);
            mapEdges[2].transform.position = new Vector3(gridconfig.Width, 0, 0);
            mapEdges[3].transform.position = new Vector3(gridconfig.Width, 0, gridconfig.Height);
        }

        Debug.Log(gridconfig.Width);
    }


}
