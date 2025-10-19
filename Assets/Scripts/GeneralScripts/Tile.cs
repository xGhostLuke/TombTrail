using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Scripts;

using UnityEngine.Tilemaps;

public class Tiles {
    private TileValue value { get; set; }
    private bool inViewRange { get; set; }
    private bool wasSeen { get; set; }

    private List<ItemObject> itemsOnTile;

    public Tiles(TileValue value, bool inViewRange) {
        this.value = value;
        this.inViewRange = inViewRange;
        wasSeen = false;
    }

    public TileValue GetTileValue() {
        return value;
    }

    public void SetTileValue(TileValue value) {
        this.value = value;
    }

    public void SetWasSeen(bool b) {
        wasSeen = b;
    }

    public bool GetWasSeen() {
        return wasSeen;
    }

    public void addItemToItemList(ItemObject item) {
        itemsOnTile.Add(item);
    }

    public List<ItemObject> getItemList() {
        return itemsOnTile;
    }
}