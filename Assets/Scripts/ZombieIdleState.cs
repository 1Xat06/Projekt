using UnityEngine;

public class ZombieIdleState : StateMachineBehaviour
{
    float timer;
    public float idleTime = 0f;

    Transform player;
    float detectionArea = 100f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (timer > idleTime)
        {
            animator.SetBool("isPatrolling", true);
        }

        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }
    }
}    