using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{

    public int unitId;
    public Faction faction;
    public class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit
            {
                faction = authoring.faction,
                unitId = authoring.unitId
            });
        }
    }


}

public struct Unit : IComponentData
{
    public int unitId;
    public Faction faction;
}

public enum Faction
{
    Friendly,
    Neutral,
    Enemy,
}
