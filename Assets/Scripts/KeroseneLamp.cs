using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeroseneLamp : MonoBehaviour
{
	public int KeroseseneLevel = 100;

	[SerializeField] private Light[] _lights;
	[SerializeField] private GameObject _flame;

	[SerializeField] private float _burningSpeed = 9f;
	[SerializeField] private float _flickeringRate = 0.8f;
	private float _burningTimer = 0;
	private float _flickeringTimer = 0;
	private float[] _intensities = { 2f, 1f };
	private float[] _lowIntesities = { 0, 0 };
	private float[] _veryLowIntesities = { 0, 0 };
	[SerializeField] private float[] _flameScales = { 5, 3, 2 };

    public TextMeshProUGUI test;

    void Start()
	{
		_intensities[0] = _lights[0].intensity;
		_intensities[1] = _lights[1].intensity;

		_lowIntesities[0] = _lights[0].intensity * 0.5f;
		_lowIntesities[1] = _lights[1].intensity * 0.5f;

		_veryLowIntesities[0] = _lights[0].intensity * 0.1f;
		_veryLowIntesities[1] = _lights[1].intensity * 0.1f;
	}

	// Update is called once per frame
	void Update()
	{
        // Kerosene burning and light level
        _burningTimer += Time.deltaTime;

		if(_burningTimer >= _burningSpeed)
		{
			KeroseseneLevel--;
			_burningTimer = 0;
		}

		if (KeroseseneLevel <= 10 && KeroseseneLevel > 3)
		{

			if (_flame.transform.localScale.y > _flameScales[1])
				_flame.transform.localScale -= new Vector3(0, Time.deltaTime, 0);

			if (_lights[0].intensity > _lowIntesities[0])
				_lights[0].intensity -= Time.deltaTime;
			else
				_lights[0].intensity = _lowIntesities[0];

			if (_lights[1].intensity > _lowIntesities[1])
				_lights[1].intensity -= Time.deltaTime;
			else
				_lights[1].intensity = _lowIntesities[1];
		}
		else if (KeroseseneLevel <= 3)
		{
            if (_flame.transform.localScale.y > _flameScales[2])
                _flame.transform.localScale -= new Vector3(0, Time.deltaTime, 0);

            if (_lights[0].intensity > _veryLowIntesities[0])
                _lights[0].intensity -= Time.deltaTime;
            else
                _lights[0].intensity = _veryLowIntesities[0];

            if (_lights[1].intensity > _veryLowIntesities[1])
                _lights[1].intensity -= Time.deltaTime;
            else
                _lights[1].intensity = _veryLowIntesities[1];

			if (KeroseseneLevel <= 0)
			{
				KeroseseneLevel = 0;
				_lights[0].intensity = 0;
				_lights[1].intensity = 0;
				_flame.SetActive(false);
			}
		}
		else
		{
			_lights[0].intensity = _intensities[0];
			_lights[1].intensity = _intensities[1];
			_flame.SetActive(true);
			_flame.transform.localScale = new Vector3(1, _flameScales[0], 1);
		}

		// Fire flickering
		_flickeringTimer += Time.deltaTime;

		if(_flickeringTimer >= _flickeringRate)
		{
			_flickeringTimer = 0;
			_lights[0].intensity -= Random.Range(0, 0.2f);
		}
	}

	public void FillUp()
	{
		KeroseseneLevel = 100;
	}
}
