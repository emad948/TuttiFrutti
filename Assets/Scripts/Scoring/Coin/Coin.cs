using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public GameObject Torus;
    public GameObject Indicator;
    private GameObject playerCollected;
    private float fadeState = 0;

    void Update() {
        if (!playerCollected) return;
        // animation after collected
        Vector3 targetPosition = playerCollected.transform.position + Vector3.up - transform.rotation.eulerAngles * 0.5f;
        Torus.transform.position = Vector3.Lerp(Torus.transform.position, targetPosition, Time.deltaTime * 2);
        fadeState += Time.deltaTime;
        Torus.GetComponent<Animator>().SetFloat("Fade", fadeState);

    }

    private void touched(GameObject other) {
        if (playerCollected || (1 << other.layer & LayerMask.GetMask("Pla_yer")) == 0) return;
        Indicator.SetActive(false);
        playerCollected = other;
        Destroy(gameObject, 2f);
        CoinManager.singleton().collected(gameObject);
    }
}
