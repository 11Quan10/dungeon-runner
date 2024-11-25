using UnityEngine;
using UnityEngine.Events;
using Unity.FPS.Game;

public class Enemy : MonoBehaviour
{

    public GameObject target;
    public float detectionDistance = 100f;
    public Health m_health;

    private bool healing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_health = GetComponent<Health>();
        m_health.OnDie += OnDie;
    }

    // Update is called once per frame
    void Update()
    {
        /*RaycastHit hit = new RaycastHit();
        Vector3 directionVector = target.transform.position - gameObject.transform.position;
        Physics.Raycast (gameObject.transform.position, directionVector, out hit, detectionDistance, -1, QueryTriggerInteraction.Ignore);

        directionVector.y = 0;

        Rigidbody body = GetComponent<Rigidbody>();

        if (hit.collider.tag == target.GetComponent<Collider>().tag) {
            body.AddForce(directionVector / 100, ForceMode.Impulse);
        }*/

        if (healing) {
            m_health.Heal(1f);
            if (m_health.CurrentHealth >= m_health.MaxHealth) {
                healing = false;
                m_health.Invincible = false;
                m_health.CurrentHealth = m_health.MaxHealth;
            }
        }

        if (m_health.CurrentHealth <= 0f) {
            OnDie();
        }
    }

    void OnDie()
    {
        healing = true;
        m_health.Invincible = true;
        //Destroy(gameObject);
    }
}
