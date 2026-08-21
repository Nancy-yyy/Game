using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScaleController : MonoBehaviour
{
    [Header("天平各部位")]
    public Transform scaleBeam;   // 拖入「天秤上_0」
    public Transform leftPan;     // 拖入「天秤左_0」
    public Transform rightPan;    // 拖入「天秤右_0」

    [Header("主角頭部 Sprite 與表情圖片")]
    public SpriteRenderer playerFaceRenderer; // 拖入左托盤上的主角 SpriteRenderer
    public Sprite faceNormal;     // 預設/平常表情
    public Sprite faceCry;        // A 方案：哭哭臉 (跪地大哭)
    public Sprite faceHesitate;   // B 方案：猶豫臉
    public Sprite faceThink;      // C 方案：思考臉 (think.png)
    public Sprite faceCollapse;   // D 方案：崩潰臉

    [Header("按鈕與卡片管理")]
    public Button resetBtn;       // 拖入「重新選擇」按鈕
    public DraggableCard2D[] allCards; // 拖入 A, B, C, D 四張卡片

    private Coroutine activeScaleRoutine;

    void Start()
    {
        if (scaleBeam == null) scaleBeam = this.transform;
        if (resetBtn != null) resetBtn.onClick.AddListener(ResetScaleAndCards);
        
        // 初始狀態：預設表情或思考臉
        SetPlayerFace(faceNormal != null ? faceNormal : faceThink);
    }

    // 接收卡片放入事件
    public void TriggerReaction(string type)
    {
        if (activeScaleRoutine != null) StopCoroutine(activeScaleRoutine);

        switch (type)
        {
            case "A": // 右邊直接沉到最底、主角頭哭哭臉
                SetPlayerFace(faceCry);
                activeScaleRoutine = StartCoroutine(RotateSmooth(-25f, 0.4f));
                break;

            case "B": // 搖擺不定最後維持平衡、主角頭猶豫臉
                SetPlayerFace(faceHesitate);
                activeScaleRoutine = StartCoroutine(WobbleAndBalance());
                break;

            case "C": // 搖擺不定最後維持平衡、主角頭思考臉
                SetPlayerFace(faceThink);
                activeScaleRoutine = StartCoroutine(WobbleAndBalance());
                break;

            case "D": // 右邊直接沉到最底、主角頭崩潰臉
                SetPlayerFace(faceCollapse);
                activeScaleRoutine = StartCoroutine(RotateSmooth(-25f, 0.4f));
                break;
        }
    }

    // 平滑旋轉過程 (非瞬間瞬移)
    private IEnumerator RotateSmooth(float targetZ, float duration)
    {
        float time = 0;
        float startZ = GetCurrentAngle();

        while (time < duration)
        {
            time += Time.deltaTime;
            float currentZ = Mathf.Lerp(startZ, targetZ, time / duration);
            ApplyScaleRotation(currentZ);
            yield return null;
        }
        ApplyScaleRotation(targetZ);
    }

    // 搖晃後回到平衡狀態
    private IEnumerator WobbleAndBalance()
    {
        float duration = 2.5f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            // 隨時間遞減的搖擺幅度 (從大到小)
            float damping = 1f - (time / duration);
            float currentZ = Mathf.Sin(time * 10f) * (15f * damping);
            ApplyScaleRotation(currentZ);
            yield return null;
        }
        ApplyScaleRotation(0f); // 回到水平平衡
    }

    // 應用旋轉，並確保左右托盤永遠垂直不歪斜
    private void ApplyScaleRotation(float zAngle)
    {
        scaleBeam.localEulerAngles = new Vector3(0, 0, zAngle);

        // 反向旋轉托盤，抵消橫桿傾斜
        if (leftPan != null) leftPan.localEulerAngles = new Vector3(0, 0, -zAngle);
        if (rightPan != null) rightPan.localEulerAngles = new Vector3(0, 0, -zAngle);
    }

    private float GetCurrentAngle()
    {
        float z = scaleBeam.localEulerAngles.z;
        return (z > 180) ? z - 360 : z;
    }

    private void SetPlayerFace(Sprite face)
    {
        if (playerFaceRenderer != null && face != null)
        {
            playerFaceRenderer.sprite = face;
        }
    }

    // 重新選擇：卡片彈回、天平歸零、表情復原
    public void ResetScaleAndCards()
    {
        if (activeScaleRoutine != null) StopCoroutine(activeScaleRoutine);
        activeScaleRoutine = StartCoroutine(RotateSmooth(0f, 0.3f));
        SetPlayerFace(faceNormal != null ? faceNormal : faceThink);

        if (allCards != null)
        {
            foreach (var card in allCards)
            {
                if (card != null) card.ResetToOriginalPos();
            }
        }
    }
}