using UnityEngine;
public class IngredientInstruction
{
    public IngredientDetails.Material material;
    public ProcessingLevels requiredLevels;

    public IngredientInstruction(IngredientDetails.Material Material)
    {
        material = Material;
        requiredLevels = new ProcessingLevels(true);
    }
    public IngredientInstruction(IngredientDetails.Material Material, ProcessingLevels Levels)
    {
        material = Material;
        requiredLevels = Levels;
    }
}