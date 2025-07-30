using UnityEngine;

public enum TileType
{
    Start,
    Property,
    Jail,
    Chance,
    SpecialProperty,
    GoToJail
   
}

public class Tile : MonoBehaviour
{
    public TileType tileType;

    [Header("Info")]
    public string tileName;
    public int cost;
    public int rent;

    [HideInInspector] public int ownerPlayerId = -1; // -1 means unowned

    public bool IsOwned => ownerPlayerId != -1;
    public bool IsOwnedBy(Player player) => ownerPlayerId == player.PlayerId;
}
