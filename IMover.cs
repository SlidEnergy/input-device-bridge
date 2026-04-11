
namespace tser
{
    public interface IMover
    {
        Task MoveAndClick(int toX, int toY);
        Task MoveAndClick(int x1, int y1, int x2, int y2);
        void MoveSmooth(int toX, int toY);
        void MoveSmooth(int x1, int y1, int x2, int y2);
    }
}