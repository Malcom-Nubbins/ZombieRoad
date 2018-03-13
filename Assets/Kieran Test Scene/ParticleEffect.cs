using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffect : MonoBehaviour {

	public GameObject prefab;

	ParticleSystem splashEmitterPS;

	public GameObject FollowCamera;

	List<GameObject> emittersList;

	void Start()
	{
		emittersList = new List<GameObject>();
	}

	public void registerAtPosition(Vector3 position, bool looping)
	{

		GameObject emitter = Instantiate(prefab, position, Quaternion.identity);
		ParticleSystem particleSystem = emitter.GetComponent<ParticleSystem>();

		//Debug.Log(particleSystem.main.startLifetimeMultiplier);
		StartCoroutine(deregister(emitter, particleSystem.main.startLifetimeMultiplier));

		emittersList.Add(emitter);

		play(particleSystem);
	}

	private IEnumerator deregister(GameObject emitter, float delay)
	{
		yield return new WaitForSeconds(delay);

		ParticleSystem particleSystem = emitter.GetComponent<ParticleSystem>();

		emittersList.Remove(emitter);
		Destroy(particleSystem);
		Destroy(emitter);
	}


	private void play(ParticleSystem particleSystem)
	{
		if (particleSystem.isEmitting)
		{
			particleSystem.Clear();
		}

		particleSystem.Play();
	}

}
