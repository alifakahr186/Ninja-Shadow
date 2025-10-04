using UnityEngine;

public class OtherCrows : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Flight Settings")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 3.5f;
    public float waypointDistance = 0.2f;

    [Header("Natural Motion")]
    public float verticalAmplitude = 0.5f; // how much up/down wave
    public float verticalFrequency = 2f;   // speed of wave
    public float circularChance = 0.3f;    

    [Header("Randomization")]
    public float randomDirectionTime = 3f; // after how many seconds to re-roll direction/behavior

    private Transform targetPoint;
    private float speed;
    private float timeOffset;
    private bool circularMode = false;
    private float nextRandomChange;

    void Start()
    {
        targetPoint = pointB; // start towards B
        RollNewBehavior();
    }

    void Update()
    {
        // Time-based re-roll of behavior
        if (Time.time > nextRandomChange)
        {
            RollNewBehavior();
        }

        // Move crow
        if (circularMode)
            FlyInCircularMotion();
        else
            FlyWithNaturalSway();
    }

    void FlyWithNaturalSway()
    {
        // move towards target
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        Vector3 move = direction * speed * Time.deltaTime;

        // add vertical sine wave for fluttering
        move.y += Mathf.Sin((Time.time + timeOffset) * verticalFrequency) * verticalAmplitude * Time.deltaTime;

        transform.position += move;

        // check if reached waypoint
        if (Vector3.Distance(transform.position, targetPoint.position) < waypointDistance)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
            Flip(targetPoint == pointB);
        }
    }

    void FlyInCircularMotion()
    {
        // simple circle around midpoint of A and B
        Vector3 center = (pointA.position + pointB.position) / 2;
        float radius = Vector3.Distance(pointA.position, pointB.position) / 2;

        // rotate around center
        transform.RotateAround(center, Vector3.forward, speed * 10f * Time.deltaTime);

        // face correct direction
        Vector3 dir = (center - transform.position).normalized;
        if (dir.x > 0) Flip(true); else Flip(false);
    }

    void RollNewBehavior()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        timeOffset = Random.Range(0f, 100f);
        circularMode = (Random.value < circularChance);
        nextRandomChange = Time.time + randomDirectionTime;
    }

    void Flip(bool facingRight)
    {
        if (facingRight)
            transform.rotation = Quaternion.Euler(0, 0, 0); // right
        else
            transform.rotation = Quaternion.Euler(0, 180, 0); // left
    }
}
