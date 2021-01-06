using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AvatarEditViewTED : MonoBehaviour
{
    public GameObject loading;
    public RawImage avatar;
    public List<Transform> lstAvatarItemDefault;
    public Dropdown dropOptionListItem; 
    public ToggleGroup toggleGroupItem;
    private Dictionary<string, Texture> lstAvatarLocal;

    public int AvatarIDSelected;

    private AvatarsListResponse avatarsListResponse;
    [SerializeField]
    private GameObject statusLabel;

    // Start is called before the first frame update
    void Start()
    {
        lstAvatarLocal = new Dictionary<string, Texture>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnEnable()
    {
        getListAvatarFromServerAsync();
    }
    public async void getListAvatarFromServerAsync()
    {
        loading.SetActive(true);
        avatarsListResponse = await Request.GetAvatarsList();
        processAvatarListRespone();
        loading.SetActive(false);
    }
    void processAvatarListRespone()
    {

        //set up for avatar Item
        for (int i = 0; i < lstAvatarItemDefault.Count; i++)
        {
            lstAvatarItemDefault[i].GetComponent<AvatarItem_TED>().avatar = avatar;
            lstAvatarItemDefault[i].GetComponent<AvatarItem_TED>().avatarURL = avatarsListResponse.default_data.data[i].image;
            lstAvatarItemDefault[i].GetComponent<AvatarItem_TED>().avatarID = avatarsListResponse.default_data.data[i].id;
        }
        //set list item cho dropdownlist
        setUpListOptionItem();

    }

    private void setUpListOptionItem()
    {
        dropOptionListItem.options.Clear();
        List<string> lstItem = new List<string>();
        lstItem.Add("Option A");
        string utf8_String = "アバター0";
        byte[] bytes = Encoding.Default.GetBytes(utf8_String);
        utf8_String = Encoding.UTF8.GetString(bytes);
        for (int i = 0; i < avatarsListResponse.user_data.data.Length; i++)
        {
            lstItem.Add(utf8_String + (i + 1));
        }
        //
        foreach (var item in lstItem)
        {
            dropOptionListItem.options.Add(new Dropdown.OptionData() { text = item });
        }
        dropOptionListItem.onValueChanged.AddListener(delegate { DropdownItemSelect(dropOptionListItem); });

    }

    public async void DropdownItemSelect(Dropdown dropdown)
    {
        int index = dropdown.value;
        if (index > 0)
        {
            Characters character = avatarsListResponse.user_data.data[index - 1];
            //show loading status
            avatar.transform.GetChild(0).gameObject.SetActive(true);


            //check avatar luu chua
            if (lstAvatarLocal.ContainsKey(character.id.ToString()))
                avatar.texture = lstAvatarLocal[character.id.ToString()];
            else
            {
                if (!string.IsNullOrEmpty(character.image))
                {
                    avatar.texture = await Request.DownloadImage(character.image);
                    lstAvatarLocal.Add(character.id.ToString(), avatar.texture);
                }
            }

            //hide loading status
            avatar.transform.GetChild(0).gameObject.SetActive(false);

            //resest toogleGroup unselect
            Toggle theActiveToggle = toggleGroupItem.ActiveToggles().FirstOrDefault(o => o.isOn == true);
            if (theActiveToggle != null)
                theActiveToggle.isOn = false;

            //set avatarid Select
            AvatarIDSelected = character.id;

            //dropdown.Hide();
        }

    }

    public async void UpdateAvatarToServer()
    {
        int avatarID = AvatarIDSelected;
        loading.SetActive(true);
        long result = await Request.UpdateAvatar(avatarID, PlayerData.profile.id);
        loading.SetActive(false);
    }
    void SetStatusLabel(string text)
    {
        statusLabel.gameObject.SetActive(true);
        statusLabel.GetComponentInChildren<Text>().text = text;
        Invoke("hidelogoutInfo", 1f);
    }
    void hidelogoutInfo()
    {
        statusLabel.SetActive(false);
    }

}
