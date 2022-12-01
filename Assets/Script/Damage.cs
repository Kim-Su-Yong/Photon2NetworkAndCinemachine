using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private Renderer[] renderers;
    //ĳ������ �ʱ� ����ġ
    private int initHp = 100;
    //���� ����ġ
    public int curHp = 100;
    private Animator anim;
    private CharacterController cc;
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");
    private readonly string bulletTag = "BULLET";
    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        curHp = initHp;
    }
    private void OnCollisionEnter(Collision col)
    {   //���� ��ġ�� 0���� ũ�� �浹ü�� �±װ� �´� ���
        if (curHp > 0 && col.collider.CompareTag(bulletTag))
        {
            curHp -= 20;
            if(curHp <= 0)
            {
                StartCoroutine("PlayerDie");
            }
        }
    }
    IEnumerator PlayerDie()
    {
        cc.enabled = false;
        anim.SetBool(hashRespawn, false);
        anim.SetTrigger(hashDie);
        yield return new WaitForSeconds(3.0f);

        anim.SetBool(hashRespawn, true);
        //ĳ���� ����ó��
        SetPlayerVisible(false);

        yield return new WaitForSeconds(1.5f);

        //���� ��ġ ������ �迭�� ����
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;
        curHp = 100;
        SetPlayerVisible(true);
        cc.enabled = true;
    }
    void SetPlayerVisible(bool isvisible)
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isvisible;
        }
    }
}
