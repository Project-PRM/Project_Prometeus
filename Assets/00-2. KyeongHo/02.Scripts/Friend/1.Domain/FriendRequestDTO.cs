using System;
using System.Collections.Generic;
using Firebase.Firestore;

[Serializable] 
public class FriendRequestDto
{
    public string SenderUid { get; private set; }

    public FriendRequestDto(string senderUid)
    {
        SenderUid = senderUid;
    }
}
