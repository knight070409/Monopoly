using UnityEngine;

public class TileButton : MonoBehaviour
{
    [SerializeField] private Tile tile;

    public void OnClick()
    {
        UIManager.Instance.ShowPropertyDetails(tile);
    }
}
