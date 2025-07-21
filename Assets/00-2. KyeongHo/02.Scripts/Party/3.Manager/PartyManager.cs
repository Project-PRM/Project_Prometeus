using UnityEngine;
using System.Threading.Tasks;
using Firebase.Firestore;

public class PartyManager : Singleton<PartyManager>
{
    private PartyRepository _partyRepository;
    private PartyDTO _currentPartyDto;
    private string _currentPartyId;
    private string _userUid;

    public void Initialize(FirebaseFirestore firestore, string userUid)
    {
        _partyRepository = new PartyRepository(firestore);
        _userUid = userUid;
    }

    public async Task CreateParty()
    {
        if (_currentPartyDto != null) return;

        _currentPartyDto = new PartyDTO
        {
            LeaderUid = _userUid,
            Members = new System.Collections.Generic.List<string> { _userUid },
            MaxSize = 3,
            State = "Forming"
        };

        _currentPartyId = await _partyRepository.CreatePartyAsync(_currentPartyDto);
    }

    public async Task InviteFriend(string friendUid)
    {
        if (_currentPartyDto == null || _currentPartyDto.LeaderUid != _userUid) return;

        await _partyRepository.SendPartyInvitationAsync(friendUid, _currentPartyId, _userUid);
    }

    public async Task AcceptInvitation(string partyId, string inviterUid)
    {
        if (_currentPartyDto != null) return;

        _currentPartyDto = await _partyRepository.GetPartyAsync(partyId);
        if (_currentPartyDto == null || _currentPartyDto.Members.Count >= _currentPartyDto.MaxSize) 
        {
            _currentPartyDto = null;
            return;
        }

        _currentPartyId = partyId;
        _currentPartyDto.Members.Add(_userUid);
        await _partyRepository.UpdatePartyAsync(_currentPartyId, _currentPartyDto);
        await _partyRepository.RemovePartyInvitationAsync(_userUid, partyId);
    }

    public async Task LeaveParty()
    {
        if (_currentPartyDto == null) return;

        _currentPartyDto.Members.Remove(_userUid);

        if (_currentPartyDto.Members.Count == 0)
        {
            await _partyRepository.DeletePartyAsync(_currentPartyId);
        }
        else
        {
            if (_currentPartyDto.LeaderUid == _userUid)
            {
                _currentPartyDto.LeaderUid = _currentPartyDto.Members[0];
            }
            await _partyRepository.UpdatePartyAsync(_currentPartyId, _currentPartyDto);
        }
        _currentPartyDto = null;
        _currentPartyId = null;
    }

    public async Task StartMatchmaking()
    {
        if (_currentPartyDto == null || _currentPartyDto.LeaderUid != _userUid) return;

        _currentPartyDto.State = "InQueue";
        await _partyRepository.UpdatePartyAsync(_currentPartyId, _currentPartyDto);

        // TODO: Implement actual matchmaking logic here
    }
}

