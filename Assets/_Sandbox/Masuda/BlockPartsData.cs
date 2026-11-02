using UnityEngine;

[CreateAssetMenu(fileName = "BlockPartsData", menuName = "Scriptable Objects/BlockPartsData")]
public class BlockPartsData : ScriptableObject
{
    // ブロックパーツの耐久値
    public int durability;

    // ブロックパーツの見た目に使用するマテリアル
    public Material material;
}
