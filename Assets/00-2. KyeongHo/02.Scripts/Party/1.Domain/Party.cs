using System.Collections.Generic;

public class Party
{
    public string PartyId { get; private set; }
    public string LeaderUid { get; private set; }
    public List<string> Members { get; private set; }
    public int MaxSize { get; private set; }
    public string State { get; private set; }

    public Party(string partyId, string leaderUid, int maxSize = 3)
    {
        PartyId = partyId;
        LeaderUid = leaderUid;
        Members = new List<string> { leaderUid };
        MaxSize = maxSize;
        State = "Forming";
    }

    public bool AddMember(string uid)
    {
        if (Members.Count >= MaxSize || Members.Contains(uid))
        {
            return false;
        }
        Members.Add(uid);
        return true;
    }

    public void RemoveMember(string uid)
    {
        Members.Remove(uid);
    }

    public void ChangeState(string newState)
    {
        State = newState;
    }
}