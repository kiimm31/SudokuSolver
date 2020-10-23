namespace SudokuLogic.Constrains.Interface
{
    public interface IConstrain
    {
        void DoWork(Cell currentCell, Board board);

        bool Check(Board board);
    }
}
