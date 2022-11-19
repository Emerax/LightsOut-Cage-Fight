using System.Collections.Generic;
using System.Linq;

public class MonsterList {

    private static readonly MonsterList instance = new();

    public static MonsterList Instance {
        get {
            return instance;
        }
    }

    private readonly List<MonsterData> monsters;

    private MonsterList() {
        monsters = new List<MonsterData>();
    }

    public void AddMonster(MonsterData monster) {
        monsters.Add(monster);
    }

    public void RemoveMonster(MonsterData monster) {
        monsters.Remove(monster);
    }


    public IEnumerable<MonsterData> GetMonstersAll() {
        return monsters;
    }

    public IEnumerable<MonsterData> GetMonstersOfTeam(int team) {
        return monsters.Where(monster => { return monster.Team == team; });
    }

    public IEnumerable<MonsterData> GetMonstersOfOtherTeams(int team) {
        return monsters.Where(monster => { return monster.Team != team; });
    }
}
