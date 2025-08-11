using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MatchAwardSystem : MonoBehaviour
{
    public static MatchAwardSystem instance;

    private void Awake()
    {
        instance = this;
    }

    public Dictionary<PlayerInput, int> killsCount = new Dictionary<PlayerInput, int>();
    public Dictionary<PlayerInput, int> keysCount = new Dictionary<PlayerInput, int>();
    public Dictionary<PlayerInput, int> healthLose = new Dictionary<PlayerInput, int>();
    public Dictionary<PlayerInput, int> minigameWinCount = new Dictionary<PlayerInput, int>();
    
    public enum MatchTitle
    {
        None,
        RealKiller,
        KeyMaster,
        Tanker,
        KingOfMinigame,
        Loser
    }
   

    public PlayerInput GetPlayerByMatchTitle(MatchTitle matchTitle)
    {
        switch (matchTitle)
        {
            case MatchTitle.RealKiller:
                return killsCount.Count > 0 ? killsCount.OrderByDescending(p => p.Value).FirstOrDefault().Key : null;
            case MatchTitle.KeyMaster:
                return keysCount.Count > 0 ? keysCount.OrderByDescending(p => p.Value).FirstOrDefault().Key : null;
            case MatchTitle.Tanker:
                return healthLose.Count > 0 ? healthLose.OrderByDescending(p => p.Value).FirstOrDefault().Key : null;
            case MatchTitle.KingOfMinigame:
                return minigameWinCount.Count > 0 ? minigameWinCount.OrderByDescending(p => p.Value).FirstOrDefault().Key : null;
            case MatchTitle.Loser:
                return minigameWinCount.Count > 0 ? minigameWinCount.OrderBy(p => p.Value).FirstOrDefault().Key : null;
        }

        return null;
    }

    public Dictionary<PlayerInput, MatchTitle> GetAllMatchTitles()
    {
        var result = new Dictionary<PlayerInput, MatchTitle>();
        var usedPlayers = new HashSet<PlayerInput>();

        // 1. RealKiller
        var killer = killsCount.OrderByDescending(p => p.Value)
                               .FirstOrDefault(p => !usedPlayers.Contains(p.Key));
        if (killer.Key != null)
        {
            result[killer.Key] = MatchTitle.RealKiller;
            usedPlayers.Add(killer.Key);
        }

        // 2. KeyMaster
        var keyMaster = keysCount.OrderByDescending(p => p.Value)
                                 .FirstOrDefault(p => !usedPlayers.Contains(p.Key));
        if (keyMaster.Key != null)
        {
            result[keyMaster.Key] = MatchTitle.KeyMaster;
            usedPlayers.Add(keyMaster.Key);
        }

        // 3. Tanker
        var tanker = healthLose.OrderByDescending(p => p.Value)
                               .FirstOrDefault(p => !usedPlayers.Contains(p.Key));
        if (tanker.Key != null)
        {
            result[tanker.Key] = MatchTitle.Tanker;
            usedPlayers.Add(tanker.Key);
        }

        // 4. KingOfMinigame
        var king = minigameWinCount.OrderByDescending(p => p.Value)
                                   .FirstOrDefault(p => !usedPlayers.Contains(p.Key));
        if (king.Key != null)
        {
            result[king.Key] = MatchTitle.KingOfMinigame;
            usedPlayers.Add(king.Key);
        }

        // 5. Loser
        var loser = minigameWinCount.OrderBy(p => p.Value)
                                    .FirstOrDefault(p => !usedPlayers.Contains(p.Key));
        if (loser.Key != null)
        {
            result[loser.Key] = MatchTitle.Loser;
            usedPlayers.Add(loser.Key);
        }

        return result;
    }

}
