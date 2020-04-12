using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))] // 이게 있으면 자동으로 생성
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody myRigidBody;
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 compensatedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z); // 이를 하지 않을 시 캡슐이 ray 축으로 누워버린다.
        transform.LookAt(compensatedPoint);
    }

    // 정기적으로 짧게 반복적으로 실행되는 데에 사용됩니다.
    // 프레임 저하가 발생해도 가중치를 곱해 이동속도를 일정하게 만듬
    protected void FixedUpdate()
    {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
    }

}
