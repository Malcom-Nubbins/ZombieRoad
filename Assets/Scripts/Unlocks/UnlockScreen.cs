using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockScreen : MonoBehaviour
{
	public GameObject EffectPrefab;
	public GameObject Coins;

	GameObject newItemDisplay;
    Quaternion rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
	GameObject lootbox;
	AnimationCurve[] animLB = new AnimationCurve[2];
	AnimationCurve animNID;
	bool bPlayedExplosion = false;
	float fTimeToBurst = 0.75f;
	float fTimeToFullSize;
	void Start()
    {
		/*Scale*/animLB[0] = new AnimationCurve(new Keyframe(0, 9.0f), new Keyframe(fTimeToBurst, 8.0f), new Keyframe(fTimeToBurst+1.0f, 7000.0f));
		/* Yaw */animLB[1] = new AnimationCurve(new Keyframe(0, 175), new Keyframe(1, 185));
		animLB[0].postWrapMode = WrapMode.ClampForever;
		animLB[1].postWrapMode = WrapMode.PingPong;

		lootbox = Instantiate(Resources.Load<GameObject>("Prefabs/lootbox"), Vector3.zero, rotation, transform);
		lootbox.transform.localScale = new Vector3(animLB[0].Evaluate(Time.timeSinceLevelLoad), animLB[0].Evaluate(Time.timeSinceLevelLoad), animLB[0].Evaluate(Time.timeSinceLevelLoad));

		UnlockManager um = GameObject.Find("UnlockManager").GetComponent<UnlockManager>();
        Unlockable newItem = um.UnlockRandom();
		if (newItem == null)
		{
			//SceneManager.LoadScene("GameOverScene");
			//return;
			newItem = Coins.GetComponent<Unlockable>();
		}
		
		{
			GameObject prefab = newItem.gameObject;
			newItemDisplay = Instantiate(prefab, Vector3.zero, rotation, transform);
			newItemDisplay.transform.localScale = Vector3.zero;
		}

		Keyframe kf2;
		fTimeToFullSize = fTimeToBurst + 0.8f;

		if (newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SKYSCRAPER)
		{
			kf2 = new Keyframe(fTimeToFullSize, 0.3f);
		}
		else if (newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.HOUSE || newItemDisplay.GetComponent<Unlockable>().type == UnlockableType.SHOP)
		{
			kf2 = new Keyframe(fTimeToFullSize, 0.65f);
		}
		else
		{
			kf2 = new Keyframe(fTimeToFullSize, 1.5f);
		}
		animNID = new AnimationCurve(new Keyframe(0, 0), new Keyframe(fTimeToBurst, 0), kf2);
		animNID.postWrapMode = WrapMode.ClampForever;
	}
	
	void Update()
    {
		lootbox.transform.position = new Vector3(0.0f, -6.75f, 20.0f);
		lootbox.transform.localScale = new Vector3(animLB[0].Evaluate(Time.timeSinceLevelLoad), animLB[0].Evaluate(Time.timeSinceLevelLoad), animLB[0].Evaluate(Time.timeSinceLevelLoad));
		lootbox.transform.localRotation = Quaternion.Euler(0.0f, animLB[1].Evaluate(Time.timeSinceLevelLoad), 0.0f);

		if (!bPlayedExplosion && Time.timeSinceLevelLoad > fTimeToBurst)
		{
			Instantiate(EffectPrefab, newItemDisplay.transform);
			bPlayedExplosion = true;
		}

		if (bPlayedExplosion && newItemDisplay)
        {
			newItemDisplay.transform.Rotate(Vector3.up, 20 * Time.deltaTime);
			newItemDisplay.transform.localScale = new Vector3(animNID.Evaluate(Time.timeSinceLevelLoad), animNID.Evaluate(Time.timeSinceLevelLoad), animNID.Evaluate(Time.timeSinceLevelLoad));

            newItemDisplay.transform.position = new Vector3(0.0f, -6.75f, 20.0f);
		}
    }
}
