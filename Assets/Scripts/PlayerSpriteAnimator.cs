using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController player;
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField] private Sprite[] runFrames;
    [SerializeField] private Sprite[] jumpFrames;
    [SerializeField] private Sprite[] fallFrames;
    [SerializeField] private float framesPerSecond = 8f;

    private Rigidbody2D body;
    private Sprite[] currentFrames;
    private float frameTimer;
    private int frameIndex;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
            player = GetComponent<PlayerController>();

        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (spriteRenderer == null || player == null || body == null)
            return;

        if (player.HorizontalInput < 0f)
            spriteRenderer.flipX = true;
        else if (player.HorizontalInput > 0f)
            spriteRenderer.flipX = false;

        Sprite[] nextFrames = GetFramesForCurrentState();
        if (nextFrames == null || nextFrames.Length == 0)
            return;

        if (currentFrames != nextFrames)
        {
            currentFrames = nextFrames;
            frameIndex = 0;
            frameTimer = 0f;
            spriteRenderer.sprite = currentFrames[frameIndex];
            return;
        }

        if (currentFrames.Length == 1)
            return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / Mathf.Max(1f, framesPerSecond))
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % currentFrames.Length;
            spriteRenderer.sprite = currentFrames[frameIndex];
        }
    }

    private Sprite[] GetFramesForCurrentState()
    {
        if (!player.IsGrounded)
            return body.velocity.y > 0.1f ? jumpFrames : fallFrames;

        if (Mathf.Abs(player.HorizontalInput) < 0.01f)
            return idleFrames;

        return player.IsSprinting ? runFrames : walkFrames;
    }
}
