using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    Animator[] anims = null;
    Sprite[] sprites = null;
    GameObject[] go = null;
	// Use this for initialization
	void Start () {
        go = GameObject.FindGameObjectsWithTag("panda");
        Debug.Log(go.Length);

        anims = FindObjectsOfType<Animator>();
        Debug.Log(string.Format("Animators: {0}", anims.Length));
        Debug.Log(string.Format("Avatar: {0}", anims[0].avatar));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
