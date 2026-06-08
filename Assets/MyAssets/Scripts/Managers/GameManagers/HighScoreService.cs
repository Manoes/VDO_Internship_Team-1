using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class HighScoreEntry
{
    public string name;
    public int score;
    public int maxLevelCompleted;
}

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> entries = new();
}

public class HighScoreService
{
    const int MaxEntries = 5;
    readonly string filePath;

    public HighScoreData Data { get; private set; } = new HighScoreData();

    public HighScoreService(string fileName = "highscores.json")
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Load();
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Data = new HighScoreData();
                Save();
                return;
            }

            string json = File.ReadAllText(filePath);
            Data = JsonUtility.FromJson<HighScoreData>(json) ?? new HighScoreData();
            Data.entries ??= new List<HighScoreEntry>();

            SortAndTrim();
        }
        catch
        {
            // Corrupt File -> Reset
            Data = new HighScoreData();
            Save();
        }
    }

    public void Save()
    {
        SortAndTrim();
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(filePath, json);
    }

    public IReadOnlyList<HighScoreEntry> GetTop() => Data.entries;

    public bool IsHighScore(int score, int maxLevelCompleted)
    {
        if (Data.entries.Count < MaxEntries) return true;

        HighScoreEntry worst = Data.entries[^1];

        if (score > worst.score) return true;
        if (score == worst.score && maxLevelCompleted > worst.maxLevelCompleted) return true;

        return false;
    }

    public void AddHighScore(string name, int score, int maxLevelCompleted)
    {
        if(!IsHighScore(score, maxLevelCompleted))
            return;        

        Data.entries.Add(new HighScoreEntry
        {
            name = SanitizeName(name),
            score = score,
            maxLevelCompleted = maxLevelCompleted
        });

        SortAndTrim();
        Save();
    }

    void SortAndTrim()
    {
        Data.entries = Data.entries
             .OrderByDescending(e => e.score)
             .ThenByDescending(e => e.maxLevelCompleted)
             .Take(MaxEntries)
             .ToList();
    }

    static string SanitizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Unknown";

        name = new string(
            name.Trim()
                .Where(c => !char.IsControl(c))
                .ToArray()
        );

        return string.IsNullOrWhiteSpace(name)
            ? "Unknown"
            : name;
    }
}