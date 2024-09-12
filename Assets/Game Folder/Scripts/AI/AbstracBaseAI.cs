using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstracBaseAI : MonoBehaviour
{
    public List<GameObject> playerList = new List<GameObject>();
    public GameObject target;
    public Transform targetPos;

    virtual public void setTarget(GameObject targets) { target = targets; }
    virtual public Vector3 getTargetDir() { return Vector3.zero; }
    virtual public GameObject getRandomPlayer()
    {
        if (playerList != null) return playerList[Random.Range(0, playerList.Count - 1)];
        return null;
    }
    virtual public float getDistanceToObject(GameObject obj)
    {
        float dist = Vector3.Distance(this.transform.position, obj.transform.position);
        return dist;
    }
    public GameObject getMyself() { return this.gameObject; }
}
