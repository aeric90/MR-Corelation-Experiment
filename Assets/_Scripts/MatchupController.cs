using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Matchup
{
    public int RoomA { get; }
    public int RoomB { get; }

    public Matchup(int roomA, int roomB)
    {
        RoomA = roomA;
        RoomB = roomB;
    }

    public override string ToString() => $"({RoomA}, {RoomB})";
}


public class MatchupController : MonoBehaviour
{
    public static MatchupController instance;

    public int roomCount = 16;
    public int matchUpReps = 3;

    private List<Matchup> matchupList = new List<Matchup>();
    private Matchup currentMatchup;

    private List<Matchup> demoMatchList = new List<Matchup>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        populateMatchUps();

        demoMatchList.Add(new Matchup(0, 15));
        demoMatchList.Add(new Matchup(4, 11));
        demoMatchList.Add(new Matchup(7, 8));
    }

    private void populateMatchUps()
    {
        var random = new System.Random();

        for (int x = 0; x < matchUpReps; x++)
        {
            for (int i = 0; i < roomCount; i++)
            {
                for (int j = i + 1; j < roomCount; j++)
                {
                    if (random.Next(0, 2) == 0)
                    {
                        matchupList.Add(new Matchup(i, j));
                    }
                    else
                    {
                        matchupList.Add(new Matchup(j, i));
                    }
                }
            }
        }
    }

    public void newMatchup()
    {
        var random = new System.Random();

        int matchupIndex = random.Next(matchupList.Count);
        currentMatchup = matchupList[matchupIndex];

        EloController.instance.setUpMatchUp(currentMatchup.RoomA, currentMatchup.RoomB);

        matchupList.Remove(matchupList[matchupIndex]);
    }

    public void newDemoMatchup()
    {
        currentMatchup = demoMatchList[0];

        demoMatchList.Remove(demoMatchList[0]);
    }

    public int getRoomA()
    {
        return currentMatchup.RoomA;
    }

    public int getRoomB()
    {
        return currentMatchup.RoomB;
    }

    public int getMatchupCount()
    {
        return matchupList.Count;
    }
}
