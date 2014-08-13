using UnityEngine;
using System.Collections.Generic;

public class MagnetSensor : MonoBehaviour
{
	public class OnCardboardTriggerListener : MonoBehaviour
	{
		public virtual void onCardboardTrigger() {}
	}

	public int _sampleSize = 40;
	public int _lowTrigger = 30;
	public int _highTrigger = 30;

	public MagnetListener _listener;

	private float _current, _mean, _tmp;
	private Queue<float> _window;

	void Start()
	{
		_window = new Queue<float> (_sampleSize + 1);
	}

	void OnEnable()
	{
		_window.Clear();
		Input.compass.enabled = true;
	}

	void OnDisable()
	{
		Input.compass.enabled = false;
	}

	void Update ()
	{
		// Add most recent reading
		_current = Input.compass.magneticHeading;
		_window.Enqueue(_current);

		// If we don't have enough samples, keep reading
		if(_window.Count < _sampleSize) return;
		// If you have more samples than we need get rid of the oldest one
		if(_window.Count > _sampleSize) _window.Dequeue();

		_mean = FindMean(_window);

		Debug.Log("----------------------------------------");
		Debug.Log("Current: " + _current);
		Debug.Log("Mean: " + _mean);
		Debug.Log("High Trigger: " + _highTrigger);
		Debug.Log("Low Trigger: " + _lowTrigger);

		if(_current < _mean - _lowTrigger || _current > _mean + _highTrigger)
		{
			Debug.Log("!!! FIRE !!!");
			if(_listener != null) _listener.onCardboardTrigger();
		}
	}

	private float FindMean(Queue<float> range)
	{
		_tmp = 0;
		foreach(float heading in range)
		{
			_tmp += heading;
		}
		return _tmp / range.Count;
	}
}
