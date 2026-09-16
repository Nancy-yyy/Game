using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseTest : MonoBehaviour
{
    // Supabase Project URL
    private const string SUPABASE_URL =
        "https://zpstusybpumuzwkfycnj.supabase.co";

    // 只能放 Publishable Key
    private const string SUPABASE_KEY =
        "sb_publishable_kWJ0uwaRshrjOfLAL80Kiw_HmLILVEN";

    void Start()
    {
        StartCoroutine(TestInsert());
    }

    IEnumerator TestInsert()
    {
        string url =
            SUPABASE_URL + "/rest/v1/experiment_results";

        string json =
            "{" +
            "\"subject_id\":\"P002\"," +
            "\"condition\":\"LowInfo_AI\"" +
            "}";

        UnityWebRequest request =
            new UnityWebRequest(url, "POST");

        byte[] bodyRaw =
            System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        request.SetRequestHeader(
            "apikey",
            SUPABASE_KEY
        );

        request.SetRequestHeader(
            "Prefer",
            "return=minimal"
        );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(
                "Supabase 測試成功！P002 已寫入資料庫"
            );
        }
        else
        {
            Debug.LogError(
                "Supabase 測試失敗\n" +
                "HTTP Status: " + request.responseCode + "\n" +
                "Error: " + request.error + "\n" +
                "Response: " + request.downloadHandler.text
            );
        }
    }
}