using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();

        foreach(var dropdown in dropdowns){
            dropdown.options.Clear();

            List<string> items = new List<string>();
            items.Add("level 0");
            items.Add("level 1");
            items.Add("level 2");
            items.Add("level 3");

            //Fill Dropdown with items
            foreach(var item in items){
                dropdown.options.Add(new Dropdown.OptionData() {text = item});
            }

            DropdownItemSelected(dropdown);

            dropdown.onValueChanged.AddListener(delegate {DropdownItemSelected(dropdown);});
        }
    }

    void DropdownItemSelected(Dropdown drd){
        int index = drd.value;
    }

}
