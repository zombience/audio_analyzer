using UnityEngine;
using System.Collections;
 
public class SimpleTimer
{
	public float life { get { return _life; } private set { _life = value; } }
	public float elapsed { get { return _curTime; } }
	public float normalized { get { return _curTime / life; } } // returns timer as a range between 0 and 1
	public float remaining { get { return life - elapsed; } }
	public bool isFinished { get { return elapsed >= life; } }
	public bool isPaused { get { return _isPaused; } private set { _isPaused = value; } }

	protected bool _fixedTime;
	protected bool _isPaused;
	protected float _life;
	protected float _startTime;
	protected float _pauseTime;
	protected float _curTime { get { return (isPaused ? _pauseTime : _getTime) - _startTime; } set { _pauseTime = value; } }
	protected float _getTime { get { return _fixedTime ? Time.fixedTime : Time.time; } }
	
 
	public SimpleTimer() { }
	/// <summary>
	/// timer is implicitly started on instantiation
	/// </summary>
	/// <param name="lifeSpan">length of the timer</param>
	/// <param name="useFixedTime">use fixed (physics) time or screen update time</param>
	public SimpleTimer(float lifeSpan, bool useFixedTime = false) { life = lifeSpan; _fixedTime = useFixedTime; _startTime = _getTime; }
	
	/// <summary>
	/// starts timer again using time remaining 
	/// </summary>
	public void Resume() { _startTime = (isPaused ? _getTime - elapsed : _getTime); isPaused = false; }
 
	/// <summary>
	/// stop pauses the timer and allows for resume at current elapsed time
	/// </summary>
	public void Stop()
	{
		if (!isPaused)
		{
			_curTime = _getTime;
			isPaused = true;
		}
	}

    /// <summary>
    /// Add time to the timer
    /// </summary>
    /// <param name="amt"></param>
    public void AddTime(float amt) 
    {
        _life += amt;
    }
}