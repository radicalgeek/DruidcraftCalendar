namespace DruidCraftCalendar.Rings
{
    public class NodesModel : INodesModel
    {
        private int _node1Count;
        private int _node2Count;

        public void Incriment()
        {
            if (_node1Count == 1)
            {
                _node1Count = 57;
            }
            _node1Count -= 1;

            if (_node2Count == 1)
            {
                _node2Count = 57;
            }
            _node2Count -= 1;
        }

        public int GetNode1Position()
        {
            return _node1Count;
        }

        public int GetNode2Position()
        {
            return _node2Count;
        }

        public void SetNode1Position(int position)
        {
            _node1Count = position;
        }

        public void SetNode2Position(int position)
        {
            _node2Count = position;
        }
    }
}