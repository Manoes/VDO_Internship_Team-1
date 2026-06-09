using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoresUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField] private string[] placeholderTexts =
    {
        "Aaron Swartz",
        "Kevin Poulsen",
        "MafiaBoy",
        "Jean James Ancheta",
        "Matthew Bevan",
        "Richard Pryce",
        "Albert Gonzalez",
        "Adrian Lamo",
        "Gary McKinnon",
        "Julian Assange",
        "Kevin Mitnick"
    };

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        HighScoreSystem.Reload();

        IReadOnlyList<HighScoreEntry> entries = HighScoreSystem.HighScoreService.GetTop();

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if(scoreTexts[i] == null) continue; 

            if(i < entries.Count)
                scoreTexts[i].text = FormatEntry(entries[i]);
            else
                scoreTexts[i].text = FormatPlaceHolder(i);
        }
    }

    public string FormatEntry(HighScoreEntry entry)
    {
        return $"{entry.name} - {entry.score:D8} - LVL{entry.maxLevelCompleted}";
    }

    private string FormatPlaceHolder(int index)
    {
        string name = index < placeholderTexts.Length ? placeholderTexts[index] : "Unkown Hacker";

        return $"{name} - 00000000 - LVL0";
    }
}
