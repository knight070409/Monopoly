using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image player1Image;
    [SerializeField] private Image player2Image;

    [Header("Text")]
    [SerializeField] private TMP_Text turnLabel;

    [Header("Buy Panel")]
    [SerializeField] private TMP_Text tileNameText;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject passButton;
    [SerializeField] private GameObject closeButton;

    [Header("Player Info")]
    [SerializeField] private TMP_Text player1Text;
    [SerializeField] private TMP_Text player2Text;

    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Log")]
    [SerializeField] private TMP_Text logText;

    private Tile currentTile;
    private Player currentPlayer;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTurnChanged += UpdateTurnUI;
            UpdateTurnUI(GameManager.Instance.CurrentPlayerIndex);
        }
        panel.SetActive(false);
        UIManager.Instance.ShowPlayerInfo();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTurnChanged -= UpdateTurnUI;
        }
    }

    private void UpdateTurnUI(int currentPlayerIndex)
    {
        bool isPlayer1Turn = currentPlayerIndex == 0;

        player1Image.enabled = isPlayer1Turn;
        player2Image.enabled = !isPlayer1Turn;

        if (turnLabel != null)
        {
            turnLabel.text = isPlayer1Turn ? "Player 1's turn!" : "Player 2's turn!";
        }
    }

    public void ShowBuyPropertyUI(Tile tile, Player player)
    {
        if (player.Money < tile.cost)
        {
            Debug.Log($"{player.name} can't afford {tile.tileName} (${tile.cost})");
            GameManager.Instance.EndTurn();
            return;
        }

        currentTile = tile;
        currentPlayer = player;

        tileNameText.text =
            $"{tile.tileType} : {tile.tileName}\n" +
            $"Cost: ${tile.cost}\n" +
            $"Rent: ${tile.rent}";

        buyButton.SetActive(true);
        passButton.SetActive(true);
        closeButton.SetActive(false);
        panel.SetActive(true);

    }

    public void BuyProperty()
    {
        if (currentPlayer.CanAfford(currentTile.cost))
        {
            currentPlayer.AdjustMoney(-currentTile.cost);
            currentTile.ownerPlayerId = currentPlayer.PlayerId;
            currentPlayer.OwnedProperties.Add(currentTile);

            ShowPlayerInfo();

            Debug.Log($"{currentPlayer.name} bought {currentTile.tileName}");
            AddLog($"{currentPlayer.name} bought {currentTile.tileName} for ${currentTile.cost}");
        }
        else
        {
            Debug.Log($"{currentPlayer.name} can't afford {currentTile.tileName}");
        }

        Pass();
    }

    public void Pass()
    {
        panel.SetActive(false);
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.RolledSix && !GameManager.Instance.CurrentPlayer.IsInJail)
            {
                Debug.Log("Player passed but had rolled a 6 — gets another turn.");
                GameManager.Instance.EnableDiceRoll(); // Allow dice roll again
            }
            else
            {
                GameManager.Instance.EndTurn();
            }
        }
    }

    public void ShowPropertyDetails(Tile tile)
    {
        string ownerText = tile.ownerPlayerId == -1 ? "None" : $"Player {tile.ownerPlayerId + 1}";

        tileNameText.text =
            $"{tile.tileType} : {tile.tileName}\n" +
            $"Cost: ${tile.cost}\n" +
            $"Rent: ${tile.rent}\n" +
            $"Owner: {ownerText}";

        panel.SetActive(true);
        closeButton.SetActive(true);
        buyButton.SetActive(false);
        passButton.SetActive(false);
    }

    public void ClosePropertyDetails()
    {
        panel.SetActive(false);
        closeButton.SetActive(false);
    }

    public void ShowPlayerInfo()
    {
        if (player1 != null && player2 != null)
        {
            int player1Count = player1.OwnedProperties.Count;
            int player2Count = player2.OwnedProperties.Count;

            player1Text.text = $"{player1.Money}\n({player1Count})";
            player2Text.text = $"{player2.Money}\n({player2Count})";
        }
    }

    public void AddLog(string message)
    {
        if (logText == null) return;

        string newLog = $"{message}";
        logText.text = newLog + "\n" + logText.text;
    }
}