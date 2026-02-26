using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //카메라 이동 속도
    public float cameraMoveSpeed;
    //가장자리 경계
    public float edge;
    //offset
    [SerializeField] private float offset;

    //맵 크기 정보
    private float width;
    private float height;
    
    GridConfig gridConfig;
    private void Start()
    {
        EntityManager entityManager = entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(GridConfig));
        gridConfig = entityQuery.GetSingleton<GridConfig>();

        width = gridConfig.Width;
        height = gridConfig.Height;
    }
    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width - edge) moveDir.x += 1;
        if (Input.mousePosition.x <= edge) moveDir.x -= 1;
        if (Input.mousePosition.y >= Screen.height - edge) moveDir.z += 1;
        if (Input.mousePosition.y <= edge) moveDir.z -= 1;

        moveDir = moveDir.normalized;
        if(transform.position.x >= -offset && transform.position.x <= width + offset && transform.position.z >= -offset && transform.position.z <= height + offset)
        {
            transform.Translate(moveDir * cameraMoveSpeed * Time.deltaTime, Space.World);
        }
        
        if(transform.position.x < -offset || transform.position.x > offset + width)
        {
            transform.position = (transform.position.x < offset) ? new Vector3(-offset, 0f, transform.position.z) : new Vector3(offset + width, 0f, transform.position.z);
        }
        if (transform.position.z < -offset || transform.position.z > offset + height)
        {
            transform.position = (transform.position.z < offset) ? new Vector3(transform.position.x, 0f, -offset) : new Vector3(transform.position.x, 0f, offset + height);
        }
        
    }
}
