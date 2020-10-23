namespace SudokuLogic.Constraints.Interface
{
    public interface IConstraint
    {
        void DoWork(Cell currentCell, Board board);

        bool Check(Board board);
    }
}
