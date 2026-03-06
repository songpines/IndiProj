using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerClickHandler
{
    public static Minimap Instance { get; private set; }
    private RectTransform recT;
    private float width;
    private float height;

    Vector3 offset;

    public event EventHandler<OnMinimapClickEventArgs> OnMinimapClick;
    public class OnMinimapClickEventArgs
    {
        public float posX;
        public float posY;
    }

    private void Awake()
    {
        Instance = this;
        recT = GetComponent<RectTransform>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(recT, Input.mousePosition, null, out Vector2 minmapToLocalPoint2d))
        {


            OnMinimapClick?.Invoke(this, new OnMinimapClickEventArgs
            {
                posX = minmapToLocalPoint2d.x/recT.rect.width,
                posY = minmapToLocalPoint2d.y/ recT.rect.height
            });
        }
    }
}
