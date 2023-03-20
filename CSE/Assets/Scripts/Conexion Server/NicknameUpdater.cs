using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Hexstar;
using UnityEngine.Networking;

public class NicknameUpdater : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentNickname;
    [SerializeField] TMP_InputField inputField;

    private async void Start()
    {
        await SesionHandler.GetNickname();
        currentNickname.text = SesionHandler.nickname;
    }

    public void ChangeNickname()
    {
        if (inputField.text == "") return;
        string url = ConexionHandler.baseUrl + "nickname";
        WWWForm form = new WWWForm();
        form.AddField("authorization",SesionHandler.sessionKEY);
        form.AddField("email", SesionHandler.email);
        form.AddField("nickname", inputField.text);
        _ = ConexionHandler.APost(url, form);
        ConexionHandler.onFinishRequest.AddListener(SetNick);
    }

    private async void SetNick(DownloadHandler download)
    {
        ConexionHandler.onFinishRequest.RemoveListener(SetNick);
        string data = ConexionHandler.ExtraerJson(download.text);
        if (data == "{}") return;
        await SesionHandler.GetNickname();
        currentNickname.text = SesionHandler.nickname;
    }
}
