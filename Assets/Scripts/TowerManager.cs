using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class TowerManager : MonoBehaviour
{
    private struct BlockData
    {
        public Vector3 scale;
        public Color color;
    }
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform platformPosition;
    private GameObject previewBlock;
    private BlockData nextBlockData;
    [SerializeField] private Transform tower;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float spawnHeightOffset = 1f;
    [SerializeField] private float maxTiltAngle = 30f;
    private bool allowTouch = false; 
    private Transform lastBlock;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameStartPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI ingameScoreText;
    private int currentHeight = 0; 

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        gameStartPanel.SetActive(true);
    }

    private BlockData GenerateNextBlockData()
    {
        BlockData data;

        data.scale = new Vector3(
            Random.Range(1f, 2f),
            Random.Range(0.8f, 1f),
            Random.Range(1f, 2f)
        );

        data.color = Random.ColorHSV();

        return data;
    }

    void PlaceBlock(Vector3 position)
    {
        GameObject newBlock = Instantiate(blockPrefab, position + Vector3.up * spawnHeightOffset, Quaternion.identity, tower);
        newBlock.transform.localScale = nextBlockData.scale;
        newBlock.GetComponent<Renderer>().material.color = nextBlockData.color;
        newBlock.tag = "block";
        lastBlock = newBlock.transform;

        // Prepare next preview
        nextBlockData = GenerateNextBlockData();

        if (previewBlock != null)
            Destroy(previewBlock);

        // previewBlock = Instantiate(blockPrefab);
        float highestY = GetHighestBlockYPosition();
        Vector3 previewPosition = new Vector3(0f, highestY + spawnHeightOffset, 0f);
        previewBlock = Instantiate(blockPrefab, previewPosition, Quaternion.identity);
        SetupPreviewBlock(previewBlock, nextBlockData);

        currentHeight++;
        UpdateScoreText(currentHeight);
    }

    private void UpdateScoreText(int height)
    {
        if (scoreText != null)
        {
            ingameScoreText.text = $"Score: {currentHeight}";
        }
    }
    private float GetHighestBlockYPosition()
    {
        float highestY = 0f;

        foreach (Transform child in tower)
        {
            if (child != null)
            {
                float blockTopY = child.position.y + (child.localScale.y / 2f);
                if (blockTopY > highestY)
                {
                    highestY = blockTopY;
                }
            }
        }

        return highestY;
    }

    void SetupPreviewBlock(GameObject block, BlockData data)
    {
        block.transform.localScale = data.scale;

        var renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color previewColor = data.color;
            previewColor.a = 0.5f;
            renderer.material.color = previewColor;
        }

        var rb = block.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        var collider = block.GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        block.transform.rotation = Quaternion.identity;
    }

    bool GetRaycastHitPoint(out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        #if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        #endif

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            Debug.DrawRay(hit.point, hit.normal * 2f, Color.red, 1f);
            Debug.Log($"Hit normal: {hit.normal}, angle: {Vector3.Angle(hit.normal, Vector3.up)}");
            if (Vector3.Angle(hit.normal, Vector3.up) <= 30f) 
            {
                if (hit.collider.CompareTag("block") || hit.collider.CompareTag("platform"))
                {
                    hitPoint = hit.point;
                    return true;
                }
            }
        }

        hitPoint = hit.point;
        return false;
    }
    void Update()
    {
        if (allowTouch == false)
        {
            return; 
        }

        Vector3 hitPoint;
        bool validHit = GetRaycastHitPoint(out hitPoint);

        // previewBlock.transform.position = hitPoint + Vector3.up * spawnHeightOffset;
        // previewBlock.transform.rotation = Quaternion.identity;

        // only place a block (and move preview block) if we hit a valid surface
        if (validHit)
        {
            // touch input
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && allowTouch)
            {
                PlaceBlock(hitPoint);
            }
            // mouse input
            else if (Input.GetMouseButtonDown(0) && allowTouch)
            {
                PlaceBlock(hitPoint);
            }
        }

        if (lastBlock == null)
        {    
            return; 
        }

        CheckGameOver(lastBlock); 
    }

    private void CheckGameOver(Transform lastBlock)
    {
        float angle = Vector3.Angle(Vector3.up, lastBlock.up);

        if (angle > maxTiltAngle)
        {
            Destroy(previewBlock);
            
            allowTouch = false;

            scoreText.text = $"Your Score: {currentHeight}";

            Debug.Log("Tower collapsed!");

            // wait for 2 seconds before collapsing
            StartCoroutine(CollapseTower(2f));

            lastBlock = null; 
        }
    }

    private IEnumerator CollapseTower(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Transform child in tower)
        {
            Destroy(child.gameObject);
        }
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
    
    
    public void PlayGame()
    {
        currentHeight = 0; 
        
        UpdateScoreText(currentHeight);

        gameStartPanel.SetActive(false);

        allowTouch = true;
        

        if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);

        gamePanel.SetActive(true);

        nextBlockData = GenerateNextBlockData();
        
        if (previewBlock != null)
            Destroy(previewBlock);

        // previewBlock = Instantiate(blockPrefab);
        // spawn over the platform
        previewBlock = Instantiate(blockPrefab, platformPosition.position + Vector3.up * spawnHeightOffset, Quaternion.identity);
        SetupPreviewBlock(previewBlock, nextBlockData);
    }
}
