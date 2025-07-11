using System;
using System.Collections.Generic;
using UnityEngine;


public class DummyLoginManager : MonoBehaviour
{
    private List<DummyAccount> dummyAccounts;

    private async void Start()
    {
        LoadDummyAccounts();

        foreach (var account in dummyAccounts)
        {
            bool isOnline = await FirebaseService.CheckIfOnline(account.userId);
            if (!isOnline)
            {
                Debug.Log($"사용 가능한 더미 계정 발견: {account.userId}");
                await LoginAsDummy(account);
                return;
            }
        }

        Debug.LogError("사용 가능한 더미 계정이 없습니다!");
    }

    private void LoadDummyAccounts()
    {
        string json = PlayerPrefs.GetString("DummyAccounts"); // 또는 Resources에서 json 읽기
        dummyAccounts = JsonUtility.FromJson<DummyAccountList>(json).accounts;
    }

    private async Task LoginAsDummy(DummyAccount account)
    {
        await FirebaseAuthManager.LoginWithUserId(account.userId);
        AccountManager.Instance.SetAccount(account);
        // 이후 Photon 연결 시작
    }

    [Serializable]
    private class DummyAccountList
    {
        public List<DummyAccount> accounts;
    }
}