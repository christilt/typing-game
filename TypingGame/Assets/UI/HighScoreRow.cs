using System.Collections.Generic;
using UnityEngine;

public class HighScoreRow : MonoBehaviour
{
    [SerializeField] private HighScoreRowOld _oldRow;
    [SerializeField] private HighScoreRowNew _newRow;

    public void Enable(bool isNewRow, int index, List<HighScore> highScores, string whiteColourString, string lastPlayerInitials)
    {
        if (isNewRow)
        {
            _newRow.TextInterpolator.Enable(index, highScores, whiteColourString);
            _newRow.InitialsInput.text = lastPlayerInitials;
        }
        else
        {
            _oldRow.TextInterpolator.Enable(index, highScores, whiteColourString);
        }

        gameObject.SetActive(true);
    }
}