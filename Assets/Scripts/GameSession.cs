using UnityEngine;

public static class GameSession
{
    private const string PlayerNameKey = "player_name";
    private const string BirdNameKey = "bird_name";

    public static string PlayerName =>
        PlayerPrefs.GetString(PlayerNameKey, "同學");

    public static string BirdName =>
        PlayerPrefs.GetString(BirdNameKey, "鳥鳥");

    public static void SetPlayerName(string value)
    {
        string trimmedValue = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmedValue))
        {
            trimmedValue = "同學";
        }

        PlayerPrefs.SetString(PlayerNameKey, trimmedValue);
        PlayerPrefs.Save();
    }

    public static void SetBirdName(string value)
    {
        string trimmedValue = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmedValue))
        {
            trimmedValue = "鳥鳥";
        }

        PlayerPrefs.SetString(BirdNameKey, trimmedValue);
        PlayerPrefs.Save();
    }

    public static string FormatDialogue(string template)
    {
        return template
            .Replace("{player}", PlayerName)
            .Replace("{bird}", BirdName);
    }
}
