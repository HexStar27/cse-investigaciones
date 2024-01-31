using Hexstar.Dialogue;
using UnityEngine;


/// <summary>
/// This works as a connection between the main gameplay script and the Dialogue Controller (DC).
/// It will mark the tutorial state for the DC to have in mind where to branch inside the tutorial dialogue.
/// </summary>
public static class TutorialChecker
{
    public enum WinCondition { WIN, LOST, SURR}
    public static void SetWinCondition(WinCondition condition)
    {
        string val = condition switch
        {
            WinCondition.WIN => "tuto_win",
            WinCondition.LOST => "tuto_lose",
            WinCondition.SURR => "tuto_meh",
            _ => "tuto_fin",
        };
        ControladorDialogos.SetDialogueEvent("win_condition", val);
    }
}
