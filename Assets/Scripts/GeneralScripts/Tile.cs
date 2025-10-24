using System.Collections.Generic;

using Scripts;

public class Tiles {
    private TileValue value { get; set; }
    private bool wasSeen { get; set; }

    private List<ItemObject> itemsOnTile = new List<ItemObject>();

    public Tiles(TileValue value) {
        this.value = value;
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