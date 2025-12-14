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

    void Start()
    {
        if (players != null && players.Length > 0 && cam != null)
            cam.SetTarget(players[currentPlayer].transform);
    }

    public void RollDiceButton()
    {
        if (isBusy) return;
        StartCoroutine(TakeTurn());
    }

    IEnumerator TakeTurn()
    {
        if (players == null || players.Length == 0 || dice == null || cam == null)
        {
            Debug.LogWarning("TurnManager missing references (players, dice or cam). Assign them in the Inspector.");
            yield break;
        }

        isBusy = true;

        PlayerController player = players[currentPlayer];

        cam.FocusOnPlayer(player.transform);

        dice.Roll();

        // wait until dice lands (set by DiceRollScript / SideDetectScript)
        yield return new WaitUntil(() => dice.isLanded);

        int roll = dice.rolledValue;

        // Player.MoveSteps expected to be a coroutine returning IEnumerator
        if (player != null)
            yield return StartCoroutine(player.MoveSteps(board, roll));

        yield return new WaitForSeconds(0.5f);

        NextPlayer();
        isBusy = false;
    }

    void NextPlayer()
    {
        currentPlayer++;
        if (players != null && currentPlayer >= players.Length)
            currentPlayer = 0;

        if (players != null && players.Length > 0 && cam != null)
            cam.SetTarget(players[currentPlayer].transform);
    }
}
