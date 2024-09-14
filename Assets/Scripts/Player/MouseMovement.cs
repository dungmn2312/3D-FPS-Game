using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouserMovement : MonoBehaviour
{
    public float mouseSensitivity = 70f;

    // Quay quanh trục X (nhìn lên xuống)
    float xRotation = 0f;

    // Giới hạn tầm nhìn không vượt quá 90 độ lên và xuống
    public float topClamp = -90f;
    public float bottomClamp = 60f;

    Transform player;

    // Start được gọi trước khi frame đầu tiên được cập nhật
    void Start()
    {
        // Khóa trỏ chuột ở giữa tâm màn hình và ẩn nó đi
        Cursor.lockState = CursorLockMode.Locked;
        player = GetComponentInParent<PlayerMovement>().gameObject.transform;
    }

    // Update được gọi một lần mỗi frame
    void Update()
    {
        // Nhận input từ chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Quay quanh trục X (nhìn lên xuống)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        // Áp dụng các giá trị xoay vào trường Rotation của Camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Quay quanh trục Y cho Player (nhìn trái phải)
        player.Rotate(Vector3.up * mouseX);
    }
}
