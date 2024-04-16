using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int dmg { get; private set; } = 1;

    public static float MinSpeed { get { return minSpeed; } set { minSpeed = value; minMagnitude = value * value * .9f; } }
    private static float minSpeed;
    private static float minMagnitude;
    private static float maxScanningAngle = 100f;
    
    private static float destroyTime = .5f;

    public bool IsShielded { get; set; } = false;


    private Rigidbody2D rb;
    [SerializeField]
    private Vector3 lastVelocity;
    //private int numOfCollisions = 0;
    //private Vector3 lastCollision;
    private Vector2 lastContact = Vector2.zero;

    private float cooldownOnStay = 0f;
    private float maxCooldownOnStay = .05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        MinSpeed = 10f;

        //lastCollision = Vector2.zero;
        //numOfCollisions = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsFacingCollision(collision))
        {
            ReflectVelocity(collision);
        }
        cooldownOnStay = Time.time + maxCooldownOnStay;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (cooldownOnStay <= Time.time && IsFacingCollision(collision))// && rb.velocity.sqrMagnitude < minMagnitude)
        {
            ReflectVelocity(collision);
            cooldownOnStay = Time.time + maxCooldownOnStay;
        }
    }

    private void ReflectVelocity(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 direction = Vector2.Reflect(lastVelocity, normal).normalized;

#if UNITY_EDITOR
        //Debug.DrawRay(collision.GetContact(0).point, normal.normalized, Color.blue, 5f);
        //Debug.DrawRay(collision.GetContact(0).point, direction, Color.green, 5f);
        //Debug.DrawRay(collision.GetContact(0).point, lastVelocity.normalized, Color.red, 5f);
        //Debug.Log("name: " + name + ", dir[green]:" + direction + ",\n normal[blue]:" + normal + ",\n lv[red]:" + lastVelocity.normalized + ",\n rbv[yellow]:" + rb.velocity.normalized);
#endif

        lastVelocity = rb.velocity = direction * minSpeed;
    }

    private bool IsFacingCollision(Collision2D collision)
    {
        Vector2 contactPoint = collision.collider.ClosestPoint(transform.position);
        float angle = Vector2.Angle(lastVelocity, contactPoint - (Vector2)transform.position);

#if UNITY_EDITOR
        //Debug.Log("facing angle difference is: " + angle + "\nLastVelocity: " + lastVelocity + " | othervector: " + (contactPoint - (Vector2)transform.position));
        //lastContact = contactPoint;
#endif

        if (angle < maxScanningAngle)
        {
            return true;
        }

        return false;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(lastContact, .1f);
    //}

    public void SetVelocity(Vector2 normalized)
    {
        //Debug.Log("name: " + name + ",\n input:" + normalized + ",\n transform.up:" + transform.up);
        lastVelocity = rb.velocity = normalized * minSpeed;
    }

    public void Return()
    {
        GetComponent<CircleCollider2D>().enabled = false;

        rb.velocity = (BallController.main.GetNewLocation() - transform.position) * (1 / destroyTime);
        Break(destroyTime);
    }

    public void Break(float delay = 0f)
    {
        Destroy(gameObject, delay);
    }
}



//void OnCollisionEnter2D(Collision2D collision)
//{
//    Vector2 oldVelocity = rb.velocity;
//    Vector2 difference = collision.transform.position - transform.position;
//    Debug.Log("normalImpulse: " + collision.contacts[0].normalImpulse);
//    Debug.Log("tangentImpulse: " + collision.contacts[0].tangentImpulse);

//    rb.velocity = collision.relativeVelocity;
//}

//void OnCollisionExit2D(Collision2D collision)
//{
//    if (IsUnderThreshold(rb.velocity))
//    {
//        rb.velocity = -lastVelocity;
//    }
//}

//private bool IsUnderThreshold(Vector3 velocity)
//{
//    if (velocity.x < minXValue && velocity.x > -minXValue)
//    {
//        return true;
//    }
//    else if (velocity.y < minYValue && velocity.y > -minYValue)
//    {
//        return true;
//    }
//    return false;
//}

/* // New version of the old one
void OnCollisionEnter2D(Collision2D collision)
{
    if (numOfCollisions == 0)
    {
        ReflectVelocity(collision);
        lastCollision = collision.transform.position;
    }
    else if (!IsLastCollision(collision))
    {
        ReflectVelocity(collision);
    }
    numOfCollisions++;
    cooldownOnStay = Time.time + maxCooldownOnStay;
}

void OnCollisionStay2D(Collision2D collision)
{
    if(cooldownOnStay <= Time.time && rb.velocity.magnitude < minMagnitude)
    {
        if(numOfCollisions <= 1 || !IsLastCollision(collision))
        {
            ReflectVelocity(collision);
            cooldownOnStay = Time.time + maxCooldownOnStay;
            lastCollision = collision.transform.position;
        }
    }
}

void OnCollisionExit2D(Collision2D collision)
{
    numOfCollisions--;
}

private void ReflectVelocity(Collision2D collision)
{
    Vector3 normal = collision.GetContact(0).normal;
    float speed = lastVelocity.magnitude;
    Vector2 direction = Vector2.Reflect(lastVelocity.normalized, normal);

    if (Physics2D.Raycast(transform.position, direction, wrongOrderRayCastLength, squareLayer))
        direction *= -1;

#if UNITY_EDITOR
    Debug.DrawRay(transform.position, -direction, Color.black, 5f);
    Debug.DrawRay(collision.GetContact(0).point, normal, Color.blue, 5f);
    Debug.DrawRay(collision.GetContact(0).point, direction, Color.green, 5f);
    Debug.DrawRay(collision.GetContact(0).point, rb.velocity.normalized, Color.yellow, 5f);
    Debug.DrawRay(collision.GetContact(0).point, lastVelocity.normalized, Color.red, 5f);
    Debug.Log("name: " + name + ", dir[green]:" + direction + ",\n normal[blue]:" + normal + ",\n lv[red]:" + lastVelocity.normalized+ ",\n rbv[yellow]:" + rb.velocity.normalized);
#endif

    lastVelocity = rb.velocity = direction * Mathf.Max(speed, minSpeed);
}

private bool IsLastCollision(Collision2D collision)
{
    return lastCollision.x == collision.transform.position.x && lastCollision.y == collision.transform.position.y;
}
*/

/* // Pure old handling
void OnCollisionEnter2D(Collision2D collision)
{
    if (numOfCollisions == 0)
    {
        ReflectVelocity(collision);
        lastCollision = collision.transform.position;
    }
    else if (lastCollision.x != collision.transform.position.x && lastCollision.y != collision.transform.position.y)
    {
        ReflectVelocity(collision);
    }
    numOfCollisions++;
}

void OnCollisionStay2D(Collision2D collision)
{
    if (rb.velocity.magnitude < minSpeed)
    {
        rb.velocity = -lastVelocity;
    }
}

void OnCollisionExit2D(Collision2D collision)
{
    numOfCollisions = 0;
}

private void ReflectVelocity(Collision2D collision)
{
    Vector3 normal = collision.GetContact(0).normal;
    float speed = lastVelocity.magnitude;
    Vector2 direction = Vector2.Reflect(lastVelocity.normalized, normal);

    //Debug.DrawRay(collision.GetContact(0).point, normal, Color.blue, 5f);
    //Debug.DrawRay(collision.GetContact(0).point, direction, Color.green, 5f);
    //Debug.Log(speed + ", dir: \n" + direction + ", lv: \n" + lastVelocity);
    lastVelocity = rb.velocity = direction * Mathf.Max(speed, minSpeed);
}
*/
