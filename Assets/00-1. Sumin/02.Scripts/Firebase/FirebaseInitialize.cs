using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using System.Threading.Tasks;

public static class FirebaseInitialize
{
    public static FirebaseApp App { get; private set; }
    public static FirebaseAuth Auth { get; private set; }
    public static FirebaseFirestore DB { get; private set; }

    public static bool IsInitialized { get; private set; } = false;

    private static TaskCompletionSource<bool> _initTcs = new TaskCompletionSource<bool>();

    static FirebaseInitialize()
    {
        InitAsync();
    }

    public static async void InitAsync()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("파이어베이스 연결에 성공했습니다.");

            App = FirebaseApp.DefaultInstance;
            Auth = FirebaseAuth.DefaultInstance;
            DB = FirebaseFirestore.DefaultInstance;

            IsInitialized = true;
            _initTcs.SetResult(true);
        }
        else
        {
            Debug.LogError($"파이어베이스 연결에 실패했습니다. : {dependencyStatus}");
            IsInitialized = false;
            _initTcs.SetException(new System.Exception("Firebase 초기화 실패"));
        }
    }

    // 다른 곳에서 초기화 완료를 기다릴 때 호출
    public static Task WaitForInitializationAsync()
    {
        return _initTcs.Task;
    }
}
