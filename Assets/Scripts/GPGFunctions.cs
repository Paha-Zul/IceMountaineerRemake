using GooglePlayGames;
using UnityEngine;

public class GPGFunctions {

    public static void start() {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

	public static bool incrementEvent(string id, uint amt) {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.Events.IncrementEvent(id, amt);
            return true;
        }
        return false;
    }

    public static void openLeaderboards() {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
            Social.ShowLeaderboardUI();
    }

    public static bool isAuthenticated() {
        return PlayGamesPlatform.Instance.IsAuthenticated();
        return false;
    }

    public static bool submitScore(int score, string boardID) {
        if (isAuthenticated()) {
            Social.ReportScore(score, boardID, (bool success) => {
                // handle success or failure
            });

            return true;
        }

        return false;
    }
}
