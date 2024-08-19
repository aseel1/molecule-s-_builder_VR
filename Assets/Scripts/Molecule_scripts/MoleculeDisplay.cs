using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoleculeDisplay : MonoBehaviour
{
    public TextMeshProUGUI formulaText;
    private MoleculeGroup moleculeGroup;

    private void Start()
    {
        formulaText = GameObject.Find("Fourmla_text").GetComponent<TextMeshProUGUI>();

        moleculeGroup = GetComponent<MoleculeGroup>();
        UpdateFormulaText();

        
    }

    // Update the formula text
    private void UpdateFormulaText()
    {
        if (moleculeGroup != null && formulaText != null)
        {
            formulaText.text = moleculeGroup.GetMolecularFormula();
            Debug.Log("Updated Formula Text: " + formulaText.text); // Print the formula text to the console

        }
    }

    // Call this method whenever the molecule group changes
    public void OnMoleculeGroupChanged()
    {
        UpdateFormulaText();
    }
}
