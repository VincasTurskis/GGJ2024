using System;
using System.Collections;
using System.Collections.Generic;
using IngredientDetails;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cauldron : MonoBehaviour
{
    public List<IngredientDetails.Material> _materials = new();
    private MouseFollower _mouseFollower;
    public float perfectWeakPotionValue;
    public float strongPotionMultiplier;
    public float incorrectGrindStagePenalty;
    public float incorrectHeatStagePenalty;
    public float incorrectCutMultiplier;
    public float cutMarginOfError;
    public float incorrectSeverityMultiplier;
    public List<IngredientInstruction> ingredientInstructions = new ();
    private int currentIngredientIndex = 0;
    public List<Ingredient> receivedIngredients = new();
    private Camera _camera;
    private float finalResult = 0f;
    private ClientRequestGenerator requestGenerator;
    public TemperatureLevel temperatureLevel = TemperatureLevel.None;
    public TextMeshProUGUI temperatureText;

    void Awake()
    {
        _mouseFollower = FindObjectOfType<MouseFollower>();
        _camera = Camera.main;
        UnityEngine.Object[] scripObjects = Resources.LoadAll("", typeof(IngredientDetails.Material));
        for(int i = 0; i < scripObjects.Length; i++)
        {
            _materials.Add((IngredientDetails.Material)scripObjects[i]);
        }
        requestGenerator = new ClientRequestGenerator();
        SetExpectedIngredients(requestGenerator.generateRequest(SpecialClientType.None));
    }
    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePosition = mouse.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            var raycastHits = Physics.RaycastAll(ray);
            foreach(var rhit in raycastHits)
            {
                if(rhit.collider.gameObject == gameObject)
                {
                    if(_mouseFollower.ingredientBeingCarried != null)
                    {
                        AddIngredient(_mouseFollower.ingredientBeingCarried);
                    }
                }
            }
        }
        switch (temperatureLevel)
        {
            case TemperatureLevel.None:
                temperatureText.text = "Current Temp: None";
                break;
            case TemperatureLevel.Low:
                temperatureText.text = "Current Temp: Low";
                break;
            case TemperatureLevel.Medium:
                temperatureText.text = "Current Temp: Medium";
                break;
            case TemperatureLevel.High:
                temperatureText.text = "Current Temp: High";
                break;
        }
        if(ingredientInstructions.Count == 0)
        {
            SetExpectedIngredients(requestGenerator.generateRequest(SpecialClientType.None));
        }
    }


    public void IncreaseTemperature()
    {
        switch (temperatureLevel)
        {
            case TemperatureLevel.None:
                temperatureLevel = TemperatureLevel.Low;
                break;
            case TemperatureLevel.Low:
            temperatureLevel = TemperatureLevel.Medium;
                break;
            case TemperatureLevel.Medium:
                temperatureLevel = TemperatureLevel.High;
                break;
            case TemperatureLevel.High:
                break;
        }
    }
    public void DecreaseTemperature()
    {
        switch (temperatureLevel)
        {
            case TemperatureLevel.None:
                break;
            case TemperatureLevel.Low:
                temperatureLevel = TemperatureLevel.None;
                break;
            case TemperatureLevel.Medium:
                temperatureLevel = TemperatureLevel.Low;
                break;
            case TemperatureLevel.High:
                temperatureLevel = TemperatureLevel.Medium;
                break;
        }
    }

    public void AddIngredient(Ingredient toAdd)
    {
        toAdd.currentLevels.temperatureLevel = temperatureLevel;
        if(currentIngredientIndex >= ingredientInstructions.Count) 
        {
            Debug.Log("Extra ingredient. Value = 0");
            receivedIngredients.Add(_mouseFollower.ingredientBeingCarried);
            _mouseFollower.ingredientBeingCarried.gameObject.transform.SetParent(null);
            _mouseFollower.ingredientBeingCarried.gameObject.SetActive(false);
            _mouseFollower.ingredientBeingCarried = null;
            return;
        }
        float result = EvaluateIngredient(ingredientInstructions[currentIngredientIndex], _mouseFollower.ingredientBeingCarried);
        currentIngredientIndex++;
        Debug.Log(result);
        finalResult += result;
        receivedIngredients.Add(_mouseFollower.ingredientBeingCarried);
        _mouseFollower.ingredientBeingCarried.gameObject.transform.SetParent(null);
        _mouseFollower.ingredientBeingCarried.gameObject.SetActive(false);
        _mouseFollower.ingredientBeingCarried = null;
    }

    public void FinishBrewing()
    {
        float toReturn = finalResult;
        ResetCauldron();
        Debug.Log(toReturn);
    }

    public void ResetCauldron()
    {
        ingredientInstructions = new();
        for(int i = 0; i < receivedIngredients.Count; i++)
        {
            Destroy(receivedIngredients[i].gameObject);
        }
        receivedIngredients = new();
        finalResult = 0f;
        currentIngredientIndex = 0;
    }

    public void SetExpectedIngredients(ClientRequest request)
    {
        ResetCauldron();
        for(int i = 0; i < request.Request_required_ingredients.Count; i++)
        {
            bool found = false;
            var (Emotion, Severity) = request.Request_required_ingredients[i];
            for(int j = 0; j < _materials.Count; j++)
            {
                if(_materials[j].emotion == Emotion && _materials[j].severity == Severity)
                {
                    ingredientInstructions.Add(new IngredientInstruction(_materials[j]));
                    found = true;
                    break;
                }
            }
            if(found == false)
            {
                Debug.Log("ERROR: COULD NOT FIND MATERIAL: " + Emotion + " " + Severity);
            }
        }
    }

    public float EvaluateIngredient(IngredientInstruction expected, Ingredient received)
    {

        float result = perfectWeakPotionValue;
        // If the emotions don't match, no reward
        if(expected.material.emotion != received.material.emotion) return 0;
        result -= Math.Abs(
            (int)expected.requiredLevels.grindLevel - (int)received.currentLevels.grindLevel
        ) * incorrectGrindStagePenalty;

        result -= Math.Abs(
            (int)expected.requiredLevels.temperatureLevel - (int)received.currentLevels.temperatureLevel
        ) * incorrectHeatStagePenalty;

        if(Math.Abs(expected.requiredLevels.cuttingLevel - received.currentLevels.cuttingLevel) > cutMarginOfError)
        {
            result -= Math.Abs(expected.requiredLevels.cuttingLevel - received.currentLevels.cuttingLevel) * incorrectCutMultiplier;
        }
        if(result < 0) return 0;
        if(expected.material.severity != received.material.severity)
        {
            result *= incorrectSeverityMultiplier;
        }
        else if(expected.material.severity == Severity.High)
        {
            result *= strongPotionMultiplier;
        }
        return result;
    }
}
