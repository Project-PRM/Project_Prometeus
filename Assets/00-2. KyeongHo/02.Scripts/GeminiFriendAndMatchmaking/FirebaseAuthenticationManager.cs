
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

// Firebase 인증(로그인, 회원가입)을 관리하는 싱글톤 클래스입니다.
public class FirebaseAuthenticationManager : MonoBehaviour
{
    public static FirebaseAuthenticationManager Instance { get; private set; }

    private FirebaseAuth auth;
    public FirebaseUser CurrentUser { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Firebase 초기화를 담당합니다.
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase 인증 시스템이 성공적으로 초기화되었습니다.");
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
            }
            else
            {
                Debug.LogError($"Firebase 종속성 확인 실패: {dependencyStatus}");
            }
        });
    }

    // 사용자의 로그인 상태 변경을 감지합니다.
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != CurrentUser)
        {
            bool signedIn = auth.CurrentUser != null;
            if (!signedIn && CurrentUser != null)
            {
                Debug.Log("사용자가 로그아웃했습니다.");
            }
            CurrentUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"사용자 로그인: {CurrentUser.DisplayName} ({CurrentUser.UserId})");
            }
        }
    }

    // 이메일과 비밀번호로 회원가입을 시도합니다.
    public async Task<FirebaseUser> RegisterAsync(string email, string password, string displayName)
    {
        try
        {
            FirebaseUser user = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            if (user != null)
            {
                UserProfile profile = new UserProfile { DisplayName = displayName };
                await user.UpdateUserProfileAsync(profile);
                Debug.Log($"회원가입 및 프로필 업데이트 성공: {user.DisplayName}");
                return user;
            }
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"회원가입 실패: {e.Message}");
        }
        return null;
    }

    // 이메일과 비밀번호로 로그인을 시도합니다.
    public async Task<FirebaseUser> LoginAsync(string email, string password)
    {
        try
        {
            FirebaseUser user = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"로그인 성공: {user.DisplayName}");
            return user;
        }
        catch (FirebaseException e)
        {
            Debug.LogError($"로그인 실패: {e.Message}");
        }
        return null;
    }

    // 로그아웃을 수행합니다.
    public void Logout()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }
}
