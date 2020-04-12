using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5.0f;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    // protected로 해주는 게 좋다.
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // 기본 스무딩을 적용하지 않는다.
                                                                                                          // 좀 더 빠릿빠릿하게 움직이게 해준다.
        Vector3 moveVelocity = moveInput.normalized * moveSpeed; // normalized는 정규화된 벡터를 말함(크기가 1인 벡터)

        controller.Move(moveVelocity);

        // Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // 원점에서 바라봄으로
                                                                 // ax+by+cz+d = 0 에서 (a, b, c)가 normal vector이고, d가 inPoint라고 생각하면 된다.
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) // out이라는 키워드는 변수를 참조로 전달한다는 것이다.
        {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        // Weapon Input
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
