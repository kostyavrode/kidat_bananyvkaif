using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Ballistics : MonoBehaviour
{
    public static Ballistics instance;
    public Transform SpawnTransform;
    public Transform TargetTransform;
    public Slider slider;
    public Slider slider2;
    public float AngleInDegrees;
    public float speed;

    float g = Physics.gravity.y;

    public GameObject Bullet;
    private GameObject newBullet;
    private int playedTimes;
    public GameObject[] obstacles;
    private void SpawnObject()
    {
        newBullet = Instantiate(Bullet, SpawnTransform.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().isKinematic = true;
    }
    private void Start()
    {
        SpawnObject();
        SpawnObstacles();
    }
    void FixedUpdate()
    {
        SpawnTransform.localEulerAngles = new Vector3(-AngleInDegrees, 0f, 0f);
        ChangeAngle(slider.value);
        ChangeSpeed(slider2.value);
    }
    public void ShootButton()
    {
        if (newBullet != null)
        {
            Shot();
        }
    }
    public void SpawnObstacles()
    {
        Debug.Log("Spaw");
        for(int i=0;i<Random.Range(0,4);i++)
        {
            obstacles[Random.Range(0, obstacles.Length)].SetActive(true);
        }
    }
    public void ChangeAngle(float temp)
    {
        AngleInDegrees = temp;
    }
    public void ChangeSpeed(float temp)
    {
        speed = temp;
    }
    public void Shot()
    {
        newBullet.GetComponent<Banana>().isCanRotate = true;
        Vector3 fromTo = TargetTransform.position - transform.position;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);

        transform.rotation = Quaternion.LookRotation(fromToXZ, Vector3.up);


        float x = fromToXZ.magnitude;
        float y = fromTo.y;

        float AngleInRadians = AngleInDegrees * Mathf.PI / 180;

        float v2 = (g * x * x) / (2 * (y - Mathf.Tan(AngleInRadians) * x) * Mathf.Pow(Mathf.Cos(AngleInRadians), 2));
        float v = Mathf.Sqrt(Mathf.Abs(v2));

        newBullet.GetComponent<Rigidbody>().isKinematic = false;
        newBullet.GetComponent<Rigidbody>().velocity = SpawnTransform.forward * v*speed;
        newBullet = null;
        playedTimes+=1;
        if (!CheckPlayedTimes())
        {
            StartCoroutine(WaitForEndGame());
        }
        else
        {
            StartCoroutine(WaitForSpawn());
        }
    }
    private bool CheckPlayedTimes()
    {
        if (playedTimes == 3)
        {
            return false;
        }
        else return true;
    }
    private IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(2);
        SpawnObject();
    }
    private IEnumerator WaitForEndGame()
    {
        yield return new WaitForSeconds(3);
        GameManager.instance.EndGame();
    }
}
