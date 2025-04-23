using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class TowerManager : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform tower;

    [SerializeField] private Camera mainCamera;
    
    [SerializeField] private float spawnHeightOffset = 1f;

    [SerializeField] private float maxTiltAngle = 30f;

    private bool allowTouch = false; 

    private Transform lastBlock;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameStartPanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentHeight = 0; 

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    private void generateNewBlock(Ray ray)
    {

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 spawnPos = hit.point + Vector3.up * spawnHeightOffset;

            GameObject newBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

            lastBlock = newBlock.transform;
            newBlock.transform.parent = tower;

            float scaleX = Random.Range(1f, 2f); //random scale
            float scaleY = Random.Range(0.8f, 1f);
            float scaleZ = Random.Range(1f, 2f);
            newBlock.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            Renderer rend = newBlock.GetComponent<Renderer>(); // random color
            rend.material.color = Random.ColorHSV();

            currentHeight++;
            
            scoreText.text = $"Your Score: {currentHeight}";
        }
    }
    void Update()
    {
        // touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && allowTouch)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            generateNewBlock(ray);
        }
        // mouse input
        else if (Input.GetMouseButtonDown(0) && allowTouch)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            generateNewBlock(ray);
        }

        if (lastBlock == null)
        {    
            return; 
        }
        
        float angle = Vector3.Angle(Vector3.up, lastBlock.up);

        if (angle > maxTiltAngle)
        {
            
            allowTouch = false;
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
        
        gameOverPanel.SetActive(true);
    }
    
    public void RestartGame()
    {
        gameOverPanel.gameObject.SetActive(false);
        allowTouch = true; 
        currentHeight = 0; 
    }

    public void PlayGame()
    {
        gameStartPanel.SetActive(false);
        allowTouch = true;
    }
}
