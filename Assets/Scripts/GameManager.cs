using UnityEngine;
using System.Collections;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Board Path")]
    public GameObject[] Path;

    [Header("Dice")]
    [SerializeField] private Sprite[] diceNumber;
    [SerializeField] private SpriteRenderer diceNumberHolder;
    [SerializeField] private SpriteRenderer diceAnimation;
    [SerializeField] private int number;

    [Header("Players")]
    [SerializeField] private Player[] players;
    private int currentPlayerIndex = 0;

    public int CurrentPlayerIndex => currentPlayerIndex;

    [Header("Players")]
    [SerializeField] private int jailTileIndex = 18;

    public int JailTileIndex => jailTileIndex;


    private Coroutine diceRandomNumber;
    private bool canDiceRoll = true;
    private bool rolledSix = false;

    public bool RolledSix => rolledSix;
    public Player CurrentPlayer => players[currentPlayerIndex];

    public event System.Action<int> OnTurnChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    private void Start()
    {
        StartTurn(); // Start with Player 1
    }

    public void RollDice()
    {
        if (canDiceRoll)
            diceRandomNumber = StartCoroutine(DiceAnimation());
    }


    IEnumerator DiceAnimation()
    {
        yield return new WaitForEndOfFrame();
        
        canDiceRoll = false;
        diceNumberHolder.gameObject.SetActive(false);
        diceAnimation.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.8f);

        number = Random.Range(0, 6);
        diceNumberHolder.sprite = diceNumber[number];

        int diceValue = number + 1;

        diceNumberHolder.gameObject.SetActive(true);
        diceAnimation.gameObject.SetActive(false);

        rolledSix = (diceValue == 6);

        Player currentPlayer = players[currentPlayerIndex];
        StartCoroutine(HandlePlayerMove(currentPlayer, diceValue));
    }

    private IEnumerator HandlePlayerMove(Player player, int steps)
    {
        yield return player.MoveSteps(steps);


        if (rolledSix && !player.IsInJail)
        {
            Debug.Log("Rolled a 6! You get another turn.");
            //UIManager.Instance.AddLog($"{player.name} rolled a 6! Gets another turn.");
            canDiceRoll = true;
        }
        else
        {
            rolledSix = false; // reset
            //EndTurn();
        }
    }

    public void EnableDiceRoll()
    {
        canDiceRoll = true;
        rolledSix = false; // reset after giving extra roll
    }

    private void StartTurn()
    {
        Debug.Log($"[GameManager] Player {currentPlayerIndex + 1}'s turn!");

        Player currentPlayer = players[currentPlayerIndex];

        if (currentPlayer.IsInJail)
        {
            currentPlayer.DecreaseJailTurn();
            EndTurn();
            return;
        }

        OnTurnChanged?.Invoke(currentPlayerIndex);
        canDiceRoll = true;
    }

    public void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        StartTurn(); // Start the next player's turn
    }

    public Transform GetTileTransform(int index)
    {
        return Path[index].transform;
    }

    public Player[] GetPlayersOnTile(int tileIndex)
    {
        return players.Where(p => p.StepsMoved == tileIndex).ToArray();
    }

    public Player GetPlayerById(int id)
    {
        return players.FirstOrDefault(p => p.PlayerId == id);
    }
}