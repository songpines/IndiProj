using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class UnitSpriteSO : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<IdSpriteStr> unitSpriteDic = new List<IdSpriteStr>();
    private Dictionary<int, Sprite> spriteDict = new Dictionary<int, Sprite>();
    public void OnAfterDeserialize()
    {
        spriteDict.Clear();
        //dictionary
        for (int i = 0; i < unitSpriteDic.Count; i++)
        {
            spriteDict.Add(unitSpriteDic[i].unitId, unitSpriteDic[i].sprite);
        }
    }

    public void OnBeforeSerialize()
    {
        
    }

    public Sprite GetSprite(int unitId)
    {
        if(spriteDict.TryGetValue(unitId, out Sprite sprite))
        {
            return sprite;
        }

        return null;
    }

}

[System.Serializable]
public struct IdSpriteStr
{
    public int unitId;
    public Sprite sprite;
}