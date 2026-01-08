using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
	public Sprite fullHeart, halfHeart, emptyHeart;
	Image heartImage;

	private void Awake()
	{
		heartImage = GetComponent<Image>();
	}

	public void SetHeartImage(TypeOfHeart heart)
	{
		switch (heart)
		{
			case TypeOfHeart.Empty:
				heartImage.sprite = emptyHeart;
				break;
			case TypeOfHeart.Half:
				heartImage.sprite = halfHeart;
				break;
			case TypeOfHeart.Full:
				heartImage.sprite = fullHeart;
				break;
		}
	}
}

public enum TypeOfHeart
{
	Empty = 0,
	Half = 1,
	Full = 2
}
