using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class GrindingMinigame : MonoBehaviour
{
    Camera grindingCamera;
    const float GRIND_SPEED = .1f;
    public Ingredient ingredient;
    public GameObject powder;
    public GameObject raw;
    public SpriteMask mask;
    public Pestle pestle;
    float grinding_percentage;

    void Awake()
    {
        grindingCamera = Camera.main;
    }

    void ResetMinigame()
    {
        switch (ingredient.currentLevels.grindLevel)
        {
            case GrindLevel.None:
                grinding_percentage = 0;
                break;

            case GrindLevel.Low:
                grinding_percentage = 0.3f;
                break;

            case GrindLevel.Medium:
                grinding_percentage = 0.6f;
                break;

            case GrindLevel.High:
                grinding_percentage = 1;
                break;
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        // Move the raw and powder ingredients per the grinding percentage
        raw.transform.position = new Vector3(
            raw.transform.position.x,
            -grinding_percentage,
            raw.transform.position.z
        );
        powder.transform.position = new Vector3(
            powder.transform.position.x,
            grinding_percentage,
            powder.transform.position.z
        );
    }

    // Start is called before the first frame update
    void Start() { 
        ResetMinigame();
    }

    // Update is called once per frame
    void Update()
    {
        Collider pestleCollider = pestle.GetComponent<Collider>();
        Collider rawCollider = raw.GetComponent<Collider>();

        // highest points of the mask and the raw ingredient
        float maskY = mask.transform.position.y + mask.transform.localScale.y / 2;
        float rawY = raw.transform.position.y + raw.transform.localScale.y / 2;

        if (
            grinding_percentage < 1
            && pestleCollider.bounds.Intersects(rawCollider.bounds)
            && maskY > rawY
        )
        {
            grinding_percentage += GRIND_SPEED;
            // update grind level of ingredient
            switch (grinding_percentage)
            {
                case float n when n < 0.3f:
                    ingredient.currentLevels.grindLevel = GrindLevel.None;
                    break;

                case float n when n < 0.6f:
                    ingredient.currentLevels.grindLevel = GrindLevel.Low;
                    break;

                case float n when n < 1:
                    ingredient.currentLevels.grindLevel = GrindLevel.Medium;
                    break;

                case float n when n == 1:
                    ingredient.currentLevels.grindLevel = GrindLevel.High;
                    break;
            }

            UpdatePosition();
        }
    }
}
