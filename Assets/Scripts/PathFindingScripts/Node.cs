using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scripts {
    public class Nodes {
        public int x;
        public int y;
        public bool walkable;

        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public Nodes parent;

        public Nodes(int x, int y, bool walkable) {
            this.x = x;
            this.y = y;
            this.walkable = walkable;
        }
    }
}