using UnityEngine;

[CreateAssetMenu(fileName = "BlockPartsData", menuName = "Scriptable Objects/BlockPartsData")]
public class BlockPartsData : ScriptableObject
{
    // ブロックパーツの耐久値
    [SerializeField] int durability;

    // ブロックパーツの見た目に使用するマテリアル
    [SerializeField] Material material;

    /// <summary>
    /// ブロックパーツの耐久値
    /// </summary>
    public int Durability => durability;

    /// <summary>
    /// ブロックパーツの見た目に使用するマテリアル
    /// </summary>
    public Material Material => material;
}
