using System;

public class FriendRequest
{
    public string SenderUid { get; }

    public FriendRequest(string senderUid)
    {
        if (string.IsNullOrWhiteSpace(senderUid))
        {
            throw new Exception("Sender UID cannot be null or whitespace.");
        }
        SenderUid = senderUid;
    }
}