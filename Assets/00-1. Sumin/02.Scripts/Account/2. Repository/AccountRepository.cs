using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AccountRepository
{
    private FirebaseFirestore _db => FirebaseInitialize.DB;
    private FirebaseAuth _auth => FirebaseInitialize.Auth;

    private Account _myAccount;
    public const string COLLECTION_NAME = "UserAccount";    
    public const string NICKNAME = "Nickname";    
    public const string EMAIL = "Email";
    
    public AccountDTO MyAccount
    {
        get
        {
            if (_myAccount == null)
            {
                throw new InvalidOperationException("현재 로그인된 계정이 없습니다.");
            }
            return _myAccount.ToDto();
        }
    }

    #region Register
    // 닉네임 중복 체크
    private async Task<bool> IsNicknameTakenAsync(string nickname)
    {
        var collection = _db.Collection(COLLECTION_NAME);
        var query = collection.WhereEqualTo(NICKNAME, nickname);
        var querySnapshot = await query.GetSnapshotAsync();
        return querySnapshot.Count > 0; // 동일한 닉네임이 있으면 true
    }

    // 회원 가입 시도
    public async Task<AccountResult> RegisterAsync(string email, string nickname, string password)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        try
        {
            // 닉네임 중복 체크
            if (await IsNicknameTakenAsync(nickname))
            {
                return new AccountResult(false, "이미 사용중인 닉네임입니다.");
            }

            var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);

            // DisplayName 설정
            await SetInitialNicknameAsync(result.User, nickname);

            // UserAccount 컬렉션에 UID를 문서 ID로 하여 등록
            var docRef = _db.Collection(COLLECTION_NAME).Document(result.User.UserId);
            await docRef.SetAsync(new { 
                Email = result.User.Email, 
                Nickname = nickname 
            });

            Debug.Log($"회원가입 성공 : {result.User.UserId}");
            return new AccountResult(true, $"회원가입 성공 : {nickname}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 실패: {ex.Message}");
            return new AccountResult(false, ex.Message);
        }
    }

    /// <summary>
    /// 회원가입 시 최초 1회 서버의 DisplayName변경용
    /// </summary>
    private async Task SetInitialNicknameAsync(FirebaseUser user, string nickname)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        if (user == null) return;

        var profile = new UserProfile { DisplayName = nickname };
        await user.UpdateUserProfileAsync(profile);
    }
    #endregion

    #region 로그인/로그아웃
    /// <summary>
    /// 로그인
    /// </summary>
    public async Task<AccountResult> LoginAsync(string email, string password)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        try
        {
            var result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
            SetMyAccount(result.User);
            Debug.Log($"로그인 성공 : {result.User.DisplayName} ({result.User.UserId})");
            return new AccountResult(true, $"로그인 성공 : {result.User.DisplayName} ({result.User.UserId})");
        }
        catch (Exception ex)
        {
            Debug.LogError($"로그인 실패: {ex.Message}");
            return new AccountResult(false, ex.Message);
        }
    }

    /// <summary>
    /// 동기로 로그아웃이 더 안정적일 듯 함
    /// </summary>
    public void Logout()
    {
        if (_auth.CurrentUser == null)
        {
            Debug.LogWarning("이미 로그아웃된 상태입니다.");
            return;
        }

        _auth.SignOut();
        SetMyAccount(null);

        Debug.Log("로그아웃 되었습니다.");
    }
    #endregion

    #region Nickname
    /// <summary>
    /// 로그인 후, 자신의 닉네임 변경 용
    /// </summary>
    public async Task<AccountResult> ChangeMyNicknameAsync(string newNickname)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var user = _auth.CurrentUser;

        if (user == null)
        {
            return new AccountResult(false, "로그인된 유저가 없습니다.");
        }

        if (user.Email != _myAccount.Email)
        {
            return new AccountResult(false, "유저 정보가 다릅니다");
        }

        // 닉네임 중복 체크
        if (await IsNicknameTakenAsync(newNickname))
        {
            return new AccountResult(false, "이미 사용중인 닉네임입니다.");
        }

        var profile = new UserProfile { DisplayName = newNickname };
        
        try
        {
            // 1. Firebase Auth의 DisplayName 업데이트
            await user.UpdateUserProfileAsync(profile);

            // 2. 내부 캐싱된 Account 객체 닉네임 업데이트
            _myAccount.SetNickname(newNickname, out string _);

            // 3. Firestore에서 사용자 문서의 닉네임 업데이트
            var docRef = _db.Collection(COLLECTION_NAME).Document(user.UserId);
            await docRef.UpdateAsync(NICKNAME, newNickname);

            return new AccountResult(true, $"닉네임이 '{newNickname}'(으)로 변경되었습니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError("닉네임 변경 실패: " + ex.Message);
            return new AccountResult(false, ex.Message);
        }
    }
    #endregion

    /// <summary>
    /// 내부적으로 가지고 있는 Account 객체 유지용
    /// </summary>
    private void SetMyAccount(FirebaseUser user)
    {
        if (user != null)
        {
            Account.TryCreate(user.UserId, user.Email, user.DisplayName, out _myAccount, out string message);
        }
        else
        {
            _myAccount = null;
        }
    }

    public async Task<string> GetUserNicknameWithEmailAsync(string email)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var collection = _db.Collection(COLLECTION_NAME);
        var query = collection.WhereEqualTo(EMAIL, email);
        var querySnapshot = await query.GetSnapshotAsync();

        foreach (var doc in querySnapshot.Documents)
        {
            if (doc.TryGetValue(NICKNAME, out string nickname))
            {
                return nickname;
            }
        }

        return null; // 해당 이메일로 등록된 닉네임 없음
    }

    public async Task<string> GetUidWithNicknameAsync(string nickname)
    {
        await FirebaseInitialize.WaitForInitializationAsync();
        
        var collection = _db.Collection(COLLECTION_NAME);
        var query = collection.WhereEqualTo(NICKNAME, nickname);
        var querySnapshot = await query.GetSnapshotAsync();

        foreach (var doc in querySnapshot.Documents)
        {
            return doc.Id; // 문서 ID가 UID
        }

        return null; // 닉네임이 존재하지 않음
    }
    public async Task<List<string>> GetUidsWithNicknameAsync(string nickname)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var collection = _db.Collection(COLLECTION_NAME);
        var query = collection.WhereEqualTo(NICKNAME, nickname);
        var snapshot = await query.GetSnapshotAsync();

        List<string> result = new();

        foreach (var doc in snapshot.Documents)
        {
            result.Add(doc.Id); // 문서 ID가 UID
        }

        return result;
    }

    public async Task<List<(string nickname, string uid)>> GetUidsByNicknameAsync(string nickname)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var collection = _db.Collection(COLLECTION_NAME);
        var query = collection.WhereEqualTo(NICKNAME, nickname);
        var snapshot = await query.GetSnapshotAsync();

        List<(string nickname, string uid)> result = new();

        foreach (var doc in snapshot.Documents)
        {
            if (doc.TryGetValue(NICKNAME, out string docNickname))
            {
                result.Add((docNickname, doc.Id)); // 문서 ID가 UID
            }
        }

        return result;
    }

    public async Task<string> GetUserNicknameWithUidAsync(string uid)
    {
        await FirebaseInitialize.WaitForInitializationAsync();

        var docRef = _db.Collection(COLLECTION_NAME).Document(uid);
        var docSnapshot = await docRef.GetSnapshotAsync();

        if (docSnapshot.Exists && docSnapshot.TryGetValue(NICKNAME, out string nickname))
        {
            return nickname;
        }

        return null; // 해당 UID로 등록된 닉네임 없음
    }
}