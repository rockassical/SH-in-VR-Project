using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reportregister : MonoBehaviour
{
    [SerializeField] TMP_Dropdown incident, offender, victim;

    private int incidentSelect, offenderSelect, victimSelect;
    public void onIncidentSelect(){
        if(incident.itemText.Equals("Sexual Harrasment")){
            incidentSelect = (int)inc.SexualHarrasment;
        }else{
            incidentSelect = (int)inc.other;
        }
    }

    public void onOffenderSelect(){
        if(incident.itemText.Equals("Sexual Harrasment")){
            offenderSelect = (int)of.offender;
        }else{
            offenderSelect = (int)of.other;
        }
    }

    public void OnReport(){
        if(incident.itemText.Equals("Sexual Harrasment")){
            victimSelect = (int)vic.victim;
        }else{
            victimSelect = (int)vic.other;
        }
        if(incidentSelect+offenderSelect+victimSelect == 0){
            FileGenerator.ARActual += 1;            
        }
        FileGenerator.ARTotal += 1;
    }
}
public enum inc{
    SexualHarrasment = 0,
    other = 1
}
public enum of{
    offender = 0,
    other = 1
}
public enum vic{
    victim = 0,
    other = 1
}