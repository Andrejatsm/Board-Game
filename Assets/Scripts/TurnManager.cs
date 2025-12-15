using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public PlayerController[] players;
    public Board board;
    public DiceRollScript dice;
    public GameCamera cam;

    private int currentPlayer = 0;
    private bool isBusy = false;

    public void RollDiceButton()
    {
        if (isBusy) return;
        StartCoroutine(TakeTurn());
    }

    IEnumerator TakeTurn()
    {
        if (players == null || players.Length == 0) yield break;

        isBusy = true;
        PlayerController player = players[currentPlayer];

        // 1. CAMERA: Focus on Dice
        if (cam != null) cam.FocusOnDice(dice.transform);

        // 2. Roll Dice
        dice.ResetDice();
        yield return new WaitForSeconds(0.2f);
        dice.Roll();

        // 3. Wait for landing
        yield return new WaitUntil(() => dice.isLanded);

        // 4. Get Number
        int steps = 0;
        int.TryParse(dice.diceFaceNum, out steps);

        // 5. CAMERA: Focus back on Player
        yield return new WaitForSeconds(0.5f);
        if (cam != null) cam.FocusOnPlayer(player.transform);

        // 6. Move Player (Walking)
        if (player != null)
        {
            yield return StartCoroutine(player.MoveSteps(board, steps));
        }

        // --- NEW: CHECK FOR SPECIAL TILE ---
        yield return new WaitForSeconds(0.2f); // Slight pause before effect happens
        yield return StartCoroutine(CheckTileEffect(player));

        // 7. Next Turn (Only if game isn't over)
        if (!isGameOver)
        {
            NextPlayer();
            isBusy = false;
        }
    }

    // Flag to stop turns if someone wins
    private bool isGameOver = false;

    IEnumerator CheckTileEffect(PlayerController player)
    {
        // Get the tile the player is currently standing on
        Transform tileTransform = board.GetTile(player.currentTileIndex);
        TileScript tileData = tileTransform.GetComponent<TileScript>();

        if (tileData != null)
        {
            switch (tileData.type)
            {
                case TileType.Win:
                    Debug.Log($"<color=green>PLAYER {currentPlayer} WINS!</color>");
                    isGameOver = true;
                    // TODO: Activate your Win Screen UI GameObject here
                    break;

                case TileType.Trap:
                    Debug.Log("Oh no! A Trap!");
                    // Calculate new position (Current - Amount)
                    int trapTarget = player.currentTileIndex - tileData.effectAmount;
                    yield return StartCoroutine(player.SlideToTile(board, trapTarget));
                    break;

                case TileType.Boost:
                    Debug.Log("Yay! A Boost!");
                    // Calculate new position (Current + Amount)
                    int boostTarget = player.currentTileIndex + tileData.effectAmount;
                    yield return StartCoroutine(player.SlideToTile(board, boostTarget));
                    break;

                case TileType.BackToStart:
                    Debug.Log("Ouch! Back to start!");
                    yield return StartCoroutine(player.SlideToTile(board, 0));
                    break;

                case TileType.Normal:
                case TileType.Start:
                default:
                    // Do nothing
                    break;
            }
        }
    }

    public void InitializePlayers(PlayerController[] allPlayers)
    {
        this.players = allPlayers;
        currentPlayer = 0;
        if (players.Length > 0 && cam != null) cam.FocusOnPlayer(players[0].transform);
    }

    void NextPlayer()
    {
        currentPlayer++;
        if (players != null && currentPlayer >= players.Length)
            currentPlayer = 0;

        if (players != null && players.Length > 0 && cam != null)
            cam.FocusOnPlayer(players[currentPlayer].transform);
    }
}