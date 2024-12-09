using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    int index;
    public Image[] hotbarHighLights;
    BlockPlacer blockPlacer;

    int lastIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockPlacer = FindFirstObjectByType<BlockPlacer>();
    }

    // Update is called once per frame
    void Update()
    {
        index = blockPlacer.prefabindex;
        ChangeColor();
    }

    void ChangeColor() 
    {
        hotbarHighLights[index].color = Color.yellow;
        if (lastIndex != index) 
        {
            hotbarHighLights[lastIndex].color = Color.white;
            lastIndex = index;
        }   
    }

}
