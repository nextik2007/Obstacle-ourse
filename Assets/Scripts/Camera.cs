using System.Collections.Generic;
using UnityEngine;

public class CarCameraScript : MonoBehaviour
{
    public Transform _itemParent; // –одительский Transform, содержащий автомобили
    public float distance = 6.4f;
    public float height = 1.4f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;
    public float zoomRatio = 0.5f;
    public float defaultFOV = 60f;

    private Vector3 rotationVector;
    private Transform targetCar; // “екущий автомобиль, за которым следит камера


    void Start()
    {
        // »щем активные автомобили в начале игры
        FindActiveCar();
    }

    void LateUpdate()
    {
        if (targetCar == null)
        {
            return; // Ќе делаем ничего, если нет активного автомобил€
        }


        float wantedAngle = rotationVector.y;
        float wantedHeight = targetCar.position.y + height;
        float myAngle = transform.eulerAngles.y;
        float myHeight = transform.position.y;

        myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.deltaTime);
        myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
        transform.position = targetCar.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        Vector3 temp = transform.position;
        temp.y = myHeight;
        transform.position = temp;
        transform.LookAt(targetCar);
    }


    void FixedUpdate()
    {
        if (targetCar == null)
        {

            return; // Ќе делаем ничего, если нет активного автомобил€
        }

        Vector3 localVelocity = targetCar.InverseTransformDirection(targetCar.GetComponent<Rigidbody>().velocity);

        if (localVelocity.z < -0.1f)
        {
            Vector3 temp = rotationVector;
            temp.y = targetCar.eulerAngles.y + 180;
            rotationVector = temp;
        }
        else
        {
            Vector3 temp = rotationVector;
            temp.y = targetCar.eulerAngles.y;
            rotationVector = temp;
        }
        float acc = targetCar.GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Camera>().fieldOfView = defaultFOV + acc * zoomRatio * Time.deltaTime;
    }



    // ћетод дл€ поиска активного автомобил€
    void FindActiveCar()
    {
        if (_itemParent == null)
        {
            Debug.LogWarning("CarCameraScript: _itemParent is not assigned. Camera will not function correctly.");
            return;
        }

        // ѕолучаем список всех дочерних элементов (потенциальных автомобилей)
        Transform[] children = new Transform[_itemParent.childCount];
        for (int i = 0; i < _itemParent.childCount; i++)
        {
            children[i] = _itemParent.GetChild(i);
        }

        targetCar = null;

        // »щем первый активный автомобиль в списке дочерних элементов
        foreach (Transform child in children)
        {
            if (child.gameObject.activeInHierarchy && child.GetComponent<Rigidbody>() != null)
            {
                targetCar = child;
                break;
            }

        }
        if (targetCar == null)
        {
            Debug.LogWarning("CarCameraScript: No active car with Rigidbody found in _itemParent.");
        }
    }

    void Update()
    {
        // ѕоиск активной машины
        FindActiveCar();
    }
}