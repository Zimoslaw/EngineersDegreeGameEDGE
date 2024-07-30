using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of subtitles, infos etc. as a text
/// </summary>
public class Subtitles : MonoBehaviour
{
	[SerializeField] private Image _background;
	[SerializeField] private TextMeshProUGUI _text;
	[SerializeField] private Color[] _textColors;
	[SerializeField] private float _displayTime = 4;

	private bool _isShowing = false;
	private float _showTimer = 0;

	void Update()
	{
		// Message is displayed for _displayTime seconds and then faded away
		if (_isShowing)
		{
			_showTimer += Time.deltaTime;

			if (_showTimer > _displayTime)
			{
				_background.color = new Color(0, 0, 0, _background.color.a - Time.deltaTime);
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _background.color.a - (Time.deltaTime * 0.75f));

				if (_text.color.a <= 0)
				{
					_showTimer = 0;
                    _background.gameObject.SetActive(false);
                    _isShowing = false;
				}
            }
		}
	}

	/// <summary>
	/// Shows string message in given type (text color)
	/// </summary>
	/// <param name="message">Message to show</param>
	/// <param name="type">Type of message. Specifies the text color</param>
	public void ShowMessage(string message, byte type = 0)
	{
		// Reset timer
		_showTimer = 0;

		// Reset background color
		_background.color = new Color(0, 0, 0, 0.75f);

		// Color choosen by type
		_text.color = _textColors[type];

		// Background width and height
		int width = message.Length * 20;
		if (width < 120)
			width = 120;
		if (width > 1800)
			width = 1800;

  		int height = message.Length > 100? 100 : 50;

        _background.rectTransform.sizeDelta = new Vector2(width, height);
		

        _background.gameObject.SetActive(true);
		_text.text = message;
		_isShowing = true;
	}
}
