namespace Haivan.Puzzles.PipeFlow
{
    interface IPipeFlowPuzzle
    {
        public bool IsConnected();

        public void RotatePipe((int, int) position);
    }
}
