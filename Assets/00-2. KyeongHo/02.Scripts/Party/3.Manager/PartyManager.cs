using UnityEngine;
using Party.Domain;
using Party.Repository;
using System.Threading.Tasks;
using Firebase.Firestore;

namespace Party.Manager
{
    public class PartyManager : MonoBehaviour
    {
        public static PartyManager Instance { get; private set; }

        private PartyRepository _partyRepository;
        private Domain.Party _currentParty;
        private string _userUid;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(FirebaseFirestore firestore, string userUid)
        {
            _partyRepository = new PartyRepository(firestore);
            _userUid = userUid;
        }

        public async Task CreateParty()
        {
            if (_currentParty != null) return;

            var partyDto = new PartyDTO
            {
                LeaderUid = _userUid,
                Members = new System.Collections.Generic.List<string> { _userUid },
                MaxSize = 3,
                State = "Forming"
            };

            string partyId = await _partyRepository.CreatePartyAsync(partyDto);
            _currentParty = new Domain.Party(partyId, _userUid);
        }

        public async Task InviteFriend(string friendUid)
        {
            if (_currentParty == null || _currentParty.LeaderUid != _userUid) return;

            await _partyRepository.SendPartyInvitationAsync(friendUid, _currentParty.PartyId, _userUid);
        }

        public async Task AcceptInvitation(string partyId, string inviterUid)
        {
            if (_currentParty != null) return;

            PartyDTO partyDto = await _partyRepository.GetPartyAsync(partyId);
            if (partyDto == null) return;

            _currentParty = new Domain.Party(partyId, partyDto.LeaderUid, partyDto.MaxSize)
            {
                // Members = partyDto.Members,
                // State = partyDto.State
            };

            if (_currentParty.AddMember(_userUid))
            {
                partyDto.Members.Add(_userUid);
                await _partyRepository.UpdatePartyAsync(partyId, partyDto);
                await _partyRepository.RemovePartyInvitationAsync(_userUid, partyId);
            }
        }

        public async Task LeaveParty()
        {
            if (_currentParty == null) return;

            _currentParty.RemoveMember(_userUid);

            if (_currentParty.Members.Count == 0)
            {
                await _partyRepository.DeletePartyAsync(_currentParty.PartyId);
            }
            else
            {
                if (_currentParty.LeaderUid == _userUid)
                {
                    // _currentParty.LeaderUid = _currentParty.Members[0];
                }
                PartyDTO partyDto = await _partyRepository.GetPartyAsync(_currentParty.PartyId);
                partyDto.Members = _currentParty.Members;
                partyDto.LeaderUid = _currentParty.LeaderUid;
                await _partyRepository.UpdatePartyAsync(_currentParty.PartyId, partyDto);
            }
            _currentParty = null;
        }

        public async Task StartMatchmaking()
        {
            if (_currentParty == null || _currentParty.LeaderUid != _userUid) return;

            _currentParty.ChangeState("InQueue");
            PartyDTO partyDto = await _partyRepository.GetPartyAsync(_currentParty.PartyId);
            partyDto.State = "InQueue";
            await _partyRepository.UpdatePartyAsync(_currentParty.PartyId, partyDto);

            // TODO: Implement actual matchmaking logic here
        }
    }
}
