using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private Renderer[] renderers;
    //캐릭터의 초기 생명치
    private int initHp = 100;
    //현재 생명치
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
    {   //생명 수치가 0보다 크고 충돌체가 태그가 맞는 경우
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
        //캐릭터 투명처리
        SetPlayerVisible(false);

        yield return new WaitForSeconds(1.5f);

        //출현 위치 정보를 배열에 저장
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
