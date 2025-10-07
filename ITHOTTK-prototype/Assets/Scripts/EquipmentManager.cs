using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public GameObject grappleHookScriptObject;
    public GameObject slingshotScriptObject;

    public Image uiIcon;
    public Sprite grappleIcon;
    public Sprite slingshotIcon;
    public Sprite emptyIcon;

    private enum Weapon { None, Grapple, Slingshot }
    private Weapon currentWeapon = Weapon.None;

    private GrapplingHook grappleHook;

    void Start()
    {
        grappleHook = grappleHookScriptObject.GetComponent<GrapplingHook>();
        EquipNone();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeapon == Weapon.Grapple)
                EquipNone();
            else
                EquipGrapple();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentWeapon == Weapon.Slingshot)
                EquipNone();
            else
                EquipSlingshot();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipNone();
        }
    }

    void EquipGrapple()
    {
        currentWeapon = Weapon.Grapple;
        grappleHookScriptObject.SetActive(true);
        slingshotScriptObject.SetActive(false);
        UpdateUI(grappleIcon);
    }

    void EquipSlingshot()
    {
        currentWeapon = Weapon.Slingshot;
        grappleHookScriptObject.SetActive(false);
        slingshotScriptObject.SetActive(true);
        UpdateUI(slingshotIcon);
    }

    void EquipNone()
    {
        if (grappleHook != null)
            grappleHook.StopGrapple();

        currentWeapon = Weapon.None;
        grappleHookScriptObject.SetActive(false);
        slingshotScriptObject.SetActive(false);
        UpdateUI(emptyIcon);
    }

    void UpdateUI(Sprite icon)
    {
        if (uiIcon != null)
            uiIcon.sprite = icon;
    }
}