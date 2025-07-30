using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private int stepsMoved = 0;
    public int StepsMoved => stepsMoved;

    public int PlayerId; // 0 for Player 1, 1 for Player 2
    public int Money = 1000;

    public bool IsInJail = false;
    private int jailTurnsRemaining = 0;

    public List<Tile> OwnedProperties = new List<Tile>();

    private bool hasStarted = false;
    private int lapsCompleted = 0;

    public void AdjustMoney(int amount)
    {
        Money += amount;
        Debug.Log($"{name} now has ${Money}");
        UIManager.Instance.ShowPlayerInfo();
    }

    public bool CanAfford(int amount) => Money >= amount;

    public void PayRent(Tile tile)
    {
        Player owner = GameManager.Instance.GetPlayerById(tile.ownerPlayerId);

        if (owner != null && tile.ownerPlayerId != PlayerId)
        {
            if (owner.IsInJail)
            {
                Debug.Log($"{owner.name} is in jail — no rent collected.");
                GameManager.Instance.EndTurn();
                return;
            }

            AdjustMoney(-tile.rent);
            owner.AdjustMoney(tile.rent); 
            Debug.Log($"{name} paid ${tile.rent} rent to {owner.name}");
            UIManager.Instance.AddLog($"{name} paid ${tile.rent} rent to {owner.name}");
        }

        if (GameManager.Instance.RolledSix && !GameManager.Instance.CurrentPlayer.IsInJail)
        {
            Debug.Log("Player passed but had rolled a 6 — gets another turn.");
            GameManager.Instance.EnableDiceRoll();
        }
        else
        {
            GameManager.Instance.EndTurn();
        }
    }

    public IEnumerator MoveSteps(int steps)
    {
        int totalTiles = GameManager.Instance.Path.Length;

        for (int i = 1; i <= steps; i++)
        {
            int previousStep = stepsMoved;
            stepsMoved = (stepsMoved + 1) % totalTiles;

            if (hasStarted && (previousStep+1) > stepsMoved)
            {
                lapsCompleted++;
                AdjustMoney(200);
                Debug.Log($"{name} passed START. Collected $200!");
                UIManager.Instance.AddLog($"{name} collected $200!");
            }

            Vector3 tilePos = GameManager.Instance.GetTileTransform(stepsMoved).position;
            transform.position = tilePos;

            yield return new WaitForSeconds(0.35f);
        }
        hasStarted = true;

        AdjustPlayerOverlap();
        HandleTileEffect();
    }

    private void AdjustPlayerOverlap()
    {
        Player[] playersOnSameTile = GameManager.Instance.GetPlayersOnTile(stepsMoved);

        // If only one player is on this tile, reset to normal scale and center position
        if (playersOnSameTile.Length == 1)
        {
            transform.position = GameManager.Instance.GetTileTransform(stepsMoved).position;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            return;
        }

        for (int i = 0; i < playersOnSameTile.Length; i++)
        {
            Player p = playersOnSameTile[i];

            float offsetX = (i - playersOnSameTile.Length / 2f) * 0.4f;

            Vector3 tilePos = GameManager.Instance.GetTileTransform(stepsMoved).position;
            p.transform.position = tilePos + new Vector3(offsetX, 0f, 0f);

            p.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private void HandleTileEffect()
    {
        Tile tile = GameManager.Instance.Path[stepsMoved].GetComponent<Tile>();

        if (tile == null) return;

        switch (tile.tileType)
        {
            case TileType.Start:
                AdjustMoney(200);
                Debug.Log($"{name} landed on START. Collect $200!");
                UIManager.Instance.AddLog($"{name} landed on START and collected $200!");
                GameManager.Instance.EndTurn();
                break;

            case TileType.Property:
                if (!tile.IsOwned)
                {
                    UIManager.Instance.ShowBuyPropertyUI(tile, this);
                }
                else if (tile.ownerPlayerId != this.PlayerId)
                {
                    PayRent(tile);
                }
                else
                {
                    Debug.Log($"{name} landed on {tile.tileName}. Property costs ${tile.cost}");
                    GameManager.Instance.EndTurn();
                }
                break;

            case TileType.Chance:
                Debug.Log($"{name} landed on CHANCE");

                float roll = Random.value; // Value between 0.0 and 1.0

                if (roll <= 0.9f)
                {
                    // 90% case: Lose 10–30% of money
                    float percent = Random.Range(0.1f, 0.31f); // 10% to 30%
                    int loss = Mathf.FloorToInt(Money * percent);
                    AdjustMoney(-loss);
                    Debug.Log($"{name} lost ${loss} (Chance card: {percent * 100:F1}%)");
                    UIManager.Instance.AddLog($"{name} lost ${loss}");
                }
                else
                {
                    // 10% case: Gain 50%
                    int bonus = Mathf.FloorToInt(Money * 0.5f);
                    AdjustMoney(bonus);
                    Debug.Log($"{name} gained a bonus of ${bonus} from Chance!");
                    UIManager.Instance.AddLog($"{name} gained a bonus of ${bonus}");
                }
                if (GameManager.Instance.RolledSix && !GameManager.Instance.CurrentPlayer.IsInJail)
                {
                    Debug.Log("Player passed but had rolled a 6 — gets another turn.");
                    //GameManager.Instance.EnableDiceRoll(); // Allow dice roll again
                }
                else
                {
                    GameManager.Instance.EndTurn();
                }
                break;

            case TileType.Jail:
                if (!IsInJail)
                {
                    IsInJail = true;
                    jailTurnsRemaining = 3;

                    Debug.Log($"{name} landed on Jail and is now in jail for {jailTurnsRemaining} turns.");
                    UIManager.Instance.AddLog($"{name} landed on Jail and is now in jail for {jailTurnsRemaining} turns.");
                }
                else
                {
                    Debug.Log($"{name} is already in jail. {jailTurnsRemaining} turns remaining.");
                    UIManager.Instance.AddLog($"{name} is already in jail. {jailTurnsRemaining} turns remaining.");
                }

                GameManager.Instance.EndTurn();
                break;

            case TileType.GoToJail:
                Debug.Log($"{name} going jail!");
                GoToJail();
                break;

            case TileType.SpecialProperty:
                if (!tile.IsOwned)
                {
                    UIManager.Instance.ShowBuyPropertyUI(tile, this);
                }
                else if (tile.ownerPlayerId != this.PlayerId)
                {
                    PayRent(tile);
                }
                else
                {
                    Debug.Log($"{name} landed on {tile.tileName}. Property costs ${tile.cost}");
                }
                GameManager.Instance.EndTurn();
                break;

            default:
                Debug.Log($"{name} landed on empty tile.");
                break;
        }
    }

    private void GoToJail()
    {
        int jailIndex = GameManager.Instance.JailTileIndex;
        int stepsToJail = (jailIndex - stepsMoved + GameManager.Instance.Path.Length) % GameManager.Instance.Path.Length;

        StartCoroutine(MoveToJail(stepsToJail, jailIndex));

        IsInJail = true;
        jailTurnsRemaining = 3;

        Debug.Log($"{name} is sent to jail and will miss {jailTurnsRemaining} turns.");
        UIManager.Instance.AddLog($"{name} is sent to jail and will miss {jailTurnsRemaining} turns.");
    }

    private IEnumerator MoveToJail(int steps, int jailIndex)
    {
        yield return StartCoroutine(MoveSteps(steps));

        stepsMoved = jailIndex; // Just to be safe
        transform.position = GameManager.Instance.GetTileTransform(jailIndex).position;

        GameManager.Instance.EndTurn();
    }

    public void DecreaseJailTurn()
    {
        jailTurnsRemaining--;

        if (jailTurnsRemaining <= 0)
        {
            IsInJail = false;
            Debug.Log($"{name} is released from jail.");
            UIManager.Instance.AddLog($"{name} is released from jail.");
        }
        else
        {
            Debug.Log($"{name} is still in jail. Turns remaining: {jailTurnsRemaining}");
            UIManager.Instance.AddLog($"{name} is still in jail. Turns remaining: {jailTurnsRemaining}");
        }
    }
}