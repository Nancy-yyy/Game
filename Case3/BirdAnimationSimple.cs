using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdAnimationSimple : MonoBehaviour
{
    [Header("放鳥鳥的動作圖 (依序放入 bird_0, bird_jump_0)")]
    public List<Sprite> birdFrames;

    [Header("切換間隔時間 (秒)")]
    public float frameRate = 0.4f;

    private Image birdImage;
    private int currentFrame = 0;
    private float timer = 0f;

    void Awake()
    {
        birdImage = GetComponent<Image>();
    }

    void Update()
    {
        if (birdFrames == null || birdFrames.Count == 0 || birdImage == null) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % birdFrames.Count;
            birdImage.sprite = birdFrames[currentFrame];
        }
    }
}