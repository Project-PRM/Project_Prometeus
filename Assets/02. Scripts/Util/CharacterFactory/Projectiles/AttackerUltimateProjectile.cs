using Photon.Pun;
using Photon.Pun.Demo.SlotRacer.Utils;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UltimateProjectile : MonoBehaviour, IProjectile
{
    private CharacterBase _owner;

    private float _speed = 0f;
    private float _damage = 0f;
    private float _maxRange = 0f;
    private float _radius = 0f;
    private float _duration = 3f;

    private Vector3 _direction;
    private float _traveledDistance = 0f;
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
        float totalTime = _maxRange / (_speed * bezierSpeedMultiplier);

        // 경과 시간 → 0~1 구간의 t
        float normalizedTime = Mathf.Clamp01(_elapsedTime / totalTime);

        // 가속 곡선에 따라 t 보정
        _bezierT = speedCurve.Evaluate(normalizedTime);

        if (_bezierT >= 1f)
        {
            _bezierT = 1f;
            SpawnAoEField();
            _hasExploded = true;
            return;
        }

        Vector3 newPosition = Bezier.GetPoint(_startPosition, _controlPoint1, _controlPoint2, _endPosition, _bezierT);
        transform.position = newPosition;

        _traveledDistance = Vector3.Distance(_startPosition, transform.position);

        DrawBezierDebugLine();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!_isInitialized)
        {
            return;
        }
        Debug.Log($"collided with {collision.gameObject}");
        // 땅 처리
        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    return;
        //}
        // 자기 자신 처리
        if (_owner != null && collision.gameObject == _owner.Behaviour.gameObject)
        {
            return;
        }
        // 팀원 처리

        SpawnAoEField();
        _hasExploded = true;
    }

    private void SpawnAoEField()
    {
        if (_hasExploded) return;
        //GameObject field = Resources.Load<GameObject>("AttackerAoEField");
        //AttackerAoEField aoeField = field.GetComponent<AttackerAoEField>();
        GameObject field = PhotonNetwork.Instantiate("AttackerAoEField", transform.position, Quaternion.identity);
        AttackerAoEField aoeField = field.GetComponent<AttackerAoEField>();

        aoeField.StartAoEField(_owner, _duration, _damage);
        PhotonNetwork.Destroy(gameObject);
    }

    public void SetData(SkillData data, CharacterBase character, Vector3? direction, CharacterBase target = null)
    {
        _owner = character;
        _speed = data.Speed;
        _damage = data.Damage;
        _maxRange = data.MaxRange;
        _radius = data.Radius;
        _duration = data.Duration; // 장판 유지 시간
        _direction = direction ?? Vector3.forward;
        _traveledDistance = 0f;

        _direction = direction ?? Vector3.forward;
        _traveledDistance = 0f;

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

    private void DrawBezierDebugLine()
    {
        if (!_isInitialized) return;

        int resolution = 20; // 샘플 수
        Vector3 prev = _startPosition;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = Bezier.GetPoint(_startPosition, _controlPoint1, _controlPoint2, _endPosition, t);
            Debug.DrawLine(prev, point, Color.cyan, 0f, false);
            prev = point;
        }
    }

}
