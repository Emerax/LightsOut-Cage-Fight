using System.Collections.Generic;
using System.Linq;

public class MonsterList {

    private static readonly MonsterList instance = new();

    public static MonsterList Instance {
        get {
            return instance;
        }
    }

    private readonly List<MonsterBehaviour> monsters;

    private MonsterList() {
        monsters = new List<MonsterBehaviour>();
    }

    public void AddMonster(MonsterBehaviour monster) {
        monsters.Add(monster);
    }

    public void RemoveMonster(MonsterBehaviour monster) {
        monsters.Remove(monster);
    }


    public IEnumerable<MonsterBehaviour> GetMonstersAll() {
        return monsters;
    }

    public IEnumerable<MonsterBehaviour> GetMonstersOfTeam(int team) {
        return monsters.Where(monster => { return monster.Data.Team == team; });
    }

    public IEnumerable<MonsterBehaviour> GetMonstersOfOtherTeams(int team) {
        return monsters.Where(monster => { return monster.Data.Team != team; });
    }
}
