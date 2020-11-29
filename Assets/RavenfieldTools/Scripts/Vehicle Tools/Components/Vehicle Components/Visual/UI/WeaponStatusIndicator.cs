using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusIndicator : MonoBehaviour {

    public Weapon weapon;

    public bool ignoreUnholster = true;
    public bool ignoreCooldown = true;

    public Text textIndicator;
    public string readyText = "";
    public string notReadyText = "";
    public string reloadText = "";

    public Graphic[] tintTargets;
    public Color readyColor;
    public Color notReadyColor;

    public GameObject readyObject;
    public GameObject notReadyObject;

    public void Update() {
        if(this.weapon != null) {

            bool weaponReady = !this.weapon.reloading && (this.ignoreUnholster || this.weapon.unholstered) && (this.ignoreCooldown || this.weapon.CoolingDown());

            if(this.textIndicator != null) {
                if(this.weapon.reloading) {
                    this.textIndicator.text = this.reloadText;
                }
                else {
                    this.textIndicator.text = weaponReady ? this.readyText : this.notReadyText;
                }
            }

            Color c = weaponReady ? this.readyColor : this.notReadyColor;

            foreach(var target in this.tintTargets) {
                target.color = c;
            }

            if(this.readyObject != null) {
                this.readyObject.SetActive(weaponReady);
            }
            if(this.notReadyObject != null) {
                this.notReadyObject.SetActive(!weaponReady);
            }
        }
    }

}
