using System;
namespace DruidCraftCalendar.Rings
{
    public interface INodesModel
    {
        int GetNode1Position();
        int GetNode2Position();
        void Incriment();
        void SetNode1Position(int position);
        void SetNode2Position(int position);
    }
}
