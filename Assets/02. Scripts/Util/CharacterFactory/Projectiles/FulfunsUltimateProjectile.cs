using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Pun;
using UnityEngine;

public class FulfunsUltimateProjectile : MonoBehaviour, IProjectile
{
    private CharacterBase _owner;
    private SkillData _data;

    private float _maxRange = 0f;

    private Vector3 _direction;
    private bool _isInitialized = false;
    private bool _hasExploded = false;

    // Bezier
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _controlPoint1;
    private Vector3 _controlPoint2;
    private float _bezierT = 0f; // 베지어 곡선 상의 현재 위치 (0 ~ 1)
    public float bezierSpeedMultiplier = 0.1f; // 베지어 곡선 속도 조절

    private float _elapsedTime = 0f;
    public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;
        // 전체 이동 시간 계산
        float totalTime = _maxRange / (_data.Speed * bezierSpeedMultiplier);

        // 경과 시간 → 0~1 구간의 t
        float normalizedTime = Mathf.Clamp01(_elapsedTime / totalTime);

        // 가속 곡선에 따라 t 보정
        _bezierT = speedCurve.Evaluate(normalizedTime);

        if (_bezierT >= 1f)
        {
            _bezierT = 1f;
            SpawnFulfunsObject();
            _hasExploded = true;
            return;
        }

        Vector3 newPosition = Bezier.GetPoint(_startPosition, _controlPoint1, _controlPoint2, _endPosition, _bezierT);
        transform.position = newPosition;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!_isInitialized)
        {
            return;
        }
        Debug.Log($"collided with {collision.gameObject}");

        // 자기 자신 처리
        if (_owner != null && collision.gameObject == _owner.Behaviour.gameObject)
        {
            return;
        }


        if (collision.CompareTag("FulfunsField"))
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f; // 땅에 붙게 Y 고정

            PhotonNetwork.Instantiate("FulfunsUltimateObject", spawnPos, Quaternion.identity);
            _hasExploded = true;
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            // 플레이어 밀쳐내기 구현 필요
        }

        _hasExploded = true;
    }

    private void SpawnFulfunsObject()
    {
        if (_hasExploded) return;
        GameObject slime = PhotonNetwork.Instantiate("FulfunsUltimateObject", transform.position, Quaternion.identity);
        //FulfunsSpawnObject slimeObject = slime.GetComponent<FulfunsSpawnObject>();
        //slimeObject - 아직 아무것도 안함
        PhotonNetwork.Destroy(gameObject);
    }

    public void SetData(SkillData data, CharacterBase character, Vector3? direction, CharacterBase target = null)
    {
        _owner = character;
        _data = data;
        _maxRange = data.MaxRange;
        _direction = direction ?? Vector3.forward;

        // 베지어 곡선 초기화
        _startPosition = transform.position;
        _endPosition = _startPosition + _direction.normalized * _maxRange; // 목표 위치 계산
        _endPosition.y = -1f;

        // 제어점 설정 (예시: 시작점과 끝점 사이에 적절히 배치)
        _controlPoint1 = _startPosition + _direction.normalized * (_maxRange * 0.3f) + Vector3.up * 2f;
        _controlPoint2 = _endPosition - _direction.normalized * (_maxRange * 0.3f) + Vector3.up * 1f;

        _bezierT = 0f; // 베지어 곡선 시작 위치 초기화

        _isInitialized = true;
    }
}
