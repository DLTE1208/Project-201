using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProcessor : MonoBehaviour
{
    private string connectionString = "server = DESKTOP-D65RF5L\\MSSQLSERVER2; uid = admin; pwd = 123456; database = RestaurantOfDLT";  //�������ӵ����ݿ���ַ���
    //private CustomerFunctions myFunctions;
    //second

    private void Awake()
    {
        //myFunctions = GetComponent<CustomerFunctions>();
    }

    private void Start()
    {
        Debug.Log("Begin");
        ShopkeeperFunctions.GetOrderInfo(connectionString);
        Debug.Log("End");
    }
}
