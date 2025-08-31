using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Scripts;

using UnityEngine.Tilemaps;

namespace Scripts {
    public class Tiles {
        private TileValue value { get; set; }
        private bool inViewRange { get; set; }

        public Tiles(TileValue value, bool inViewRange) {
            this.value = value;
            this.inViewRange = inViewRange;
        }

        public TileValue GetTileValue() {
            return value;
        }

        public void SetTileValue(TileValue value) {
            this.value = value;
        }
    }
}