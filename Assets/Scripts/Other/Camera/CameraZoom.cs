using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    [SerializeField] private float defaultZoomSpeed;
    private float currentZoomSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    //드래그 앤 드랍
    [SerializeField] private CinemachineFollow cinemachineFollow;

    // Update is called once per frame
    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        //스크롤 조작할 때만
        if (scrollInput == 0) return;

        currentZoomSpeed = (cinemachineFollow.FollowOffset.y / maxZoom) * defaultZoomSpeed;
        //카메라 줌
        if (cinemachineFollow.FollowOffset.y < minZoom)
        {
            scrollInput = (scrollInput > 0) ? 0 : scrollInput;
        }

        if(cinemachineFollow.FollowOffset.y > maxZoom)
        {
            scrollInput = (scrollInput > 0) ? scrollInput : 0;
        }

        //inverse
        cinemachineFollow.FollowOffset.y -= scrollInput * currentZoomSpeed;
    }
}
