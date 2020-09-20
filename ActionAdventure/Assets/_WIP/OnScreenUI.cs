using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenUI : MonoBehaviour
{
    private static OnScreenUI _instance;
    public static OnScreenUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(OnScreenUI)).AddComponent<OnScreenUI>();
            }
            return _instance;
        }
    }

    public float TargetedSpeed { get; private set; }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (Input.touchCount >= 5 && _delay < 0)
        {
            ToggleDebug();
            _delay = 3f;
        }

        _delay -= Time.deltaTime;
    }

    [SerializeField] RectTransform _leftZone = null;
    [SerializeField] RectTransform _leftAnalog = null;

    public void EnableJoystick(Vector2 position)
    {
        _leftAnalog.gameObject.SetActive(true);
        _leftZone.gameObject.SetActive(true);
        _leftAnalog.transform.position = position;

        _leftZone.transform.position = position - Vector2.up * _leftAnalog.rect.height;
    }

    public void UpdateJoystick(Vector2 position)
    {
        _leftAnalog.position = position;

        if ((_leftAnalog.position - _leftZone.position).sqrMagnitude > _leftAnalog.rect.height * _leftAnalog.rect.height)
        {
            _leftZone.position = Vector3.Lerp(_leftZone.position, _leftZone.position + (_leftAnalog.position - _leftZone.position) * 0.5f, 0.3f);
        }

        TargetedSpeed = Direction().magnitude / _leftAnalog.rect.width;
    }

    public void DisableJoystick()
    {
        _leftAnalog.gameObject.SetActive(false);
        _leftZone.gameObject.SetActive(false);
        TargetedSpeed = 0.0f;
    }

    public Vector2 Direction()
    {
        return (_leftAnalog.position - _leftZone.position);
    }

    #region Debug
    private bool toggleDebug = false;
    [SerializeField] private GameObject _debugMenu = null;
    float _delay = 0;

    [SerializeField] UnityEngine.UI.Text _debugCounter = null;

    public void ToggleDebug()
    {

        if (toggleDebug)
        {
            _debugMenu.SetActive(false);
            toggleDebug = false;
        }
        else
        {
            _debugMenu.SetActive(true);
            toggleDebug = true;
        }
    }

    public void DebugUpdateTapCount(string tapCount)
    {
        _debugCounter.text = tapCount;
    }
    #endregion
}
