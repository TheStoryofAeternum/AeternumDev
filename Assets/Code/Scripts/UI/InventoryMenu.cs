using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 
/// Captures player input to determine view state of 
/// dynamically drawn inventory based on number of 
/// slots available.
/// <para>
/// TODO: Populate slots with icons representing
/// item data. Drag and drop.
/// </para>
/// <para>
/// TODO: Mouse drag window position
/// </para>
/// </summary>
public class InventoryMenu : MonoBehaviour
{
    // Member fields
    [SerializeField] int _count;
    [SerializeField] float _tileWidth, _tileHeight;
    [SerializeField] float _tilePad;
    [SerializeField] float _inventoryPad;
    [SerializeField] bool _isActive;

    GameObject _canvasObject;
    List<GameObject> _inventoryObject;
    Canvas _canvas;

    void Start() 
    {
        _inventoryObject = new List<GameObject>();
        _isActive = false;
        InitMenu();
    }

    void Update()
    {
        GetInput();
    }

    /// <summary>
    /// Initializes parent canvas object.
    /// </summary>
    void InitMenu()
    {
        _canvasObject = new GameObject();
        _canvasObject.name = "par_Canvas";

        _canvas = _canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    /// <summary>
    /// Checks for player input to change view state of
    /// inventory menu.
    /// </summary>
    void GetInput()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(!_isActive)
            {
                _isActive = true;
                DrawMenu(_count, Vector2.zero);
                //DrawMenu(10, new Vector2(50, -50));
                return;
            }

            _isActive = false;
            CloseMenu();
        }
    }

    /// <summary>
    /// Dynamically draws inventory menu based on slot count with images 
    /// representing contents.
    /// </summary>
    void DrawMenu(int count, Vector2 position) 
    {
        // Create empty inventory parent object and set as child of canvas.
        GameObject _tempInventory = new GameObject();
        _tempInventory.name = "par_InventoryMenu";
        _tempInventory.transform.SetParent(_canvasObject.transform);
        _inventoryObject.Add(_tempInventory);

        // Create visual asset for inventory.
        Image inventoryPanel = _tempInventory.AddComponent<Image>();
        RectTransform inventoryRect = inventoryPanel.GetComponent<RectTransform>();

        int height = 0;
        int width = (int)Mathf.Sqrt(count);

        // Do until elements run out.
        while(count > 0)
        {
            // Create a row of tiles dependent on max width of menu.
            for(int x = 0; x < width && count > 0; x++)
            {
                CreateTile(_tempInventory.transform, x, height);
                count--;
            }
            height++;
        }

        // Size inventory rect according to tile format.
        inventoryRect.sizeDelta = new Vector2(
            width * (_tileWidth + _tilePad) + _inventoryPad / 2f, 
            height * (_tileHeight + _tilePad) + _inventoryPad / 2f + 20f
        );
        inventoryRect.localPosition = position;
    }

    /// <summary>
    /// Creates a tile visual asset as child of inventory menu.
    /// <para>
    /// TODO: Represent tiles as button element to be interactable.
    /// </para>
    /// </summary>
    void CreateTile(Transform parent, int x, int y)
    {
        GameObject tileObject = new GameObject();
        
        tileObject.name = $"img_Tile_{x}_{y}";
        tileObject.transform.SetParent(parent);
        
        // Add visual asset to tile object.
        Image tile = tileObject.AddComponent<Image>();
        
        tile.color = Color.black;

        RectTransform tileRect = tile.GetComponent<RectTransform>();

        // Set position and dimensions of tile.
        tileRect.anchorMin = Vector2.up;
        tileRect.anchorMax = Vector2.up;
        tileRect.sizeDelta = new Vector2(_tileWidth, _tileHeight);
        tileRect.position = new Vector3(
            (x - 0.5f) * (_tileWidth + _tilePad) + _inventoryPad / 2f, 
            (y - 0.5f) * -(_tileHeight + _tilePad) - _inventoryPad / 2f - 20f, 
            0
        );
    }

    /// <summary>
    /// Closes menu by destroying generated inventory parent.
    /// </summary>
    void CloseMenu()
    {
        foreach(GameObject inventoryObject in _inventoryObject)
        {
            Destroy(inventoryObject);
        }
    }
}
