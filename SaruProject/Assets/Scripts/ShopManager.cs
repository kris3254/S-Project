using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

enum Direccion { up,down,left,right};
[Serializable]public struct rowsColumns { public int rows; public int columns; }
[Serializable] public struct items { public string itemInformation; public Sprite image; public int price; }
public class ShopManager : MonoBehaviour {

    public GameObject allItems;
    public GameObject allBackGroundItems;
    public float limitToAxisMove =.5f;
    public float secondToMove =.3f;
    public Color selectItem;
    public Color deselectItem;
    public Text infoItems;
    public Text priceItems;
    public items[] items;
    public rowsColumns numRowsAndColums;
    


    Image[] fondoImages;
    int actualPosition = 0;
    int nextPosition = 0;
    bool isChanging = false;
   
    // Use this for initialization
    void Start () {
        fondoImages = allBackGroundItems.transform.GetComponentsInChildren<Image>();
        int i = 0;
        foreach (Image item in allItems.GetComponentsInChildren<Image>())
        {
            item.sprite = items[i++].image;
        }
        fondoImages[actualPosition].color = Color.blue;

	}

    private void Update()
    {
        if (isChanging) return;
        if(Input.GetAxis("Horizontal") > limitToAxisMove)
        {
            MoveToItem(Direccion.right);
        }
        else if(Input.GetAxis("Horizontal") < -limitToAxisMove)
        {
            MoveToItem(Direccion.left);
        }

        if (Input.GetAxis("Vertical") > limitToAxisMove)
        {
            MoveToItem(Direccion.up);
        }
        else if (Input.GetAxis("Vertical") < -limitToAxisMove)
        {
            MoveToItem(Direccion.down);
        }

        //changeposition se debe llamar al final por la posibilidad de pulsar en diagonal
        ChangePosition();
    }

    private void MoveToItem(Direccion direccion)
    {
        switch (direccion)
        {
            case Direccion.up:
                HorizontalMove(1);
                break;
            case Direccion.down:
                HorizontalMove(-1);
                break;
            case Direccion.left:
                VerticalMove(-1);
                break;
            case Direccion.right:
                VerticalMove(1);
                break;
            default:
                break;
        }
    }

    private void HorizontalMove(int i)
    {
        nextPosition += i * numRowsAndColums.columns;
        CheckNextPosition();
    }

    private void VerticalMove(int i)
    {       
        nextPosition += i;
        CheckNextPosition();
    }

    private void CheckNextPosition()
    {
        if(nextPosition >= items.Length)
        {
            nextPosition = nextPosition - items.Length;
        }
        if (nextPosition < 0)
        {
            nextPosition = nextPosition + items.Length;
        }
    }

    private void ChangePosition()
    {
        fondoImages[actualPosition].color = deselectItem;
        fondoImages[nextPosition].color = selectItem;
        actualPosition = nextPosition;
        infoItems.text = items[actualPosition].itemInformation;
        priceItems.text = items[actualPosition].price.ToString();
        StartCoroutine(WaitForMove());
    }

    IEnumerator WaitForMove()
    {
        isChanging = true;
        yield return new WaitForSeconds(secondToMove);
        isChanging = false;
    }
}
