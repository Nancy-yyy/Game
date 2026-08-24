using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GlobalCursorManager : MonoBehaviour
{
    public static GlobalCursorManager Instance { get; private set; }

    [Header("預設游標 (留空即為系統預設箭頭)")]
    public Texture2D defaultCursor;

    [Header("懸停在可點選物件時的游標 (留空會自動生成小手)")]
    public Texture2D hoverCursor;

    [Header("游標點擊熱點")]
    public Vector2 hotSpot = new Vector2(6, 2);

    private bool isHoveringInteractive = false;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (hoverCursor == null)
        {
            hoverCursor = CreateProceduralHandCursor();
        }

        SetDefaultCursor();
    }

    void Update()
    {
        if (EventSystem.current == null) return;
        if (Mouse.current == null) return;

        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        // 純新版 Input System 讀取滑鼠座標
        Vector2 mousePos = Mouse.current.position.ReadValue();

        pointerEventData.position = mousePos;
        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        bool foundInteractive = false;

        foreach (var result in raycastResults)
        {
            GameObject hitObj = result.gameObject;

            Selectable selectable = hitObj.GetComponentInParent<Selectable>();
            if (selectable != null && selectable.interactable && selectable.gameObject.activeInHierarchy)
            {
                foundInteractive = true;
                break;
            }

            if (hitObj.GetComponentInParent<IPointerClickHandler>() != null)
            {
                foundInteractive = true;
                break;
            }
        }

        if (foundInteractive && !isHoveringInteractive)
        {
            isHoveringInteractive = true;
            if (hoverCursor != null)
            {
                Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
            }
        }
        else if (!foundInteractive && isHoveringInteractive)
        {
            isHoveringInteractive = false;
            SetDefaultCursor();
        }
    }

    private void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private Texture2D CreateProceduralHandCursor()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        Color clear = new Color(0, 0, 0, 0);
        Color white = Color.white;
        Color black = Color.black;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, clear);

        int[,] handMap = new int[,]
        {
            {0,0,1,1,0,0,0,0},
            {0,1,2,2,1,0,0,0},
            {0,1,2,2,1,0,0,0},
            {0,1,2,2,1,0,0,0},
            {0,1,2,2,1,1,1,0},
            {1,1,2,2,2,2,2,1},
            {1,2,2,2,2,2,2,1},
            {1,2,2,2,2,2,2,1},
            {0,1,2,2,2,2,1,0},
            {0,0,1,1,1,1,0,0}
        };

        int rows = handMap.GetLength(0);
        int cols = handMap.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int val = handMap[r, c];
                int py = size - 1 - (r * 2);
                int px = c * 2 + 4;

                Color cColor = val == 1 ? black : (val == 2 ? white : clear);
                if (val != 0)
                {
                    tex.SetPixel(px, py, cColor);
                    tex.SetPixel(px + 1, py, cColor);
                    tex.SetPixel(px, py - 1, cColor);
                    tex.SetPixel(px + 1, py - 1, cColor);
                }
            }
        }

        tex.Apply();
        return tex;
    }
}