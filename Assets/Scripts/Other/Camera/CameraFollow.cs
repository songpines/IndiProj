using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //카메라 이동 속도
    public float cameraMoveSpeed;
    //가장자리 경계
    public float edge;
    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width - edge) moveDir.x += 1;
        if (Input.mousePosition.x <= edge) moveDir.x -= 1;
        if (Input.mousePosition.y >= Screen.height - edge) moveDir.z += 1;
        if (Input.mousePosition.y <= edge) moveDir.z -= 1;

        moveDir = moveDir.normalized;
        transform.Translate(moveDir * cameraMoveSpeed * Time.deltaTime, Space.World);
    }
}
