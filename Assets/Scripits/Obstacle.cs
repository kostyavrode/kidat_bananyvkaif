using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Obstacle : MonoBehaviour
{
    public int type;

    private void Start()
    {
        if (type==0)
        GoRight();
        if (type == 1)
            GoLeft();
        if (type == 2)
            GoUp();
    }
    private void StartRotation()
    {

    }
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, 1));
    }
    private void GoRight()
    {
        transform.DOMove(transform.position + transform.right * 15, 3).SetLoops(-1,LoopType.Yoyo);
    }
    private void GoLeft()
    {
        transform.DOMove(transform.position - transform.right * 15, 3).SetLoops(-1, LoopType.Yoyo);
    }
    private void GoUp()
    {
        transform.DOMove(transform.position + transform.up * 15, 6).SetLoops(-1, LoopType.Yoyo);
    }
}
