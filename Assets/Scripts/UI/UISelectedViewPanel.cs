using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UISelectedViewPanel : MonoBehaviour
{
    //Todo
    //선택된 유닛 숫자에 따라 grid 크기 변경

    private RectTransform rect;
    private GridLayoutGroup gridLayoutGroup;
    [SerializeField] public int defaultUnitSelectionNumber; //20
    private int defaultUnitSelectionCellSize;
    [SerializeField] private float UnitSelectionCellSize;

    //유닛 sprite db so
    [SerializeField] UnitSpriteSO unitSpriteDB;

    //유닛 sprite&id
    private Image[] selectedUnitSprite;
    private int[] selectedUnitId;

    //on업데이트 제한
    public bool isOccupied;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();

        

        //엔티티로 만들기
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entity = entityManager.CreateEntity();
        entityManager.AddComponentData(entity, new UISelectedViewPanelData { Value = this });

        //on업데이트 제한
        isOccupied = false;



    }

    private void Start()
    {
        //초기 세팅
        selectedUnitSprite = GetComponentsInChildren<Image>().Where<Image>(x => x.gameObject != gameObject).ToArray();
        selectedUnitId = new int[defaultUnitSelectionNumber];

        //select할 때 isOccupied 초기화
        UnitSelectionManager.Instance.UISelectionPanelIsOccupiedOnChanged += UnitSelectionManager_UISelectionPanelIsOccupiedOnChanged;
    }

    private void UnitSelectionManager_UISelectionPanelIsOccupiedOnChanged(object sender, System.EventArgs e)
    {
        isOccupied = false;
        Clear();
    }

    public void SetSelectedPanel(int[] unitId)
    {
        for (int i = 0; i < unitId.Length; i++)
        {
            if (i > selectedUnitId.Length) break;
            
            selectedUnitId[i] = unitId[i];
        }
        VisualUpdate();
        CheckOccupied();
    }

    private void VisualUpdate()
    {
        for(int i = 0; i < selectedUnitId.Length; i++)
        {
            selectedUnitSprite[i].sprite = unitSpriteDB.GetSprite(selectedUnitId[i]);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < selectedUnitId.Length; i++)
        {
            selectedUnitId[i] = 0;
        }
        VisualUpdate();
        CheckOccupied();
    }

    private void CheckOccupied()
    {
        for (int i = 0; i < selectedUnitId.Length; i++)
        {
            if (selectedUnitId[i] != 0)
            {
                isOccupied = true;
                return;
            }
        }
        isOccupied = false;
    }


    //체력 업데이트
    //public void

    //private float GetCellSize(int selectedNumber)
    //{
    //    if(selectedNumber == 0)
    //    {
    //        return 0;
    //    }
    //    //
    //    float maxHorizontalSize = (rect.rect.width - (gridLayoutGroup.padding.right + gridLayoutGroup.padding.left));
    //    float maxVerticalSize = (rect.rect.height - (gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom));
    //    return Mathf.Min(maxHorizontalCellSize, maxVerticalCellSize);
    //}
}


//엔티티 등록을 위한 icomponentdata

public class UISelectedViewPanelData : IComponentData
{
    public UISelectedViewPanel Value;
}

