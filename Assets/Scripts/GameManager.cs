using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    Animator[] anims = null;
    GameObject[] go = null;
    List<GameObject> posPanda = null;
    ConfigManager cm = null;
    GameObject arrow = null;

    float startedTime;
    float waiting = 1.0f;
    
    int curPos = -1;

    PandaStatus pandaStatus = PandaStatus.Waiting;

    enum Direction {
        Left, Right
    }

    enum PandaStatus {
        Waiting, StartMoving, Moving, EndMoving
    }

    public float distancePerSecond = 1.0f;
	// Use this for initialization
	void Start () {
        cm = new ConfigManager();
        startedTime = Time.time+10.0f;
        distancePerSecond = 3.0f / (cm.speed * 0.001f);
        waiting = (cm.wait * 0.001f);

        go = GameObject.FindGameObjectsWithTag("panda");
        Debug.Log(go.Length);

        posPanda = new List<GameObject>();
        foreach(GameObject panda in go) {
            posPanda.Add(panda);
        }
        posPanda.Sort((a, b) => {
            return NumberPanda(a) - NumberPanda(b);
        });
        int i = 0;
        foreach(GameObject p in posPanda) {
            i++;
            if(i > cm.maxPanda) {
                p.SetActive(false);
            }
            Debug.Log(p.name);
        }

        anims = FindObjectsOfType<Animator>();
        Debug.Log(string.Format("Animators: {0}", anims.Length));
        Debug.Log(string.Format("Avatar: {0}", anims[0].avatar));

        arrow = GameObject.FindGameObjectWithTag("arrow");
        arrow.transform.position = new Vector3((cm.target-1)*3.0f, 2.15f, 0);
    }

    private int NumberPanda(GameObject gobj) {
        return int.Parse(gobj.name.Replace("panda", ""));
    }

    // Update is called once per frame
    void Update()
    {
        if(cm == null) {
            Debug.Log("No CM Ready");
            return;
        }

        if(Time.time < startedTime) {
            return;
        }

        curPos = CurrentTodo();
        if(curPos < 0) {
            return;
        }
        if(curPos >= cm.steps.Count) {
            return;
        }
        int left = cm.steps[curPos][0];
        int right = cm.steps[curPos][1];
        ExchangePanda(left, right);
//Debug.Log(currentTodo());
        /*
        switch (pandaStatus) {
            case PandaStatus.Moving: ExchangePanda();
        }
        go[0].transform.Translate(distancePerSecond * Time.deltaTime, 0, 0);*/
    }

    private int CurrentTodo() {
        float elapsedTime = Time.time - startedTime;
        //Debug.Log(cm.speed);
        int now = (int)Math.Truncate(elapsedTime / ((cm.speed * 0.001f) + waiting));
        if(now < 0) {
            return -1;
        }
        float advanced = (elapsedTime - (now * ((cm.speed * 0.001f) + waiting)));
        //Debug.Log("now:" + now + ", adv:" + advanced);
        if(advanced > (cm.speed * 0.001f)) {
            Debug.Log("waiting time");
            if (pandaStatus == PandaStatus.Moving) {
                pandaStatus = PandaStatus.EndMoving;
            } else {
                pandaStatus = PandaStatus.Waiting;
            }
        } else if(now != curPos) {
            Debug.Log("start time");
            pandaStatus = PandaStatus.StartMoving;
        }
        return now;
    }


    private void MovePanda(int target, Direction direction, int startFlag) {
        GameObject panda = posPanda[target - 1];
        if (startFlag == 1) {
            Animator anim = panda.GetComponent<Animator>();
            anim.SetInteger("running", (direction == Direction.Left ? 1 : 2));
            if (NumberPanda(panda) == cm.target) {
                arrow.transform.position = new Vector3(panda.transform.localPosition.x, 2.15f, 0);
            }
        } else if (startFlag == 2) {
            Animator anim = panda.GetComponent<Animator>();
            anim.SetInteger("running", 0);
            panda.transform.position.Set((target + (direction == Direction.Left ? -1 : 1)) * 3.0f,2.15f,0);
            if (NumberPanda(panda) == cm.target) {
                arrow.transform.position = new Vector3(panda.transform.localPosition.x, 2.15f, 0);
            }
        } else {
            if (cm.targetvisible != 1) {
                arrow.SetActive(false);
            }
            if (NumberPanda(panda) == cm.target) {
                arrow.transform.Translate(distancePerSecond * Time.deltaTime * (direction == Direction.Left ? -1 : 1), 0, 0);
            }
            panda.transform.Translate(distancePerSecond * Time.deltaTime * (direction == Direction.Left ? -1 : 1), 0, 0);
        }
    }

    private void ExchangePanda(int left, int right) {
        Debug.Log("ExchangePanda: " + (left-1) + ", " + (right-1));
        Debug.Log("left:"+posPanda[left - 1].name+" right:"+posPanda[right - 1].name);
        if (pandaStatus == PandaStatus.StartMoving) {
            pandaStatus = PandaStatus.Moving;
            MovePanda(left, (left < right ? Direction.Right : Direction.Left), 1);
            MovePanda(right, (left < right ? Direction.Left : Direction.Right), 1);
        } else if(pandaStatus == PandaStatus.Moving) {
            MovePanda(left, (left < right ? Direction.Right : Direction.Left), 0);
            MovePanda(right, (left < right ? Direction.Left : Direction.Right), 0);
        } else if(pandaStatus == PandaStatus.EndMoving) {
            MovePanda(left, (left < right ? Direction.Right : Direction.Left), 2);
            MovePanda(right, (left < right ? Direction.Left : Direction.Right), 2);
            pandaStatus = PandaStatus.Waiting;
            posPanda.Reverse(Math.Min(left - 1, right - 1), 2);
        }
    }
}
