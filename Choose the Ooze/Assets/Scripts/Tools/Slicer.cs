using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class Slicer : MonoBehaviour
{ 
    public SpriteMask ingredientOneMask;
    const int PART_OFF_SET = 6;

    public void Slice(float slicePointX, Ingredient ingredient)
    {
        SpriteRenderer ingrRenderer = ingredient.GetComponent<SpriteRenderer>();
        float ingrWidth = ingrRenderer.bounds.size.x;
        float ingrHeight = ingrRenderer.bounds.size.y;

        Transform maskTransform = ingredientOneMask.gameObject.transform;

        maskTransform.position = new Vector3(
            ingredient.transform.position.x,
            ingredient.transform.position.y
        );

        maskTransform.localScale = new Vector3(1, 2);

        float ingrLeftPercent = (slicePointX + (ingrWidth / 2)) / ingrWidth;
        print(ingrLeftPercent);

        float scaleForSecondPartWidth = 1 - ingrLeftPercent;

        maskTransform.localScale = new Vector3(
            ingrHeight,
            maskTransform.localScale.x * scaleForSecondPartWidth * 2
        );

        Vector3 cutPos = new Vector3(slicePointX + (ingredientOneMask.bounds.size.x / 2), maskTransform.transform.position.y, 0);
        maskTransform.position = cutPos;

        ingredientOneMask.gameObject.SetActive(true); 

        Ingredient cutPiece = Object.Instantiate(
            ingredient,
            new Vector3(
                ingredient.transform.position.x + PART_OFF_SET,
                Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).y + ingredientOneMask.bounds.size.y / 2
            ),
            Quaternion.identity
        );
        cutPiece.transform.rotation *= Quaternion.Euler(0, 0, -90);
        SpriteMask partMask = cutPiece.GetComponentInChildren<SpriteMask>();

        partMask.gameObject.transform.localScale = new Vector3(1, 2);

        partMask.gameObject.transform.localScale = new Vector3(
            ingrHeight,
            partMask.gameObject.transform.localScale.x * ingrLeftPercent * 2
        );

        partMask.gameObject.transform.position = new Vector3(
            slicePointX + PART_OFF_SET - (partMask.bounds.size.x / 2),
            partMask.transform.position.y
        );
    }

   public void HideMasks() {
        ingredientOneMask.gameObject.SetActive(false);
   }
}
