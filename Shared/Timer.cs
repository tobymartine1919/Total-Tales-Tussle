using UnityEngine;
using System;

[System.Serializable]
public class Timer
{
    [SerializeField] private float _duration;
    private float _nextTriggerTime;
    private bool _hasStarted;

    public bool IsReady => _hasStarted && Time.time >= _nextTriggerTime;
    public bool IsInDuration => _hasStarted && Time.time < _nextTriggerTime;
    public float RemainingTime => Mathf.Max(0f, _nextTriggerTime - Time.time);
    public float ProgressInPercentage => IsReady ? 1f : 1f - (RemainingTime / _duration);
    public float ElapsedTime => IsReady ? _duration : _duration - RemainingTime;
    public int CurrentFrame => Mathf.FloorToInt(ElapsedTime * 60f);
    public int TotalFrames => Mathf.FloorToInt(_duration * 60f);
    public int RemainingFrames => TotalFrames - CurrentFrame;

    public Timer(float duration)
    {
        this._duration = duration;
        _nextTriggerTime = 0f;
        _hasStarted = false;
    }

    public void Start()
    {
        _hasStarted = true;
        _nextTriggerTime = Time.time + _duration;
    }

    public void Reset()
    {
        _hasStarted = false;
        _nextTriggerTime = 0f;
    }

    public void SetDuration(float newDuration) => _duration = newDuration;

    public void Finish()
    {
        _hasStarted = true;
        _nextTriggerTime = Time.time - _duration;
    }
}