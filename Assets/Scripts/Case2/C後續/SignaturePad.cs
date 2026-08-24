using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SignaturePad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("繪圖區域設定")]
    public RawImage drawArea;       // 拖入 DrawArea (RawImage)
    public int textureWidth = 600;  // 簽名板解析度寬
    public int textureHeight = 300; // 簽名板解析度高
    public Color penColor = new Color(0.15f, 0.25f, 0.15f, 1f); // 墨水顏色
    public int penBrushSize = 4;    // 筆觸粗細

    [Header("功能按鈕")]
    public Button clearBtn;         // 拖入 ClearBtn
    public Button confirmBtn;       // 拖入 ConfirmSignBtn

    private Texture2D signTexture;
    private RectTransform rectTransform;
    private Vector2? lastDrawPos = null;
    public bool hasSigned { get; private set; } = false;

    void Awake()
    {
        if (drawArea != null)
        {
            rectTransform = drawArea.GetComponent<RectTransform>();
            InitTexture();
        }

        if (clearBtn != null)
        {
            clearBtn.onClick.RemoveAllListeners();
            clearBtn.onClick.AddListener(ClearPad);
        }
    }

    private void InitTexture()
    {
        signTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        signTexture.filterMode = FilterMode.Bilinear;
        ClearPad();
        drawArea.texture = signTexture;
    }

    public void ClearPad()
    {
        if (signTexture == null) return;

        Color[] clearColors = new Color[textureWidth * textureHeight];
        for (int i = 0; i < clearColors.Length; i++) clearColors[i] = Color.clear;

        signTexture.SetPixels(clearColors);
        signTexture.Apply();
        hasSigned = false;
        lastDrawPos = null;
    }

    // 當手指/滑鼠按下：開啟新筆畫，不連線
    public void OnPointerDown(PointerEventData eventData)
    {
        lastDrawPos = null;
        DrawAtPointer(eventData);
    }

    // 當手指/滑鼠拖曳：平滑補線
    public void OnDrag(PointerEventData eventData)
    {
        DrawAtPointer(eventData);
    }

    // 當手指/滑鼠放開：強制中斷筆劃，避免與下一筆相連
    public void OnPointerUp(PointerEventData eventData)
    {
        lastDrawPos = null;
    }

    private void DrawAtPointer(PointerEventData eventData)
    {
        if (rectTransform == null || signTexture == null) return;

        Vector2 localPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPos))
        {
            float px = (localPos.x / rectTransform.rect.width + 0.5f) * textureWidth;
            float py = (localPos.y / rectTransform.rect.height + 0.5f) * textureHeight;

            Vector2 currentPos = new Vector2(px, py);

            if (lastDrawPos.HasValue)
            {
                DrawLine(lastDrawPos.Value, currentPos);
            }
            else
            {
                DrawCircle((int)px, (int)py, penBrushSize);
            }

            signTexture.Apply();
            lastDrawPos = currentPos;
            hasSigned = true;
        }
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        int steps = (int)Vector2.Distance(from, to);
        if (steps <= 0)
        {
            DrawCircle((int)from.x, (int)from.y, penBrushSize);
            return;
        }

        for (int i = 0; i <= steps; i++)
        {
            Vector2 point = Vector2.Lerp(from, to, (float)i / steps);
            DrawCircle((int)point.x, (int)point.y, penBrushSize);
        }
    }

    private void DrawCircle(int cx, int cy, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px >= 0 && px < textureWidth && py >= 0 && py < textureHeight)
                    {
                        signTexture.SetPixel(px, py, penColor);
                    }
                }
            }
        }
    }

    public Texture2D GetSignatureTexture()
    {
        return signTexture;
    }
}