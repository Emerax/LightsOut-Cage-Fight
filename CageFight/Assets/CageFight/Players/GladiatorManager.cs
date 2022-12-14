using Photon.Realtime;
using System;
/// <summary>
/// Represents one player of the game.
/// </summary>
public class GladiatorManager {
    public bool IsMe { get => player.IsLocal; }
    public int Team { get => player.ActorNumber; }

    public int Score {
        get => (int)player.CustomProperties[GameLogic.SCORE_KEY];
    }

    public int Money {
        get => money;
        set {
            money = value;
            MoneyChangeAction?.Invoke(money);
        }
    }

    public Action<int> MoneyChangeAction;

    private readonly Player player;
    private int money;

    public GladiatorManager(Player player, int startingMoney) {
        this.player = player;
        Money = startingMoney;
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameLogic.SCORE_KEY, 0 } });
    }

    public void AddMoney(int value) {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameLogic.SCORE_KEY, Score + value } });
        Money += value;
    }

    public void RemoveMoney(int cost) {
        Money -= cost;
    }

    public void ToggleReady(bool ready) {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameLogic.READY_KEY, ready } });
    }

    public void ResetScore() {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { GameLogic.SCORE_KEY, 0 } });
    }
}
