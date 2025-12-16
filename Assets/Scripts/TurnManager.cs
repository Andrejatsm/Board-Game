using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Needed for Button control

public class TurnManager : MonoBehaviour
{
    public PlayerController[] players;
    public Board board;
    public DiceRollScript dice;
    public GameCamera cam;

    // NEW: Drag your UI "Roll Button" here so we can hide it during bot turns
    public Button rollButton;

    private int currentPlayer = 0;
    private bool isBusy = false;
    private bool isGameOver = false;

    // Called by GameSetupManager when the scene starts
    public void InitializePlayers(PlayerController[] allPlayers)
    {
        this.players = allPlayers;
        currentPlayer = 0;

        if (players.Length > 0 && cam != null)
        {
            cam.FocusOnPlayer(players[0].transform);
        }

        // Check who goes first (Human or Bot?)
        CheckTurnType();
    }

    public void RollDiceButton()
    {
        if (isBusy) return;
        StartCoroutine(TakeTurn());
    }

    void CheckTurnType()
    {
        // ASSUMPTION: Player 0 is ALWAYS the Human. Players 1+ are Bots.
        if (currentPlayer == 0)
        {
            // Human Turn: Show Button, Wait for Click
            if (rollButton != null) rollButton.interactable = true;
            Debug.Log("Human Turn: Waiting for input...");
        }
        else
        {
            // Bot Turn: Hide Button, Auto-Roll after delay
            if (rollButton != null) rollButton.interactable = false;
            Debug.Log($"Bot Turn (Player {currentPlayer}): Rolling automatically...");
            StartCoroutine(BotRollDelay());
        }
    }

    IEnumerator BotRollDelay()
    {
        yield return new WaitForSeconds(1.5f); // Wait a bit so camera can focus
        if (!isGameOver) RollDiceButton();     // "Click" the button via code
    }

    IEnumerator TakeTurn()
    {
        if (players == null || players.Length == 0) yield break;
        isBusy = true;

        // Disable button immediately so you can't double-click
        if (rollButton != null) rollButton.interactable = false;

        PlayerController player = players[currentPlayer];

        // 1. CAMERA: Focus on Dice
        if (cam != null) cam.FocusOnDice(dice.transform);

        // 2. Roll Dice
        dice.ResetDice();
        yield return new WaitForSeconds(0.2f);
        dice.Roll();
        yield return new WaitUntil(() => dice.isLanded);

        // 3. Get Number
        int steps = 0;
        int.TryParse(dice.diceFaceNum, out steps);

        // 4. CAMERA: Focus back on Player
        yield return new WaitForSeconds(0.5f);
        if (cam != null) cam.FocusOnPlayer(player.transform);

        // 5. Move Player
        if (player != null)
        {
            yield return StartCoroutine(player.MoveSteps(board, steps));
        }

        // 6. Special Tile Effects
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(CheckTileEffect(player));

        // 7. Next Turn
        if (!isGameOver)
        {
            NextPlayer();
            isBusy = false;
        }
    }

    void NextPlayer()
    {
        currentPlayer++;
        if (players != null && currentPlayer >= players.Length)
            currentPlayer = 0;

        if (players != null && players.Length > 0 && cam != null)
            cam.FocusOnPlayer(players[currentPlayer].transform);

        // Decide if we show button or auto-roll
        CheckTurnType();
    }

    // ... (Keep your existing CheckTileEffect function here) ...
    // Copy the CheckTileEffect function from your previous script to here.
    public WinScreenScript winScreen;

    IEnumerator CheckTileEffect(PlayerController player)
    {
        Transform tileTransform = board.GetTile(player.currentTileIndex);
        TileScript tileData = tileTransform.GetComponent<TileScript>();

        if (tileData != null)
        {
            switch (tileData.type)
            {
                case TileType.Win:
                    Debug.Log($"<color=green>PLAYER WINS!</color>");
                    isGameOver = true;

                    // GET THE NAME
                    string winnerName = player.GetComponent<NameScript>().playerName;
                    // (Assuming your NameScript has a public string playerName variable)

                    // SHOW WIN SCREEN
                    if (winScreen != null) winScreen.ShowWin(winnerName);
                    break;

                case TileType.Trap:
                    int trapTarget = player.currentTileIndex - tileData.effectAmount;
                    yield return StartCoroutine(player.SlideToTile(board, trapTarget));
                    break;

                case TileType.Boost:
                    int boostTarget = player.currentTileIndex + tileData.effectAmount;
                    yield return StartCoroutine(player.SlideToTile(board, boostTarget));
                    break;

                case TileType.BackToStart:
                    yield return StartCoroutine(player.DeathAndRespawn(board));
                    break;
            }
        }
    }
}