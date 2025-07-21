using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class FriendManager : Singleton<FriendManager>
{
    private readonly FriendRepository _repository = new FriendRepository();

    public async Task SendFriendRequest(string senderUid, string recipientUid)
    {
        var requests = await _repository.GetFriendRequestsAsync(recipientUid);
        if (requests.Any(r => r.SenderUid == senderUid)) return;
        await _repository.AddFriendRequestAsync(recipientUid, new FriendRequestDto(senderUid));
    }

    public async Task AcceptFriendRequest(string userUid, string requesterUid)
    {
        var requests = await _repository.GetFriendRequestsAsync(userUid);
        if (!requests.Any(r => r.SenderUid == requesterUid)) return;

        var currentFriends = await _repository.GetFriendsAsync(userUid);
        if (!currentFriends.Contains(requesterUid))
        {
            await _repository.AddFriendAsync(userUid, requesterUid);
        }

        await _repository.RemoveFriendRequestAsync(userUid, new FriendRequestDto(requesterUid));
    }

    public async Task DeclineFriendRequest(string userUid, string requesterUid)
    {
        var requests = await _repository.GetFriendRequestsAsync(userUid);
        if (requests.Any(r => r.SenderUid == requesterUid))
        {
            await _repository.RemoveFriendRequestAsync(userUid, new FriendRequestDto(requesterUid));
        }
    }

    public async Task RemoveFriend(string userUid, string friendUid)
    {
        var currentFriends = await _repository.GetFriendsAsync(userUid);
        if (currentFriends.Contains(friendUid))
        {
            await _repository.RemoveFriendAsync(userUid, friendUid);
        }
    }

    public async Task<List<string>> GetFriendUids(string userUid)
    {
        return await _repository.GetFriendsAsync(userUid);
    }

    public async Task<List<FriendRequestDto>> GetFriendRequests(string userUid)
    {
        var dtos = await _repository.GetFriendRequestsAsync(userUid);
        return dtos.Select(dto => new FriendRequestDto(dto.SenderUid)).ToList();
    }
}
