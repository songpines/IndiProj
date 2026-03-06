using System.Collections;
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
        //EntityManager entityManager = entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(GridConfig));
        //gridConfig = entityQuery.GetSingleton<GridConfig>();
        StartCoroutine(WaitForGridConfig());
        width = gridConfig.Width;
        height = gridConfig.Height;

        //미니맵과 연결
        Minimap.Instance.OnMinimapClick += Minimap_OnMinimapClick;
    }

    IEnumerator WaitForGridConfig()
    {
        EntityManager entityManager = entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(GridConfig));
        while (entityQuery.IsEmpty)
        {
            entityQuery = entityManager.CreateEntityQuery(typeof(GridConfig));
            yield return null;
        }
        gridConfig = entityQuery.GetSingleton<GridConfig>();

        width = gridConfig.Width;
        height = gridConfig.Height;
    }

    private void Minimap_OnMinimapClick(object sender, Minimap.OnMinimapClickEventArgs e)
    {
        transform.position = new Vector3(e.posX * gridConfig.Width, 0f, e.posY * gridConfig.Height);
        Debug.Log(e.posX);
        Debug.Log(e.posY);
    }

    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        if ((Input.mousePosition.x >= Screen.width - edge && Input.mousePosition.x < Screen.width + Screen.width/4f) || Input.GetKey(KeyCode.D)) moveDir.x += 1;
        if (Input.mousePosition.x <= edge || Input.GetKey(KeyCode.A)) moveDir.x -= 1;
        if ((Input.mousePosition.y >= Screen.height - edge &&  Input.mousePosition.z < Screen.height + Screen.height / 10f) || Input.GetKey(KeyCode.W)) moveDir.z += 1;
        if (Input.mousePosition.y <= edge || Input.GetKey(KeyCode.S)) moveDir.z -= 1;

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
