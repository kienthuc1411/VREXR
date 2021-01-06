using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dropdownlist : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TMP_Dropdown dropdown = transform.GetComponent<TMP_Dropdown>();
        dropdown.options.Clear();

        List<string> items = new List<string>();
        items.Add("Select item");
        items.Add("Items 1");
        items.Add("Items 2");
        foreach (var item in items)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = item });
        }

        //DropdownItemSelect(dropdown);
        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelect(dropdown); });
    }

    private void DropdownItemSelect(TMP_Dropdown dropdown)
    {
        int index = dropdown.value;
    }

    
}
