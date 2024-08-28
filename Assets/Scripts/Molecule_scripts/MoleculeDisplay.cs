using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class MoleculeDisplay : NetworkBehaviour
{
    public TextMeshProUGUI formulaText;
    private MoleculeGroup moleculeGroup;

    // This method will be called from the MoleculeGroup after the formula panel is instantiated
    public void InitializeDisplay(TextMeshProUGUI formulaText)
    {
        this.formulaText = formulaText;
        moleculeGroup = GetComponent<MoleculeGroup>();
        UpdateFormulaText();
    }


    // Update the formula text
    private void UpdateFormulaText()
    {
        if (moleculeGroup != null && formulaText != null)
        {
            formulaText.text = moleculeGroup.GetMolecularFormula();

        }
    }

    // Call this method whenever the molecule group changes
    public void OnMoleculeGroupChanged()
    {
        UpdateFormulaText();
    }
}
