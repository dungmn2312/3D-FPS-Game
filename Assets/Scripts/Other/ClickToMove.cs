using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ray: tia ray được bắn ra
            // hit: vật bị tia ray bắn trúng (hit.point: điểm bị bắn, hit.collider: vật bị bắn)
            // Infinity: Phạm vi bắn của tia ray là không giới hạn, bắn xa mãi mãi trừ khi gặp vật cản
            // AllAreas: Thực chất là bitmask, tất cả các mask đều có thể tính là bị bắn
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, NavMesh.AllAreas)) {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }
}
