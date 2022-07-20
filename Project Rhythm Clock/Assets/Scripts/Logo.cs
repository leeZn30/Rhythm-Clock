using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logo : MonoBehaviour
{
	float FadeInTime = 0.2f;
	float FadeOutTime = 0.05f;
	[SerializeField] private Sprite[] images;


    // Start is called before the first frame update
    void Start()
	{
	}

    // Update is called once per frame
    void Update()
    {
		//StartCoroutine(FadeOutLogo());
		StartCoroutine(FadeInLogo(1f));
    }

	public void getSprite(int type)
    {
		if (type != null)
        {
			GetComponent<SpriteRenderer>().sprite = images[type];
        }
		else
        {
			Debug.Log("sprite is null");
        }
    }

	IEnumerator FadeInLogo(float alpha)
	{
		SpriteRenderer tmpSR = GetComponent<SpriteRenderer>();
		Color tmpColor = tmpSR.color;
		while (tmpColor.a < alpha)
		{
			tmpColor.a += Time.deltaTime / FadeInTime;
			tmpSR.color = tmpColor;
			yield return null;
		}

		if (tmpColor.a > alpha)
			tmpColor.a = alpha;
		tmpSR.color = tmpColor;


		callFadeOut();
	}

	void callFadeOut()
    {
		StartCoroutine(FadeOutLogo());
    }


	IEnumerator FadeOutLogo()
	{

		SpriteRenderer tmpSR = GetComponent<SpriteRenderer>();
		Color tmpColor = tmpSR.color;
		while (tmpColor.a > 0f)
		{
			tmpColor.a -= Time.deltaTime / FadeOutTime;
			tmpSR.color = tmpColor;
			yield return null;
		}

		if (tmpColor.a < 0f)
			tmpColor.a = 0f;
		tmpSR.color = tmpColor;


		GetComponent<SpriteRenderer>().sprite = null;
		ObjectPool.Instance.LogoQueue.Enqueue(gameObject);
		gameObject.SetActive(false);

	}
}
