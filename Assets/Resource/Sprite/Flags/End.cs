using UnityEngine;

public class End : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject[] otherPanels;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            endPanel.SetActive(true);
            foreach (GameObject panel in otherPanels)
            {
                panel.SetActive(false);
            }
        }
    }
}