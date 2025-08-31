using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class messageregister : MonoBehaviour
{
    [SerializeField] TMP_Dropdown reciever;

    private int victimSelect;
    public void onRecipientSelect(){
        if(reciever.itemText.Equals("User 1")){
            victimSelect = (int)recipient.User1;
        }else{
            victimSelect = (int)inc.other;
        }
    }

    public void onSend(){
        if(victimSelect == 0){
            FileGenerator.ADActual += 1;
        }
        FileGenerator.ADTotal = 1;
    }

}
public enum recipient{
    User1 = 0,
    other = 1
}

