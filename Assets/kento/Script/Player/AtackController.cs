using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditorInternal;
using UnityEngine;

public class AtackController : MonoBehaviour
{
    [SerializeField] private float curentForce = 15f;
    private float duration = 0.5f;
    private float cooldown = 1.0f; //攻撃クールダウン
    private bool lisCooldown = false;

    [HideInInspector] public float chargeMax = 5.0f; //チャージ上限
    private float t = 0f;
    private bool isMax = false;

    //-----硬直-----
    [SerializeField] private float StrongRecoveryTime = 1.0f; //硬直時間
    private float curentRecoveryTime;
    [HideInInspector] public bool isRigid = false;

    [Header("ノックバック,無敵設定")]
    [SerializeField] private float WeakKnockbackForce = 15.0f; //弱ノックバック
    [SerializeField] private float StrongKnockbackForce = 30.0f;//強ノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力

    [Header("当たり判定設定")]
    [SerializeField] private SphereCollider searchArea;
    [SerializeField] private float angle = 45f;

    Rigidbody rb;
    PlayerStateManager stateManager;
   
    public void SetCharge(float value)
    {
        t = value;
    }

    private void Start()
    {
        curentRecoveryTime = StrongRecoveryTime;

        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        if (stateManager.ActionState == ActionState.Charge)
        {
            if (t < chargeMax)
            {
                t += Time.deltaTime;
                //stateManager.SetAttackPower(AttackPower.Weak);
            }
            if(t >= chargeMax)
            {
                isMax = true;
                //stateManager.SetAttackPower(AttackPower.Strong);
            }
        }
        if(stateManager.State == State.Knockback)
        {
            t = 0f;
            CancelInvoke("EndAttack");
            EndAttack();
        }
        else
        {
            t = 0f;
            //stateManager.SetAttackPower(AttackPower.None);
        }
        if (isRigid)
        {
            if(curentRecoveryTime > 0f)
            {
                curentRecoveryTime -= Time.deltaTime;
            }
            if(curentRecoveryTime <= 0f)
            {
                isRigid = false;
                curentRecoveryTime = StrongRecoveryTime;
            }
        }
    }

    public void Shot(int x)
    {
        if (x == 0)
        {
            if(lisCooldown) { return; }
            if (stateManager.ActionState == ActionState.Charge) {return; }
            isRigid = false;

            //チャージ開始,ステート変更
            stateManager.SetActionState(ActionState.Charge);
        }
        if (x == 1)
        {
            if(lisCooldown) { return; }
            if (isRigid) { return; }

            if (stateManager.ActionState == ActionState.Charge)
            {
                //チャージを止め攻撃,ステート変更
                stateManager.SetActionState(ActionState.Attack);

                //? = true , : = false
                curentknockbackForce = isMax ? StrongKnockbackForce : WeakKnockbackForce;

                rb.AddForce(transform.forward * curentForce, ForceMode.Impulse);

                Invoke("EndAttack", duration);
            } 
          
        }  
    }

    void EndAttack()
    {
        rb.linearVelocity = Vector3.zero;
        //ステートをNoneに
        stateManager.SetActionState(ActionState.None);

        if (isMax)
        {
            Debug .Log("強攻撃");
            isRigid = true;
        }
    
        isMax = false;
        //stateManager.SetAttackPower(AttackPower.None);
        t = 0f;

        StartCoroutine(CooldownCount());
    }

    IEnumerator CooldownCount()
    {
        lisCooldown = true;
        yield return new WaitForSeconds(cooldown);
        lisCooldown = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 posDir = other.transform.position - this.transform.position;
            float target_angle = Vector3.Angle(this.transform.forward, posDir);

            var dist = Vector3.Distance(other.transform.position, transform.position);

            if (target_angle > angle) { return; }

            if (target_angle <= angle)
            {
                if (Physics.Raycast(this.transform.position + Vector3.up * 0.5f, posDir, out RaycastHit hit))
                {
                    if (hit.collider == other)
                    {
                        //Debug.Log("Hit");
                        if (stateManager.ActionState == ActionState.Attack)
                        {
                            Reception p = other.gameObject.GetComponent<Reception>();
                            //if (p.isHit) { return; }
                            p.KnockBack(rb.linearVelocity.normalized, curentknockbackForce);
                            //当たった時点でInvokeをキャンセルしてタックルを止める
                            CancelInvoke("EndAttack");
                            EndAttack();
                        }
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        pos.y = 1.0f;
        Handles.color = Color.red;
        Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0.0f, -angle, 0f) * transform.forward, angle * 2f, searchArea.radius);
    }
#endif
}